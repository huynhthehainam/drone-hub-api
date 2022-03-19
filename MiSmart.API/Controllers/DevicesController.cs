

using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using System.Linq;
using MiSmart.Infrastructure.Commands;
using MiSmart.DAL.ViewModels;
using System.Linq.Expressions;
using MiSmart.Infrastructure.ViewModels;
using MiSmart.API.ControllerBases;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using MiSmart.Infrastructure.Permissions;
using MiSmart.API.Permissions;
using System.Collections.Generic;
using MiSmart.Infrastructure.Minio;
using Microsoft.AspNetCore.Authorization;
using MiSmart.Infrastructure.Services;

namespace MiSmart.API.Controllers
{
    public class DevicesController : AuthorizedAPIControllerBase
    {

        public DevicesController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {

        }

        [HttpPost("{id:int}/AssignExecutionCompany")]
        public IActionResult AssignExecutionCompany([FromServices] CustomerUserRepository customerUserRepository, [FromServices] ExecutionCompanyRepository executionCompanyRepository,
        [FromServices] DeviceRepository deviceRepository,
         [FromServices] TeamUserRepository teamUserRepository, [FromRoute] Int32 id, [FromBody] AssigningDeviceExecutionCompanyCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            CustomerUser customerUser = customerUserRepository.GetByPermission(CurrentUser.ID);
            if (customerUser is null)
            {
                response.AddNotAllowedErr();
            }

            var executionCompany = executionCompanyRepository.Get(ww => ww.ID == command.ExecutionCompanyID.GetValueOrDefault());
            if (executionCompany is null)
            {
                response.AddInvalidErr("ExecutionCompanyID");
            }
            Expression<Func<Device, Boolean>> query = ww => (ww.ID == id) && (ww.CustomerID == customerUser.CustomerID);
            var device = deviceRepository.Get(query);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }
            device.ExecutionCompany = executionCompany;
            deviceRepository.Update(device);
            response.SetUpdatedMessage();

            return response.ToIActionResult();
        }

        [HttpPost("{id:int}/AssignTeam")]
        public IActionResult AssignTeam([FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromRoute] Int32 id,
        [FromServices] DeviceRepository deviceRepository,
        [FromServices] TeamRepository teamRepository, [FromBody] AssigningDeviceTeamCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = executionCompanyUserRepository.GetByPermission(CurrentUser.ID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }
            var device = deviceRepository.Get(ww => ww.ID == id && ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }

            var team = teamRepository.Get(ww => ww.ID == command.TeamID.GetValueOrDefault() && ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID);
            if (team is null)
            {
                response.AddInvalidErr("TeamID");
            }

            device.Team = team;
            deviceRepository.Update(device);
            response.SetUpdatedMessage();

            return response.ToIActionResult();
        }




        [HttpGet]
        public IActionResult GetList([FromServices] CustomerUserRepository customerUserRepository,
        [FromServices] DeviceRepository deviceRepository,

         [FromServices] TeamUserRepository teamUserRepository, [FromQuery] PageCommand pageCommand, [FromQuery] String search, [FromQuery] String mode = "Small")
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            CustomerUser customerUser = customerUserRepository.GetByPermission(CurrentUser.ID);

            if (customerUser is null)
            {
                response.AddNotAllowedErr();
            }

            Expression<Func<Device, Boolean>> query = ww => (true)
            && (!String.IsNullOrWhiteSpace(search) ? ww.Name.ToLower().Contains(search.ToLower()) : true);
            if (mode == "Large")
            {

            }
            else if (mode == "Medium")
            {

            }
            else
            {
                var listResponse = deviceRepository.GetListResponseView<SmallDeviceViewModel>(pageCommand, query);
                listResponse.SetResponse(response);
            }
            return response.ToIActionResult();
        }
        [HttpGet("{id:int}/GetToken")]
        public IActionResult GetToken([FromServices] CustomerUserRepository customerUserRepository, [FromServices] TeamUserRepository teamUserRepository, [FromServices] DeviceRepository deviceRepository, [FromRoute] Int32 id)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();

            CustomerUser customerUser = customerUserRepository.GetByPermission(CurrentUser.ID);
            if (customerUser is null)
            {
                response.AddNotAllowedErr();
            }


            Expression<Func<Device, Boolean>> query = ww => (ww.ID == id)
             && (true);
            var device = deviceRepository.Get(query);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }

            response.SetData(ViewModelHelpers.ConvertToViewModel<Device, SuperSmallDeviceViewModel>(device));

            return response.ToIActionResult();
        }
        [HttpPatch("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult PatchDevice([FromServices] DeviceRepository deviceRepository, [FromServices] DeviceModelRepository deviceModelRepository, [FromRoute] Int32 id, [FromBody] PatchingDeviceCommand command)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();

            var device = deviceRepository.Get(ww => ww.ID == id);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }


            device.Name = String.IsNullOrWhiteSpace(command.Name) ? device.Name : command.Name;

            if (command.DeviceModelID.HasValue)
            {
                var deviceModel = deviceModelRepository.Get(ww => ww.ID == command.DeviceModelID.GetValueOrDefault());
                if (deviceModel is null)
                {
                    response.AddInvalidErr("DeviceModelID");
                }
                device.DeviceModel = deviceModel;
            }


            deviceRepository.Update(device);

            response.SetUpdatedMessage();

            return response.ToIActionResult();
        }
        [HttpPost("UploadOfflineStats")]
        [AllowAnonymous]
        public IActionResult UploadOfflineStats([FromBody] AddingBulkOfflineFlightStatsCommand command,
        [FromServices] FlightStatRepository flightStatRepository,
         [FromServices] DeviceRepository deviceRepository, [FromServices] JWTService jwtService)
        {
            var response = actionResponseFactory.CreateInstance();
            List<FlightStat> flightStats = new List<FlightStat>();
            foreach (var item in command.Data)
            {
                var deviceJWT = jwtService.GetUser(item.DeviceAccessToken);
                if (deviceJWT.Type == "Device")
                {

                    var device = deviceRepository.Get(ww => ww.ID == deviceJWT.ID);
                    if (device is not null)
                    {

                        if (item.FlywayPoints.Count == 0)
                        {
                            response.AddInvalidErr("FlywayPoints");
                        }
                        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                        item.FlywayPoints.Add(JsonSerializer.Deserialize<LocationPoint>(JsonSerializer.Serialize(item.FlywayPoints[0])));

                        var stat = new FlightStat
                        {
                            FlightDuration = item.FlightDuration.GetValueOrDefault(),
                            FieldName = item.FieldName,
                            Flights = item.Flights.GetValueOrDefault(),
                            FlightTime = item.FlightTime ?? DateTime.Now,
                            FlywayPoints = geometryFactory.CreateLineString(item.FlywayPoints.Select(ww => new Coordinate(ww.Longitude.GetValueOrDefault(), ww.Latitude.GetValueOrDefault())).ToArray()),
                            PilotName = item.PilotName,
                            CreatedTime = DateTime.Now,
                            CustomerID = device.CustomerID,
                            DeviceID = device.ID,
                            DeviceName = device.Name,
                            TaskLocation = item.TaskLocation,
                            TaskArea = item.TaskArea.GetValueOrDefault(),
                        };
                        flightStatRepository.Create(stat);
                        if (device.Team is not null)
                        {
                            device.Team.TotalFlights += item.Flights.GetValueOrDefault();
                            device.Team.TotalFlightDuration += item.FlightDuration.GetValueOrDefault();
                            device.Team.TotalTaskArea += item.TaskArea.GetValueOrDefault();
                        }

                        deviceRepository.Update(device);
                        flightStats.Add(stat);
                    }
                }

            }
            response.SetData(flightStats.Select(fs => new { ID = fs.ID }));
            return response.ToIActionResult();
        }
    }
    public class AuthorizedDevicesController : AuthorizedDeviceAPIControllerBase
    {
        public AuthorizedDevicesController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }



        [HttpPost("me/TelemetryRecords")]
        public IActionResult CreateTelemetryRecord([FromServices] DeviceRepository deviceRepository, [FromServices] TelemetryGroupRepository telemetryGroupRepository, [FromBody] AddingBulkTelemetryRecordCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = deviceRepository.Get(ww => ww.ID == CurrentDevice.ID);

            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }
            if (command.Data.Count == 0)
            {
                response.AddInvalidErr("Data");
            }


            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            TimeSpan span = new TimeSpan(0, 0, 0, 0, 5000);
            TimeSpan eachSpan = span / command.Data.Count;
            var records = new List<TelemetryRecord>();
            var startedTime = DateTime.Now - span;
            for (int i = 0; i < command.Data.Count; i++)
            {
                var item = command.Data[i];
                records.Add(new TelemetryRecord
                {
                    CreatedTime = startedTime.Add(eachSpan * i),
                    AdditionalInformation = item.AdditionalInformation,
                    Direction = item.Direction.GetValueOrDefault(),
                    LocationPoint = geometryFactory.CreatePoint(new Coordinate(item.Longitude.GetValueOrDefault(), item.Latitude.GetValueOrDefault())),

                });
            }
            TelemetryGroup group = new TelemetryGroup()
            {
                DeviceID = device.ID,
                Records = records,
            };

            telemetryGroupRepository.Create(group);

            device.LastGroupID = group.ID;

            deviceRepository.Update(device);


            response.SetCreatedObject(group);
            return response.ToIActionResult();
        }
        [HttpPost("me/Logs")]
        public IActionResult UploadLogFile([FromServices] DeviceRepository deviceRepository, [FromServices] MinioService minioService, [FromForm] AddingLogFileCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = deviceRepository.Get(ww => ww.ID == CurrentDevice.ID);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }


            LogFile logFile = new LogFile
            {
                FileUrl = minioService.PutFile(command.File, new String[] { "drone-hub-api", "logs", $"{device.ID}_{device.Name}" }),
            };

            device.LogFiles.Add(logFile);

            deviceRepository.Update(device);


            response.SetCreatedObject(logFile);

            return response.ToIActionResult();
        }

        [HttpPost("me/FlightStats")]
        public IActionResult CreateFlightStat([FromServices] DeviceRepository deviceRepository, [FromServices] FlightStatRepository flightStatRepository, [FromBody] AddingFlightStatCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = deviceRepository.Get(ww => ww.ID == CurrentDevice.ID);
            if (command.FlywayPoints.Count == 0)
            {
                response.AddInvalidErr("FlywayPoints");
            }
            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            command.FlywayPoints.Add(JsonSerializer.Deserialize<LocationPoint>(JsonSerializer.Serialize(command.FlywayPoints[0])));

            var stat = new FlightStat
            {
                FlightDuration = command.FlightDuration.GetValueOrDefault(),
                FieldName = command.FieldName,
                Flights = command.Flights.GetValueOrDefault(),
                FlightTime = command.FlightTime ?? DateTime.Now,
                FlywayPoints = geometryFactory.CreateLineString(command.FlywayPoints.Select(ww => new Coordinate(ww.Longitude.GetValueOrDefault(), ww.Latitude.GetValueOrDefault())).ToArray()),
                PilotName = command.PilotName,
                CreatedTime = DateTime.Now,
                CustomerID = device.CustomerID,
                DeviceID = device.ID,
                DeviceName = device.Name,
                TaskLocation = command.TaskLocation,
                TaskArea = command.TaskArea.GetValueOrDefault(),

            };
            flightStatRepository.Create(stat);
            if (device.Team is not null)
            {
                device.Team.TotalFlights += command.Flights.GetValueOrDefault();
                device.Team.TotalFlightDuration += command.FlightDuration.GetValueOrDefault();
                device.Team.TotalTaskArea += command.TaskArea.GetValueOrDefault();
            }
            deviceRepository.Update(device);

            response.SetCreatedObject(stat);

            return response.ToIActionResult();
        }
        [HttpPost("me/Plans")]
        public IActionResult CreatePlan([FromServices] DeviceRepository deviceRepository, [FromServices] PlanRepository planRepository, [FromForm] AddingPlanCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = deviceRepository.Get(ww => ww.ID == CurrentDevice.ID);

            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var plan = planRepository.Get(ww => ww.FileName == command.File.FileName && ww.Device == device);
            if (plan is null)
            {
                plan = new Plan { FileName = command.File.FileName, Device = device };
            }

            plan.Location = geometryFactory.CreatePoint(new Coordinate(command.Longitude.GetValueOrDefault(), command.Latitude.GetValueOrDefault()));
            plan.FileName = command.File.FileName;
            plan.FileBytes = command.GetFileBytes();

            if (plan.ID == 0)
            {
                planRepository.Create(plan);
            }
            else
            {
                planRepository.Update(plan);
            }
            response.SetCreatedObject(plan);

            return response.ToIActionResult();
        }
        [HttpGet("RetrivePlans")]
        public IActionResult GetPlans([FromServices] PlanRepository planRepository, [FromQuery] PageCommand pageCommand, [FromQuery] String search, [FromQuery] Double? latitude, [FromQuery] Double? longitude, [FromQuery] Double? range)
        {
            var response = actionResponseFactory.CreateInstance();
            Point centerLocation = null;
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            if (latitude.HasValue && longitude.HasValue && range.HasValue)
            {
                centerLocation = geometryFactory.CreatePoint(new Coordinate(longitude.GetValueOrDefault(), latitude.GetValueOrDefault()));
            }
            Expression<Func<Plan, Boolean>> query = ww => (String.IsNullOrWhiteSpace(search) ?
            ((centerLocation != null) ? (ww.Location.Distance(centerLocation) < range.GetValueOrDefault()) : true)
            : ww.FileName.ToLower().Contains(search.ToLower()));
            var listResponse = planRepository.GetListResponseView<SmallPlanViewModel>(pageCommand, query);
            if (centerLocation is not null)
            {
                foreach (var item in listResponse.Data)
                {
                    item.CalculateDistance(centerLocation);
                }
            }
            listResponse.SetResponse(response);

            return response.ToIActionResult();

        }

        [HttpPost("RetrivePlanFile")]
        public IActionResult GetFile([FromServices] PlanRepository planRepository, [FromBody] RetrievingPlanFileCommand command)
        {
            var response = actionResponseFactory.CreateInstance();

            var plan = planRepository.Get(ww => ww.ID == command.PlanID);
            if (plan is null)
            {
                response.AddNotFoundErr("Plan");
            }

            response.SetFile(plan.FileBytes, "application/octet-stream", plan.FileName);



            return response.ToIActionResult();
        }



    }
}