

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
            CustomerUserPermission customerUserPermission = customerUserRepository.GetMemberPermission(CurrentUser, CustomerMemberType.Owner);

            if (customerUserPermission is null)
            {
                response.AddNotAllowedErr();
            }

            var team = teamRepository.Get(ww => ww.ID == command.TeamID.GetValueOrDefault() && ww.CustomerID == customerUserPermission.CustomerID);
            if (team is null)
            {
                response.AddInvalidErr("TeamID");
            }
            Expression<Func<Device, Boolean>> query = ww => (ww.ID == id) && (ww.CustomerID == customerUserPermission.CustomerID);
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

        // [HttpPost]
        // public IActionResult Create([FromServices] CustomerUserRepository customerUserRepository, [FromServices] TeamRepository teamRepository, [FromBody] AddingDeviceCommand command)
        // {
        //     ActionResponse response = actionResponseFactory.CreateInstance();
        //     CustomerUserPermission customerUserPermission = customerUserRepository.GetMemberPermission(CurrentUser, CustomerMemberType.Owner);

        //     if (customerUserPermission is null)
        //     {
        //         response.AddNotAllowedErr();
        //     }
        //     if (command.TeamID is not null)
        //     {
        //         var team = teamRepository.Get(ww => ww.ID == command.TeamID.GetValueOrDefault() && ww.CustomerID == customerUserPermission.CustomerID);
        //         if (team is null)
        //         {
        //             response.AddInvalidErr("TeamID");
        //         }
        //     }

        //     if (!deviceModelRepository.Any(ww => ww.ID == command.DeviceModelID))
        //     {
        //         response.AddInvalidErr("ModelDeviceID");
        //     }

        //     var device = new Device
        //     {
        //         Name = command.Name,
        //         CustomerID = customerUserPermission.CustomerID,
        //         TeamID = command.TeamID,
        //         DeviceModelID = command.DeviceModelID.Value,

        //     };
        //     deviceRepository.Create(device);
        //     response.SetCreatedObject(device);


        //     return response.ToIActionResult();

        // }

        [HttpGet]
        public IActionResult GetList([FromServices] CustomerUserRepository customerUserRepository, [FromServices] TeamUserRepository teamUserRepository, [FromQuery] PageCommand pageCommand, [FromQuery] String search, [FromQuery] String mode = "Small")
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            CustomerUserPermission customerUserPermission = customerUserRepository.GetMemberPermission(CurrentUser);

            if (customerUserPermission is null)
            {
                response.AddNotAllowedErr();
            }
            var teamIDs = teamUserRepository.GetListEntities(new PageCommand(), ww => ww.UserID == CurrentUser.ID).Select(ww => ww.TeamID).ToList();
            Expression<Func<Device, Boolean>> query = ww => (customerUserPermission.Type == CustomerMemberType.Owner ? (ww.CustomerID == customerUserPermission.CustomerID) : (teamIDs.Contains(ww.TeamID.GetValueOrDefault())))
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

            CustomerUserPermission customerUserPermission = customerUserRepository.GetMemberPermission(CurrentUser);
            if (customerUserPermission is null)
            {
                response.AddNotAllowedErr();
            }
            var teamIDs = teamUserRepository.GetListEntities(new PageCommand(), ww => ww.UserID == CurrentUser.ID).Select(ww => ww.TeamID).ToList();

            Expression<Func<Device, Boolean>> query = ww => (ww.ID == id)
             && (customerUserPermission.Type == CustomerMemberType.Owner ? ww.CustomerID == customerUserPermission.CustomerID : (teamIDs.Contains(ww.TeamID.GetValueOrDefault())));
            var device = deviceRepository.Get(query);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }

            response.SetData(ViewModelHelpers.ConvertToViewModel<Device, SuperSmallDeviceViewModel>(device));

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

            response.SetCreatedObject(stat);

            return response.ToIActionResult();
        }
    }
}