
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
using System.Text.Json;
using MiSmart.Infrastructure.Constants;
using System.Net.Http;
using MiSmart.API.Helpers;

namespace MiSmart.API.Controllers
{
    public class FlightStatsController : AuthorizedAPIControllerBase
    {
        public FlightStatsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpPost("GetFlightStatsFromTM")]
        public async Task<IActionResult> GetFlightStatsFromTM([FromServices] FlightStatRepository flightStatRepository,
                [FromQuery] PageCommand pageCommand,
                [FromBody] GettingFlightStatsFromTMCommand command,
                       [FromServices] IOptions<ActionResponseSettings> options)
        {
            FlightStatsActionResponse response = new FlightStatsActionResponse();
            response.ApplySettings(options.Value);
            Expression<Func<FlightStat, Boolean>> query = ww => ww.TMUserUID == command.TMUserUID;



            var listResponse = await flightStatRepository.GetListFlightStatsViewAsync<SmallFlightStatViewModel>(pageCommand, query, ww => ww.FlightTime, false);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetFlightStats([FromServices] FlightStatRepository flightStatRepository,
        [FromServices] TeamUserRepository teamUserRepository,
        [FromServices] IOptions<ActionResponseSettings> options,
        [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] CustomerUserRepository customerUserRepository, [FromQuery] PageCommand pageCommand,
         [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Int32? executionCompanyID,
         [FromQuery] Int32? customerID,
         [FromQuery] Int64? teamID, [FromQuery] Int32? deviceID, [FromQuery] Int32? deviceModelID,
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
                    && (true);
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
                    && (executionCompanyID.HasValue ? (ww.ExecutionCompanyID == executionCompanyID.GetValueOrDefault()) : true);
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

                   && (executionCompanyUser.Type == ExecutionCompanyUserType.Member ? (teamIDs.Contains(ww.TeamID.GetValueOrDefault())) : true);
            }
            var listResponse = await flightStatRepository.GetListFlightStatsViewAsync<SmallFlightStatViewModel>(pageCommand, query, ww => ww.FlightTime, false);
            List<Task> tasks = new List<Task> { };
            List<String> test = new List<String> {};            for (var i = 0; i < listResponse.Data.Count; i++)
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


            flightStat.FieldName = String.IsNullOrEmpty(command.FieldName) ? flightStat.FieldName : command.FieldName;
            flightStat.TaskLocation = String.IsNullOrEmpty(command.TaskLocation) ? flightStat.TaskLocation : command.TaskLocation;
            flightStat.TMUser = command.TMUser == null ? flightStat.TMUser : JsonDocument.Parse(JsonSerializer.Serialize(command.TMUser, JsonSerializerDefaultOptions.CamelOptions));
            flightStat.Medicines = command.Medicines.Count == 0 ? flightStat.Medicines : JsonDocument.Parse(JsonSerializer.Serialize(command.Medicines, JsonSerializerDefaultOptions.CamelOptions));
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