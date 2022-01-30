

using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System;
using MiSmart.Infrastructure.Helpers;
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
using MiSmart.Infrastructure.QueuedBackgroundTasks;
using System.Threading.Tasks;
using System.Threading;
using MiSmart.Infrastructure.Permissions;
using MiSmart.API.Permissions;

namespace MiSmart.API.Controllers
{
    public class DevicesController : AuthorizedAPIControllerBase
    {

        private readonly CustomerRepository customerRepository;
        private readonly TeamRepository teamRepository;
        private readonly DeviceRepository deviceRepository;
        private readonly DeviceModelRepository deviceModelRepository;
        public DevicesController(IActionResponseFactory actionResponseFactory, CustomerRepository customerRepository, TeamRepository teamRepository, DeviceRepository deviceRepository, DeviceModelRepository deviceModelRepository) : base(actionResponseFactory)
        {
            this.teamRepository = teamRepository;
            this.deviceModelRepository = deviceModelRepository;
            this.deviceRepository = deviceRepository;
            this.customerRepository = customerRepository;
        }

        [HttpPost("{id:int}/AssignTeam")]
        public IActionResult AssignTeam([FromServices] CustomerUserRepository customerUserRepository, [FromServices] TeamRepository teamRepository, [FromServices] TeamUserRepository teamUserRepository, [FromRoute] Int32 id, [FromBody] AssingingDeviceCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            CustomerUser customerUser = customerUserRepository.GetByPermission(CurrentUser.ID, CustomerMemberType.Owner);

            if (customerUser is null)
            {
                response.AddNotAllowedErr();
            }

            var team = teamRepository.Get(ww => ww.ID == command.TeamID.GetValueOrDefault() && ww.CustomerID == customerUser.CustomerID);
            if (team is null)
            {
                response.AddInvalidErr("TeamID");
            }
            Expression<Func<Device, Boolean>> query = ww => (ww.ID == id) && (ww.CustomerID == customerUser.CustomerID);
            var device = deviceRepository.Get(query);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }
            device.Team = team;
            deviceRepository.Update(device);
            response.SetMessage("Updated", "Đã cập nhật");

            return response.ToIActionResult();
        }



        [HttpGet]
        public IActionResult GetList([FromServices] CustomerUserRepository customerUserRepository, [FromServices] TeamUserRepository teamUserRepository, [FromQuery] PageCommand pageCommand, [FromQuery] String search, [FromQuery] String mode = "Small")
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            CustomerUser customerUser = customerUserRepository.GetByPermission(CurrentUser.ID);

            if (customerUser is null)
            {
                response.AddNotAllowedErr();
            }

            var teamIDs = teamUserRepository.GetListEntities(new PageCommand(), ww => ww.CustomerUserID == customerUser.ID).Select(ww => ww.TeamID).ToList();
            Expression<Func<Device, Boolean>> query = ww => (customerUser.Type == CustomerMemberType.Owner ? (ww.CustomerID == customerUser.CustomerID) : (teamIDs.Contains(ww.TeamID.GetValueOrDefault())))
            && (!String.IsNullOrWhiteSpace(search) ? ww.Name.ToLower().Contains(search.ToLower()) : true);
            if (mode == "Large")
            {

            }
            else if (mode == "Medium")
            {
                var listResponse = deviceRepository.GetListResponseView<MediumDeviceViewModel>(pageCommand, query);
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
            var teamIDs = teamUserRepository.GetListEntities(new PageCommand(), ww => ww.CustomerUserID == customerUser.ID).Select(ww => ww.TeamID).ToList();

            Expression<Func<Device, Boolean>> query = ww => (ww.ID == id)
             && (customerUser.Type == CustomerMemberType.Owner ? ww.CustomerID == customerUser.CustomerID : (teamIDs.Contains(ww.TeamID.GetValueOrDefault())));
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
    }
    public class AuthorizedDevicesController : AuthorizedDeviceAPIControllerBase
    {
        public AuthorizedDevicesController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }



        [HttpPost("me/TelemetryRecords")]
        public IActionResult CreateTelemetryRecord([FromServices] DeviceRepository deviceRepository, [FromServices] TelemetryRecordRepository telemetryRecordRepository, [FromBody] AddingTelemetryRecordCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = deviceRepository.Get(ww => ww.ID == CurrentDevice.ID);

            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }


            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);


            TelemetryRecord record = new TelemetryRecord
            {
                LocationPoint = geometryFactory.CreatePoint(new Coordinate(command.Longitude.GetValueOrDefault(), command.Latitude.GetValueOrDefault())),
                AdditionalInformation = command.AdditionalInformation,
                CreatedTime = DateTime.Now,
                DeviceID = device.ID,
                Direction = command.Direction.GetValueOrDefault(),

            };
            telemetryRecordRepository.Create(record);
            device.LastPoint = geometryFactory.CreatePoint(new Coordinate(command.Longitude.GetValueOrDefault(), command.Latitude.GetValueOrDefault()));
            device.LastDirection = command.Direction.GetValueOrDefault();
            device.LastAdditionalInformation = command.AdditionalInformation;
            deviceRepository.Update(device);

            response.SetCreatedObject(record);
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