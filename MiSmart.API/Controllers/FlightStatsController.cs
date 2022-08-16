
using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using MiSmart.Infrastructure.Commands;
using MiSmart.DAL.ViewModels;
using MiSmart.Infrastructure.ViewModels;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using MiSmart.DAL.Responses;
using Microsoft.Extensions.Options;
using MiSmart.Infrastructure.Permissions;
using MiSmart.API.Permissions;
using MiSmart.API.Commands;
using System.Threading.Tasks;
using System.Net.Http;
using MiSmart.API.Helpers;
using MiSmart.Infrastructure.Minio;
using Microsoft.AspNetCore.Authorization;
using MiSmart.API.Settings;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using MiSmart.Infrastructure.Constants;

namespace MiSmart.API.Controllers
{
    public class FlightStatsController : AuthorizedAPIControllerBase
    {
        public FlightStatsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        [HttpPost("UpdateFromTMUser")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateFlightStatFromTMUser([FromBody] UpdatingFlightStatsFromTMUserCommand command, [FromServices] FlightStatRepository flightStatRepository, [FromServices] IOptions<FarmAppSettings> options)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var settings = options.Value;
            if (command.SecretKey != settings.SecretKey)
            {
                actionResponse.AddAuthorizationErr();
            }

            var flightStat = await flightStatRepository.GetAsync(ww => ww.ID == command.FlightStatID);
            if (flightStat is null)
            {
                actionResponse.AddInvalidErr("FlightStatID");
            }
            flightStat.Medicines = command.Medicines;
            await flightStatRepository.UpdateAsync(flightStat);
            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }

        [HttpPost("UpdateFromTM")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateFlightStatFromTM([FromBody] UpdatingFlightStatsFromTMCommand command, [FromServices] FlightStatRepository flightStatRepository, [FromServices] IOptions<FarmAppSettings> options)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var settings = options.Value;
            if (command.SecretKey != settings.SecretKey)
            {
                actionResponse.AddAuthorizationErr();
            }
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            foreach (var item in command.Data)
            {
                var coords = new List<Coordinate>();
                if (item.FieldPoints.Count < 2)
                {
                    continue;
                }
                item.FieldPoints.Add(new LocationPoint { Latitude = item.FieldPoints[0].Latitude, Longitude = item.FieldPoints[0].Longitude });
                foreach (var point in item.FieldPoints)
                {
                    Console.WriteLine($"{{lng: {point.Longitude}, lat: {point.Latitude}}},");
                    coords.Add(new Coordinate(point.Longitude.GetValueOrDefault(), point.Latitude.GetValueOrDefault()));
                }
                var polygon = geometryFactory.CreatePolygon(coords.ToArray());
                var flightStats = await flightStatRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.IsBoundaryArchived && ww.Boundary != null && !ww.IsTMInformationArchived, ww => ww.FlightTime, false);

                foreach (var flightStat in flightStats)
                {
                    Polygon intersection = (Polygon)polygon.Intersection(flightStat.Boundary);

                    var fieldPoints = new List<Object>();


                    if (intersection.Coordinates.Count() > 0)
                    {
                        var intersectionArea = intersection.Area;
                        var boundaryArea = flightStat.Boundary.Area;
                        var iou = intersectionArea / boundaryArea;
                        if (iou > settings.IOU)
                        {
                            flightStat.TMUserUUID = item.User.UUID;
                            flightStat.TMUser = JsonDocument.Parse(JsonSerializer.Serialize(item.User, JsonSerializerDefaultOptions.CamelOptions));
                            flightStat.TMPlantID = item.Plant.ID;
                            flightStat.TMPlant = JsonDocument.Parse(JsonSerializer.Serialize(item.Plant, JsonSerializerDefaultOptions.CamelOptions));
                            flightStat.TMFieldID = item.ID;
                            flightStat.TMField = JsonDocument.Parse(JsonSerializer.Serialize(new { ID = item.ID }, JsonSerializerDefaultOptions.CamelOptions));
                            flightStat.IsTMInformationArchived = true;
                            await flightStatRepository.UpdateAsync(flightStat);
                        }
                    }


                }
            }


            return actionResponse.ToIActionResult();
        }

        [HttpGet("GetFlightStatsFromTM")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFlightStatsFromTM([FromServices] FlightStatRepository flightStatRepository,
                [FromQuery] PageCommand pageCommand,
                [FromQuery] String tmUserUUID,
                [FromQuery] DateTime? from,
                [FromQuery] DateTime? to,
                       [FromServices] IOptions<ActionResponseSettings> options)
        {
            FlightStatsActionResponse response = new FlightStatsActionResponse();
            response.ApplySettings(options.Value);


            Expression<Func<FlightStat, Boolean>> query = ww => String.IsNullOrEmpty(tmUserUUID) ? false : ww.TMUserUUID == tmUserUUID
                && (from.HasValue ? (ww.FlightTime >= from.Value) : true)
                && (to.HasValue ? (ww.FlightTime <= to.Value.AddDays(1)) : true)
                && (ww.IsTMInformationArchived);

            var listResponse = await flightStatRepository.GetListFlightStatsViewAsync<SmallFlightStatViewModel>(pageCommand, query, ww => ww.FlightTime, false);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }

        [HttpPost("{id:Guid}/AddReportRecord")]
        public async Task<IActionResult> AddReportRecord([FromServices] FlightStatRepository flightStatRepository, [FromServices] MinioService minioService, [FromForm] AddingFlightStatReportRecordCommand command, [FromRoute] Guid id, [FromServices] FlightStatReportRecordRepository flightStatReportRecordRepository)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator)
            {
                actionResponse.AddNotAllowedErr();
            }

            var flightStat = await flightStatRepository.GetAsync(ww => ww.ID == id);
            if (flightStat is null)
            {
                actionResponse.AddNotFoundErr("FlightStat");
            }
            List<String> images = new List<String>();
            foreach (var imageCommand in command.Files)
            {
                var url = await minioService.PutFileAsync(imageCommand, new String[] { "drone-hub-api", "flight-stat-report", $"{flightStat.ID}" });
                images.Add(url);
            }
            FlightStatReportRecord flightStatReportRecord = new FlightStatReportRecord { FlightStat = flightStat, Reason = command.Reason, Images = images };
            await flightStatReportRecordRepository.CreateAsync(flightStatReportRecord);
            actionResponse.SetCreatedObject(flightStatReportRecord);
            return actionResponse.ToIActionResult();
        }
        [HttpPost("{id:Guid}/UpdateFromAdministrator")]
        public async Task<IActionResult> UpdateFromAdministrator([FromRoute] Guid id, [FromServices] FlightStatRepository flightStatRepository, [FromBody] UpdatingFlightStatFromAdministratorCommand command)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator)
            {
                actionResponse.AddNotAllowedErr();
            }
            var flightStat = await flightStatRepository.GetAsync(ww => ww.ID == id);
            if (flightStat is null)
            {
                actionResponse.AddNotFoundErr("FlightStat");
            }

            flightStat.Status = command.Status;
            flightStat.StatusUpdatedTime = DateTime.UtcNow;
            flightStat.StatusUpdatedUserUUID = CurrentUser.UUID;

            await flightStatRepository.UpdateAsync(flightStat);

            return actionResponse.ToIActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetFlightStats([FromServices] FlightStatRepository flightStatRepository, [FromServices] TeamUserRepository teamUserRepository, [FromServices] IOptions<ActionResponseSettings> options, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromServices] IHttpClientFactory httpClientFactory, [FromServices] CustomerUserRepository customerUserRepository, [FromQuery] PageCommand pageCommand, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Int32? executionCompanyID, [FromQuery] Int32? customerID, [FromQuery] Int64? teamID, [FromQuery] Int32? deviceID, [FromQuery] Int32? deviceModelID, [FromQuery] Boolean? getMappingRecords, [FromQuery] Boolean? getSeedingRecords,
         [FromQuery] Boolean? getSprayingRecords,
         [FromQuery] String relation = "Owner")
        {
            FlightStatsActionResponse response = new FlightStatsActionResponse();
            response.ApplySettings(options.Value);
            Expression<Func<FlightStat, Boolean>> query = ww => false;

            if (relation == "Owner")
            {

                CustomerUser customerUser = await customerUserRepository.GetByPermissionAsync(CurrentUser.UUID);
                if (customerUser is null)
                {
                    response.AddNotAllowedErr();
                }

                query = ww => (ww.CustomerID == customerUser.CustomerID)
                    && (teamID.HasValue ? (ww.Device.TeamID == teamID.Value) : true)
                    && (deviceID.HasValue ? (ww.DeviceID == deviceID.Value) : true)
                    && (from.HasValue ? (ww.FlightTime >= from.Value) : true)
                    && (to.HasValue ? (ww.FlightTime <= to.Value.AddDays(1)) : true)
                    && (deviceModelID.HasValue ? (ww.Device.DeviceModelID == deviceModelID.Value) : true)
                    && (executionCompanyID.HasValue ? (ww.ExecutionCompanyID == executionCompanyID.GetValueOrDefault()) : true)
                    && (getMappingRecords.HasValue ? (getMappingRecords.GetValueOrDefault() ? true : (ww.TaskArea > 0)) : true)
                    && (getSprayingRecords.HasValue ? (getSprayingRecords.GetValueOrDefault() ? true : (ww.TaskArea == 0)) : true)
                    && (getSeedingRecords.HasValue ? true : true)
                    ;
            }
            else if (relation == "Administrator")
            {
                if (!CurrentUser.IsAdministrator)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => (teamID.HasValue ? (ww.Device.TeamID == teamID.Value) : true)
                    && (deviceID.HasValue ? (ww.DeviceID == deviceID.Value) : true)
                    && (from.HasValue ? (ww.FlightTime >= from.Value) : true)
                    && (to.HasValue ? (ww.FlightTime <= to.Value.AddDays(1)) : true)
                    && (customerID.HasValue ? (ww.CustomerID == customerID.Value) : true)
                    && (deviceModelID.HasValue ? (ww.Device.DeviceModelID == deviceModelID.Value) : true)
                    && (executionCompanyID.HasValue ? (ww.ExecutionCompanyID == executionCompanyID.GetValueOrDefault()) : true)
                      && (getMappingRecords.HasValue ? (getMappingRecords.GetValueOrDefault() ? true : (ww.TaskArea > 0)) : true)
                    && (getSprayingRecords.HasValue ? (getSprayingRecords.GetValueOrDefault() ? true : (ww.TaskArea == 0)) : true)
                    && (getSeedingRecords.HasValue ? true : true)
                    ;
            }
            else
            {
                ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.UUID);
                if (executionCompanyUser is null)
                {
                    response.AddNotAllowedErr();
                }
                List<Int64> teamIDs = teamUserRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.ExecutionCompanyUserID == executionCompanyUser.ID).Result.Select(ww => ww.TeamID).ToList();
                query = ww => (ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID)
                   && (teamID.HasValue ? (ww.Device.TeamID == teamID.Value) : true)
                   && (deviceID.HasValue ? (ww.DeviceID == deviceID.Value) : true)
                   && (from.HasValue ? (ww.FlightTime >= from.Value) : true)
                   && (to.HasValue ? (ww.FlightTime <= to.Value.AddDays(1)) : true)
                   && (customerID.HasValue ? (ww.CustomerID == customerID.GetValueOrDefault()) : true)
                    && (deviceModelID.HasValue ? (ww.Device.DeviceModelID == deviceModelID.Value) : true)

                   && (executionCompanyUser.Type == ExecutionCompanyUserType.Member ? (teamIDs.Contains(ww.TeamID.GetValueOrDefault())) : true)
                     && (getMappingRecords.HasValue ? (getMappingRecords.GetValueOrDefault() ? true : (ww.TaskArea > 0)) : true)
                    && (getSprayingRecords.HasValue ? (getSprayingRecords.GetValueOrDefault() ? true : (ww.TaskArea == 0)) : true)
                    && (getSeedingRecords.HasValue ? true : true)
                   ;
            }
            var listResponse = await flightStatRepository.GetListFlightStatsViewAsync<SmallFlightStatViewModel>(pageCommand, query, ww => ww.FlightTime, false);
            List<Task> tasks = new List<Task> { };
            List<String> test = new List<String> { }; for (var i = 0; i < listResponse.Data.Count; i++)
            {
                if (i % 5 == 0)
                {
                    Task.WaitAll(tasks.ToArray());
                    tasks = new List<Task> { };
                }
                var client = httpClientFactory.CreateClient();
                var item = listResponse.Data[i];
                tasks.Add(BingLocationHelper.UpdateLocation(listResponse, i, httpClientFactory));
            }
            Task.WaitAll(tasks.ToArray());

            listResponse.SetResponse(response);
            return response.ToIActionResult();
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetByID([FromServices] FlightStatRepository flightStatRepository, [FromServices] CustomerUserRepository customerUserRepository,
        [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromRoute] Guid id,
        [FromQuery] String relation = "Owner")
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<FlightStat, Boolean>> query = ww => false;
            if (relation == "Owner")
            {
                CustomerUser customerUser = await customerUserRepository.GetByPermissionAsync(CurrentUser.UUID);
                if (customerUser is null)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => ww.ID == id && ww.CustomerID == customerUser.CustomerID;
            }
            else if (relation == "Administrator")
            {
                if (!CurrentUser.IsAdministrator)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => ww.ID == id;
            }
            else
            {
                ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.UUID);
                if (executionCompanyUser is null)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => ww.ID == id && ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID;
            }
            var flightStat = await flightStatRepository.GetAsync(query);

            if (flightStat is null)
            {
                response.AddNotFoundErr("FlightStat");
            }
            response.SetData(ViewModelHelpers.ConvertToViewModel<FlightStat, LargeFlightStatViewModel>(flightStat));
            return response.ToIActionResult();
        }

        [HttpGet("{id:Guid}/GetDetailByIDFromTM")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDetailByIDFromTM([FromServices] FlightStatRepository flightStatRepository, [FromServices] CustomerUserRepository customerUserRepository,
        [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromRoute] Guid id, [FromQuery] String tmUserUUID)
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<FlightStat, Boolean>> query = ww => (String.IsNullOrEmpty(tmUserUUID) ? false : (ww.TMUserUUID == tmUserUUID && ww.ID == id)) && (ww.IsTMInformationArchived);
            var flightStat = await flightStatRepository.GetAsync(query);

            if (flightStat is null)
            {
                response.AddNotFoundErr("FlightStat");
            }
            response.SetData(ViewModelHelpers.ConvertToViewModel<FlightStat, LargeFlightStatViewModel>(flightStat));
            return response.ToIActionResult();
        }

        [HttpPost("{id:Guid}/UpdateFromExecutor")]
        public async Task<IActionResult> UpdateFromExecutor([FromRoute] Guid id, [FromServices] TeamRepository teamRepository,
         [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromServices] FlightStatRepository flightStatRepository,
         [FromServices] ExecutionCompanyUserFlightStatRepository executionCompanyUserFlightStatRepository,
         [FromBody] UpdatingFlightStatFromExecutorCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.UUID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }


            var flightStat = await flightStatRepository.GetAsync(ww => ww.ID == id && ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID);
            if (flightStat is null)
            {
                response.AddNotFoundErr("FlightStat");
            }

            if (command.TeamID.HasValue)
            {
                var team = await teamRepository.GetAsync(ww => ww.ID == command.TeamID.GetValueOrDefault() && ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID);
                if (team is null)
                {
                    response.AddInvalidErr("TeamID");
                }
                flightStat.Team = team;
                List<TeamUser> teamUsers = team.TeamUsers.ToList();

                var executionCompanyUserFlightStats = await executionCompanyUserFlightStatRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.FlightStatID == flightStat.ID);
                await executionCompanyUserFlightStatRepository.DeleteRangeAsync(executionCompanyUserFlightStats);

                foreach (var teamUser in teamUsers)
                {
                    ExecutionCompanyUserFlightStat executionCompanyUserFlightStat = await executionCompanyUserFlightStatRepository.CreateAsync(new ExecutionCompanyUserFlightStat
                    {
                        ExecutionCompanyUserID = teamUser.ExecutionCompanyUserID,
                        FlightStatID = flightStat.ID,
                        Type = teamUser.Type,
                    });
                }
            }


            // flightStat.FieldName = String.IsNullOrEmpty(command.FieldName) ? flightStat.FieldName : command.FieldName;
            // flightStat.TaskLocation = String.IsNullOrEmpty(command.TaskLocation) ? flightStat.TaskLocation : command.TaskLocation;
            // if (command.TMUser is not null)
            // {
            //     flightStat.TMUser = JsonDocument.Parse(JsonSerializer.Serialize(command.TMUser, JsonSerializerDefaultOptions.CamelOptions));
            //     flightStat.TMUserUID = command.TMUser.UID;
            // }

            // flightStat.Medicines = command.Medicines.Count == 0 ? flightStat.Medicines : JsonDocument.Parse(JsonSerializer.Serialize(command.Medicines, JsonSerializerDefaultOptions.CamelOptions));
            await flightStatRepository.UpdateAsync(flightStat);
            response.SetUpdatedMessage();

            return response.ToIActionResult();
        }

        [HttpDelete("{id:Guid}")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> DeleteByID([FromServices] FlightStatRepository flightStatRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromRoute] Guid id)
        {
            var response = actionResponseFactory.CreateInstance();
            var flightStat = await flightStatRepository.GetAsync(ww => ww.ID == id);

            if (flightStat is null)
            {
                response.AddNotFoundErr("FlightStat");
            }
            await flightStatRepository.DeleteAsync(flightStat);
            response.SetNoContent();
            return response.ToIActionResult();
        }
    }
}