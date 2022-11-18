

using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
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
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using MiSmart.Infrastructure.Services;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using MiSmart.DAL.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using System.Data;
using MiSmart.API.Helpers;
using Microsoft.Extensions.Options;
using MiSmart.Infrastructure.Settings;
using MiSmart.API.Services;
using System.Net.Http;
using MiSmart.API.Settings;
using MiSmart.API.GrpcServices;

namespace MiSmart.API.Controllers
{
    public class DevicesController : AuthorizedAPIControllerBase
    {

        public DevicesController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {

        }



        [HttpPost("{id:int}/AssignExecutionCompany")]
        public async Task<IActionResult> AssignExecutionCompany([FromServices] CustomerUserRepository customerUserRepository, [FromServices] ExecutionCompanyRepository executionCompanyRepository,
        [FromServices] DeviceRepository deviceRepository,
         [FromServices] TeamUserRepository teamUserRepository, [FromRoute] Int32 id, [FromBody] AssigningDeviceExecutionCompanyCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var customerUser = await customerUserRepository.GetByPermissionAsync(CurrentUser.UUID);
            if (customerUser is null && !CurrentUser.IsAdministrator)
            {
                response.AddNotAllowedErr();
                return response.ToIActionResult();
            }

            var executionCompany = await executionCompanyRepository.GetAsync(ww => ww.ID == command.ExecutionCompanyID.GetValueOrDefault());
            if (executionCompany is null)
            {
                response.AddInvalidErr("ExecutionCompanyID");
                return response.ToIActionResult();
            }
            Expression<Func<Device, Boolean>> query = ww => (ww.ID == id);
            var device = await deviceRepository.GetAsync(query);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
                return response.ToIActionResult();
            }
            if (executionCompany.ID != device.ExecutionCompanyID)
            {
                device.ExecutionCompanyID = executionCompany.ID;
                device.TeamID = null;
                await deviceRepository.UpdateAsync(device);
            }
            response.SetUpdatedMessage();

            return response.ToIActionResult();
        }

        [HttpPost("{id:int}/AssignTeam")]
        public async Task<IActionResult> AssignTeam([FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromRoute] Int32 id,
        [FromServices] DeviceRepository deviceRepository,
        [FromServices] TeamRepository teamRepository, [FromBody] AssigningDeviceTeamCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.UUID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
                return response.ToIActionResult();
            }
            var device = await deviceRepository.GetAsync(ww => ww.ID == id && ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
                return response.ToIActionResult();
            }
            if (command.TeamID.HasValue)
            {
                var team = await teamRepository.GetAsync(ww => ww.ID == command.TeamID.GetValueOrDefault() && ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID);
                if (team is null)
                {
                    response.AddInvalidErr("TeamID");
                    return response.ToIActionResult();
                }
                device.Team = team;
            }
            else device.TeamID = null;
            await deviceRepository.UpdateAsync(device);
            response.SetUpdatedMessage();

            return response.ToIActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromServices] CustomerUserRepository customerUserRepository,
        [FromServices] DeviceRepository deviceRepository,
        [FromServices] BatteryGroupLogRepository batteryGroupLogRepository,
    [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
         [FromServices] TeamUserRepository teamUserRepository, [FromQuery] PageCommand pageCommand, [FromQuery] String? search,
         [FromQuery] String? relation = "Owner")
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            Expression<Func<Device, Boolean>> query = ww => false;
            if (relation == "Owner")
            {
                var customerUser = await customerUserRepository.GetByPermissionAsync(CurrentUser.UUID);

                if (customerUser is null)
                {
                    response.AddNotAllowedErr();
                    return response.ToIActionResult();
                }

                query = ww => (ww.CustomerID == customerUser.CustomerID)
                && (!String.IsNullOrWhiteSpace(search) ? (ww.Name ?? "").ToLower().Contains(search.ToLower()) : true);

            }
            else if (relation == "Administrator")
            {
                if (!CurrentUser.IsAdministrator)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => true && (!String.IsNullOrWhiteSpace(search) ? (ww.Name ?? "").ToLower().Contains(search.ToLower()) : true);
            }
            else if (relation == "Maintainer")
            {
                if (CurrentUser.RoleID != 3)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => true && (!String.IsNullOrWhiteSpace(search) ? (ww.Name ?? "").ToLower().Contains(search.ToLower()) : true);
            }
            else if (relation == "LogAnalyst")
            {
                if (CurrentUser.RoleID != 4)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => true && (!String.IsNullOrWhiteSpace(search) ? (ww.Name ?? "").ToLower().Contains(search.ToLower()) : true);
            }
            else
            {
                var executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.UUID);
                if (executionCompanyUser is null)
                {
                    response.AddNotAllowedErr();
                    return response.ToIActionResult();
                }
                List<Int64> teamIDs = teamUserRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.ExecutionCompanyUserID == executionCompanyUser.ID).Result.Select(ww => ww.TeamID).ToList();
                query = ww => (ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID)
                && (executionCompanyUser.Type == ExecutionCompanyUserType.Member ? (teamIDs.Contains(ww.TeamID.GetValueOrDefault())) : true)
                 && (!String.IsNullOrWhiteSpace(search) ? (ww.Name ?? "").ToLower().Contains(search.ToLower()) : true);
            }
            var listResponse = await deviceRepository.GetListResponseViewAsync<SmallDeviceViewModel>(pageCommand, query, ww => ww.Status);
            foreach (var item in listResponse.Data ?? new List<SmallDeviceViewModel>())
            {
                if (item.LastBatteryGroupIDs is not null)
                    if (item.LastBatteryGroupIDs.Count > 0)
                    {
                        item.BatteryGroupLogs = new List<BatteryGroupLogViewModel>();
                        foreach (var groupID in item.LastBatteryGroupIDs)
                        {
                            var group = await batteryGroupLogRepository.GetAsync(ww => ww.ID == groupID);
                            if (group is not null)
                            {
                                item.BatteryGroupLogs.Add(ViewModelHelpers.ConvertToViewModel<BatteryGroupLog, BatteryGroupLogViewModel>(group));
                            }
                        }
                    }
            }
            listResponse.SetResponse(response);
            return response.ToIActionResult();
        }
        [HttpGet("{id:int}/GetToken")]
        public async Task<IActionResult> GetToken([FromServices] CustomerUserRepository customerUserRepository, [FromServices] TeamUserRepository teamUserRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
         [FromServices] DeviceRepository deviceRepository, [FromRoute] Int32 id, [FromQuery] String? relation = "Owner")
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            Expression<Func<Device, Boolean>> query = ww => false;
            if (relation == "Owner")
            {

                var customerUser = await customerUserRepository.GetByPermissionAsync(CurrentUser.UUID);
                if (customerUser is null)
                {
                    response.AddNotAllowedErr();
                    return response.ToIActionResult();
                }


                query = ww => (ww.ID == id)
                   && (ww.CustomerID == customerUser.CustomerID);
            }
            else if (relation == "Administrator")
            {
                if (!CurrentUser.IsAdministrator)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => (ww.ID == id);
            }
            else if (relation == "Maintainer")
            {
                if (CurrentUser.RoleID != 3)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => (ww.ID == id);
            }
            else
            {
                var executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.UUID);
                if (executionCompanyUser is null)
                {
                    response.AddNotAllowedErr();
                    return response.ToIActionResult();
                }
                query = ww => (ww.ID == id) && (ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID);
            }
            var device = await deviceRepository.GetAsync(query);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
                return response.ToIActionResult();
            }

            response.SetData(ViewModelHelpers.ConvertToViewModel<Device, SuperSmallDeviceViewModel>(device));

            return response.ToIActionResult();
        }
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchDevice([FromServices] DeviceRepository deviceRepository, [FromServices] DeviceModelRepository deviceModelRepository, [FromRoute] Int32 id, [FromBody] PatchingDeviceCommand command,
        [FromServices] ExecutionCompanyRepository executionCompanyRepository, [FromServices] CustomerRepository customerRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3)
            {
                response.AddNotAllowedErr();
                return response.ToIActionResult();
            }
            var device = await deviceRepository.GetAsync(ww => ww.ID == id);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
                return response.ToIActionResult();
            }


            device.Name = String.IsNullOrWhiteSpace(command.Name) ? device.Name : command.Name;

            if (command.DeviceModelID.HasValue)
            {
                var deviceModel = await deviceModelRepository.GetAsync(ww => ww.ID == command.DeviceModelID.GetValueOrDefault());
                if (deviceModel is null)
                {
                    response.AddInvalidErr("DeviceModelID");
                    return response.ToIActionResult();
                }
                device.DeviceModel = deviceModel;
            }

            if (command.ExecutionCompanyID.HasValue)
            {
                var executionCompany = await executionCompanyRepository.GetAsync(ww => ww.ID == command.ExecutionCompanyID.GetValueOrDefault());
                if (executionCompany is null)
                {
                    response.AddInvalidErr("ExecutionCompanyID");
                    return response.ToIActionResult();
                }
                device.ExecutionCompany = executionCompany;
            }
            if (command.CustomerID.HasValue)
            {
                var customer = await customerRepository.GetAsync(ww => ww.ID == command.CustomerID.GetValueOrDefault());
                if (customer is null)
                {
                    response.AddInvalidErr("CustomerID");
                    return response.ToIActionResult();
                }
                device.Customer = customer;
            }


            await deviceRepository.UpdateAsync(device);

            response.SetUpdatedMessage();

            return response.ToIActionResult();
        }
        [HttpPost("UploadOfflineStats")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadOfflineStats([FromBody] AddingBulkOfflineFlightStatsCommand command,
        [FromServices] FlightStatRepository flightStatRepository,
        [FromServices] DatabaseContext databaseContext,
        [FromServices] ExecutionCompanySettingRepository executionCompanySettingRepository,
        [FromServices] ExecutionCompanyUserFlightStatRepository executionCompanyUserFlightStatRepository,
        [FromServices] MyEmailService emailService,
        [FromServices] BatteryRepository batteryRepository,
        [FromServices] IOptions<FrontEndSettings> options,
        [FromServices] IHttpClientFactory httpClientFactory,
         [FromServices] DeviceRepository deviceRepository, [FromServices] JWTService jwtService)
        {
            var response = actionResponseFactory.CreateInstance();
            List<FlightStat> flightStats = new List<FlightStat>();
            var sendNotification = false;
            var taskAreas = new List<Double>();
            for (var i = 0; i < command.Data.Count - 1; i++)
            {
                var item = command.Data[i];
                var item1 = command.Data[i + 1];

                if (item.TaskArea == item1.TaskArea && item.FlightTime == item1.FlightTime && item.FlightDuration == item1.FlightDuration)
                {
                    sendNotification = true;
                    taskAreas.Add(item.TaskArea.GetValueOrDefault());
                }
            }
            if (sendNotification)
            {
                await emailService.SendMailAsync(new String[] { "huynhthehainam@gmail.com" }, new String[] { }, new String[] { }, "[Duplicate] Report flight stat ", @$"
                               count: {command.Data.Count}
                               taskArea: {String.Join(",", taskAreas)}
                               offline
                            ");
            }
            foreach (var item in command.Data)
            {
                var deviceJWT = jwtService.GetUser(item.DeviceAccessToken ?? "");
                if (deviceJWT != null)
                {
                    if (deviceJWT.Type == "Device")
                    {

                        var device = await deviceRepository.GetAsync(ww => ww.ID == deviceJWT.ID);
                        if (device is not null)
                        {
                            device.LastOnline = DateTime.UtcNow;
                            await deviceRepository.UpdateAsync(device);
                            if (!Constants.AllowedVersions.Contains(item.GCSVersion ?? ""))
                            {
                                continue;
                            }
                            if (item.FlywayPoints.Count < 2)
                            {
                                if (item.TaskArea.GetValueOrDefault() < 0)
                                {
                                    await emailService.SendMailAsync(new String[] { "huynhthehainam@gmail.com" }, new String[] { }, new String[] { }, "Report flight stat", @$"
                                task area: {item.TaskArea},
                                sprayedIndexes: {item.SprayedIndexes.Count()}
                                flywayPoints: {item.FlywayPoints.Count()}
                                device: {device.Name}
                                flightDuration: {item.FlightDuration.GetValueOrDefault()}
                                flightTime: {item.FlightTime}
                                offline
                            ");
                                }
                                continue;
                            }
                            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                            if (item.SprayedIndexes.Count > 0 && item.TaskArea.GetValueOrDefault() <= 0)
                            {
                                if (item.TaskArea.GetValueOrDefault() < 0)
                                {
                                    await emailService.SendMailAsync(new String[] { "huynhthehainam@gmail.com" }, new String[] { }, new String[] { }, "Report flight stat", @$"
                                task area: {item.TaskArea},
                                sprayedIndexes: {item.SprayedIndexes.Count()}
                                flywayPoints: {item.FlywayPoints.Count()}
                                device: {device.Name}
                                flightDuration: {item.FlightDuration.GetValueOrDefault()}
                                flightTime: {item.FlightTime}
                                offline
                            ");
                                }
                                var taskArea = 0.0;
                                for (var i = 0; i < item.FlywayPoints.Count - 1; i++)
                                {
                                    if (item.SprayedIndexes.Contains(i))
                                    {
                                        var firstLng = item.FlywayPoints[i].Longitude.GetValueOrDefault();
                                        var firstLat = item.FlywayPoints[i].Latitude.GetValueOrDefault();
                                        var secondLng = item.FlywayPoints[i + 1].Longitude.GetValueOrDefault();
                                        var secondLat = item.FlywayPoints[i + 1].Latitude.GetValueOrDefault();
                                        var point1 = geometryFactory.CreatePoint(new Coordinate(firstLng, firstLat));
                                        var point2 = geometryFactory.CreatePoint(new Coordinate(secondLng, secondLat));
                                        var distance = 0.0;

                                        using (var databaseCommand = databaseContext.Database.GetDbConnection().CreateCommand())
                                        {
                                            databaseCommand.CommandText = @$"select ST_Distance(st_transform( st_geomfromtext ('point({firstLng} {firstLat})', 4326), 3857 ),
st_transform(st_geomfromtext ('point({secondLng} {secondLat})',4326) , 3857)) * cosd({firstLat})
";
                                            databaseCommand.CommandType = CommandType.Text;
                                            databaseContext.Database.OpenConnection();
                                            using (var result = databaseCommand.ExecuteReader())
                                            {
                                                while (result.Read())
                                                {
                                                    var parsed = Double.TryParse(result[0].ToString(), out distance);
                                                    break;
                                                }
                                            }
                                        }
                                        taskArea += distance * 6; // Mr Dat confirmed
                                    }
                                }
                                item.TaskArea = taskArea;
                            }



                            var stat = new FlightStat
                            {
                                FlightDuration = item.FlightDuration.GetValueOrDefault(),
                                FieldName = item.FieldName,
                                FlightUID = item.FlightUID,
                                Flights = item.Flights.GetValueOrDefault(),
                                FlightTime = item.FlightTime ?? DateTime.UtcNow,
                                FlywayPoints = geometryFactory.CreateLineString(item.FlywayPoints.Select(ww => new Coordinate(ww.Longitude.GetValueOrDefault(), ww.Latitude.GetValueOrDefault())).ToArray()),
                                SprayedIndexes = item.SprayedIndexes,
                                PilotName = item.PilotName,
                                CreatedTime = DateTime.UtcNow,
                                CustomerID = device.CustomerID,
                                Device = device,
                                DeviceName = device.Name,
                                TaskLocation = item.TaskLocation,
                                TeamID = device.TeamID,
                                TaskArea = item.TaskArea.GetValueOrDefault(),
                                ExecutionCompanyID = device.ExecutionCompanyID,
                                GCSVersion = item.GCSVersion,
                                AdditionalInformation = item.AdditionalInformation,
                                BatteryPercentRemaining = item.BatteryPercentRemaining,
                                IsOnline = false,
                            };
                            if (!String.IsNullOrEmpty(item.BatterySerialNumber))
                            {
                                var battery = await batteryRepository.GetOrCreateBySerialNumberAsync(item.BatterySerialNumber);
                                stat.Battery = battery;
                                stat.CycleCount = item.BatteryCycleCount.GetValueOrDefault();
                            }
                            try
                            {
                                stat.TaskLocation = await BingLocationHelper.UpdateFlightStatLocation(stat, httpClientFactory);
                                stat.IsBingLocation = true;
                            }
                            catch (Exception)
                            {

                            }

                            if (device.ExecutionCompanyID.HasValue)
                            {
                                var latestSetting = executionCompanySettingRepository.GetLatestSetting(device.ExecutionCompanyID.GetValueOrDefault());
                                if (latestSetting is not null)
                                {
                                    stat.Cost = stat.TaskArea / 10000 * latestSetting.CostPerHectare;
                                }
                            }


                            stat = await flightStatRepository.CreateAsync(stat);
                            if (device.Team is not null)
                            {
                                var team = device.Team;
                                List<TeamUser>? teamUsers = team?.TeamUsers?.ToList();
                                if (teamUsers != null)
                                {
                                    var executionCompanyUserFlightStats = await executionCompanyUserFlightStatRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.FlightStatID == stat.ID);
                                    await executionCompanyUserFlightStatRepository.DeleteRangeAsync(executionCompanyUserFlightStats);

                                    foreach (var teamUser in teamUsers)
                                    {
                                        ExecutionCompanyUserFlightStat executionCompanyUserFlightStat = await
                                        executionCompanyUserFlightStatRepository.CreateAsync(new ExecutionCompanyUserFlightStat
                                        {
                                            ExecutionCompanyUserID = teamUser.ExecutionCompanyUserID,
                                            FlightStatID = stat.ID,
                                            Type = teamUser.Type,
                                        });
                                    }
                                }
                            }
                            await deviceRepository.UpdateAsync(device);
                            if (stat.BatteryPercentRemaining.HasValue)
                            {
                                if (stat.BatteryPercentRemaining.GetValueOrDefault() < 30)
                                {
                                    await emailService.SendLowBatteryReportAsync(stat, false);
                                }
                            }
                            flightStats.Add(stat);
                        }
                    }

                }
            }
            response.SetData(flightStats.Select(fs => new { ID = fs.ID }));
            return response.ToIActionResult();
        }
        [HttpPost("DevicesFromTM")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDevicesFromTM([FromServices] DeviceRepository deviceRepository, [FromServices] AuthGrpcClientService authGrpcClientService,
        [FromBody] GetDeviceFromTMCommand command, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
        [FromServices] CustomerUserRepository customerUserRepository,
        [FromQuery] String? relation = "Pilot")
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            var resp = authGrpcClientService.GetUserExistingInformation(command.EncryptedUUID ?? "");
            if (resp.IsExist)
            {
                try
                {
                    var uuid = Guid.Parse(resp.DecryptedUUID);
                    Expression<Func<Device, bool>> query = ww => false;
                    if (relation == "Pilot")
                    {
                        ExecutionCompanyUser? executionCompanyUser = await executionCompanyUserRepository.GetAsync(ww => ww.UserUUID == uuid);
                        if (executionCompanyUser == null)
                        {
                            response.AddInvalidErr("EncryptedUUID");
                            return response.ToIActionResult();
                        }
                        query = ww => true;
                    }
                    else if (relation == "Owner")
                    {
                        CustomerUser? customerUser = await customerUserRepository.GetAsync(ww => ww.UserUUID == uuid);
                        if (customerUser == null)
                        {
                            response.AddInvalidErr("EncryptedUUID");
                            return response.ToIActionResult();
                        }
                        query = ww => ww.CustomerID == customerUser.CustomerID;
                    }
                    var listDevice = await deviceRepository.GetListResponseViewAsync<SmallDeviceViewModel>(new PageCommand(), query);
                    listDevice.SetResponse(response);

                }
                catch (Exception)
                {
                    response.AddInvalidErr("EncryptedUUID");
                    return response.ToIActionResult();
                }
            }
            else
            {
                response.AddInvalidErr("EncryptedUUID");
                return response.ToIActionResult();
            }
            return response.ToIActionResult();
        }
        [HttpGet("GetDevicesFromTM")]
        [AllowAnonymous]
        public async Task<IActionResult> GetListDeviceFromTM([FromQuery] PageCommand pageCommand,
        [FromServices] DeviceRepository deviceRepository, [FromQuery] String? search, [FromQuery] String? secretKey,
        [FromServices] IOptions<FarmAppSettings> options, [FromQuery] Guid? deviceUUID)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            Expression<Func<Device, Boolean>> query = ww => (!String.IsNullOrWhiteSpace(search) ? (ww.Name ?? "").ToLower().Contains(search.ToLower()) : true)
                                            && (deviceUUID.HasValue ? ww.UUID == deviceUUID.GetValueOrDefault() : true);
            var settings = options.Value;
            if (secretKey != settings.SecretKey)
            {
                response.AddAuthorizationErr();
                return response.ToIActionResult();
            }
            var listResponse = await deviceRepository.GetListResponseViewAsync<SmallDeviceFromTMViewModel>(pageCommand, query);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }
    }
    public class AuthorizedDevicesController : AuthorizedDeviceAPIControllerBase
    {
        public AuthorizedDevicesController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }



        [HttpPost("me/TelemetryRecords")]
        public async Task<IActionResult> CreateTelemetryRecord([FromServices] DeviceRepository deviceRepository,
        [FromServices] BatteryGroupLogRepository batteryGroupLogRepository,
        [FromServices] BatteryModelRepository batteryModelRepository,
        [FromServices] MyEmailService emailService,
        [FromServices] CountingService countingService,
        [FromServices] BatteryRepository batteryRepository, [FromServices] TelemetryGroupRepository telemetryGroupRepository, [FromBody] AddingBulkTelemetryRecordCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = await deviceRepository.GetAsync(ww => ww.ID == CurrentDevice.ID);

            if (device is null)
            {
                response.AddNotFoundErr("Device");
                return response.ToIActionResult();
            }
            if (command.Data == null || command.Data.Count == 0)
            {
                countingService.Count += 1;
                response.AddInvalidErr("Data");
                return response.ToIActionResult();
            }

            if (device.ID == 8)
            {
                countingService.Count2 += 1;
            }
            else if (device.ID == 28)
            {
                countingService.Count3 += 1;
            }
            else if (device.ID == 29)
            {
                countingService.Count4 += 1;
            }
            else if (device.ID == 42)
            {
                countingService.Count5 += 1;
            }
            else if (device.ID == 43)
            {
                countingService.Count6 += 1;
            }


            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            TimeSpan span = new TimeSpan(0, 0, 0, 0, 5000);
            TimeSpan eachSpan = span / command.Data.Count;
            var records = new List<TelemetryRecord>();
            var startedTime = DateTime.UtcNow - span;
            for (Int32 i = 0; i < command.Data.Count; i++)
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

            group = await telemetryGroupRepository.CreateAsync(group);

            device.LastGroupID = group.ID;
            if (device.Status != DeviceStatus.Active)
                device.Status = DeviceStatus.Active;

            await deviceRepository.UpdateAsync(device);

            var batteryLogGroups = command.BatteryLogs.GroupBy(bl => bl.ActualID);
            List<Guid> lastGroupIDs = new List<Guid>();
            foreach (var batteryGroup in batteryLogGroups)
            {
                var key = batteryGroup.Key;
                if (key != "TestBattery")
                {
                    var battery = await batteryRepository.GetAsync(b => b.ActualID == key);
                    if (battery is null)
                    {
                        var batteryModel = await batteryModelRepository.GetAsync(ww => true);
                        if (batteryModel is not null)
                        {
                            battery = new Battery { ActualID = key, BatteryModelID = batteryModel.ID };
                            battery = await batteryRepository.CreateAsync(battery);
                            await emailService.SendMailAsync(new String[] { "huynhthehainam@gmail.com" }, new String[] { }, new String[] { }, "New battery", $"Battery name {battery.ActualID} is just registered.");
                        }
                    }
                    if (battery is not null)
                    {

                        TimeSpan span1 = new TimeSpan(0, 0, 0, 0, 5000);
                        TimeSpan eachSpan1 = span1 / batteryGroup.Count();
                        var startedTime1 = DateTime.UtcNow - span1;
                        var actualGroup = batteryGroup.ToList();
                        var logs = new List<BatteryLog>();
                        for (var i = 0; i < actualGroup.Count(); i++)
                        {
                            logs.Add(new BatteryLog
                            {
                                CellMaximumVoltage = actualGroup[i].CellMaximumVoltage,
                                CellMaximumVoltageUnit = actualGroup[i].CellMaximumVoltageUnit,
                                CellMinimumVoltage = actualGroup[i].CellMinimumVoltage,
                                CellMinimumVoltageUnit = actualGroup[i].CellMinimumVoltageUnit,
                                CreatedTime = startedTime1.Add(eachSpan1 * i),
                                Current = actualGroup[i].Current,
                                CurrentUnit = actualGroup[i].CurrentUnit,
                                CycleCount = actualGroup[i].CycleCount,
                                PercentRemaining = actualGroup[i].PercentRemaining,
                                Temperature = actualGroup[i].Temperature,
                                TemperatureUnit = actualGroup[i].TemperatureUnit,
                            });
                        }
                        var groupLog = new BatteryGroupLog
                        {
                            Battery = battery,
                            CreatedTime = DateTime.UtcNow,
                            Logs = logs,
                        };
                        await batteryGroupLogRepository.CreateAsync(groupLog);
                        battery.LastGroup = groupLog;
                        await batteryRepository.UpdateAsync(battery);
                        lastGroupIDs.Add(groupLog.ID);
                    }
                }

            }

            device.LastBatterGroupLogs = lastGroupIDs;
            device.LastOnline = DateTime.UtcNow;

            await deviceRepository.UpdateAsync(device);


            response.SetCreatedObject(group);
            return response.ToIActionResult();
        }

        [HttpPost("me/FlightStats")]
        public async Task<IActionResult> CreateFlightStat([FromServices] DeviceRepository deviceRepository, [FromServices] FlightStatRepository flightStatRepository,
        [FromServices] ExecutionCompanySettingRepository executionCompanySettingRepository,
        [FromServices] DatabaseContext databaseContext,
        [FromServices] MyEmailService emailService,
        [FromServices] IOptions<FrontEndSettings> options,
        [FromServices] BatteryRepository batteryRepository,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] ExecutionCompanyUserFlightStatRepository executionCompanyUserFlightStatRepository, [FromBody] AddingFlightStatCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = await deviceRepository.GetAsync(ww => ww.ID == CurrentDevice.ID);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
                return response.ToIActionResult();
            }
            device.LastOnline = DateTime.UtcNow;
            await deviceRepository.UpdateAsync(device);
            if (!Constants.AllowedVersions.Contains(command.GCSVersion ?? ""))
            {
                response.SetMessage("Invalid");
                return response.ToIActionResult();
            }
            if (command.FlywayPoints.Count < 2)
            {
                response.SetMessage("Invalid");
                await emailService.SendMailAsync(new String[] { "huynhthehainam@gmail.com" }, new String[] { }, new String[] { }, "Report flight stat", @$"
                                task area: {command.TaskArea},
                                sprayedIndexes: {command.SprayedIndexes.Count()}
                                device: {device.Name}
                                flightDuration: {command.FlightDuration.GetValueOrDefault()}
                                flywayPoints: {command.FlywayPoints.Count()}
                                flightTime: {command.FlightTime}
                                online
                            ");
                return response.ToIActionResult();
            }

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            if (command.SprayedIndexes.Count > 0 && command.TaskArea.GetValueOrDefault() <= 0)
            {
                if (command.TaskArea.GetValueOrDefault() < 0)
                {
                    await emailService.SendMailAsync(new String[] { "huynhthehainam@gmail.com" }, new String[] { }, new String[] { }, "Report flight stat", @$"
                                task area: {command.TaskArea},
                                sprayedIndexes: {command.SprayedIndexes.Count()}
                                device: {device.Name}
                                flightDuration: {command.FlightDuration.GetValueOrDefault()}
                                flywayPoints: {command.FlywayPoints.Count()}
                                flightTime: {command.FlightTime}
                                online
                            ");
                }
                var taskArea = 0.0;
                for (var i = 0; i < command.FlywayPoints.Count - 1; i++)
                {
                    if (command.SprayedIndexes.Contains(i))
                    {
                        var firstLng = command.FlywayPoints[i].Longitude.GetValueOrDefault();
                        var firstLat = command.FlywayPoints[i].Latitude.GetValueOrDefault();
                        var secondLng = command.FlywayPoints[i + 1].Longitude.GetValueOrDefault();
                        var secondLat = command.FlywayPoints[i + 1].Latitude.GetValueOrDefault();
                        var point1 = geometryFactory.CreatePoint(new Coordinate(firstLng, firstLat));
                        var point2 = geometryFactory.CreatePoint(new Coordinate(secondLng, secondLat));
                        var distance = 0.0;

                        using (var databaseCommand = databaseContext.Database.GetDbConnection().CreateCommand())
                        {
                            databaseCommand.CommandText = @$"select ST_Distance(st_transform( st_geomfromtext ('point({firstLng} {firstLat})', 4326), 3857 ),
st_transform(st_geomfromtext ('point({secondLng} {secondLat})',4326) , 3857)) * cosd({firstLat})
";
                            databaseCommand.CommandType = CommandType.Text;
                            databaseContext.Database.OpenConnection();
                            using (var result = databaseCommand.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    var parsed = Double.TryParse(result[0].ToString(), out distance);
                                    break;
                                }
                            }
                        }
                        taskArea += distance * 6; // Mr Dat confirmed
                    }
                }
                command.TaskArea = taskArea;
            }
            else if (command.TaskArea.GetValueOrDefault() < 0)
            {
                await emailService.SendMailAsync(new String[] { "huynhthehainam@gmail.com" }, new String[] { }, new String[] { }, "Report flight stat", @$"
                                task area: {command.TaskArea},
                                sprayedIndexes: {command.SprayedIndexes.ToString()}
                                device: {device.Name}
                                flightDuration: {command.FlightDuration.GetValueOrDefault()}
                            ");
            }



            var stat = new FlightStat
            {
                FlightDuration = command.FlightDuration.GetValueOrDefault(),
                FieldName = command.FieldName,
                FlightUID = command.FlightUID,
                Flights = command.Flights.GetValueOrDefault(),
                FlightTime = command.FlightTime ?? DateTime.UtcNow,
                FlywayPoints = geometryFactory.CreateLineString(command.FlywayPoints.Select(ww => new Coordinate(ww.Longitude.GetValueOrDefault(), ww.Latitude.GetValueOrDefault())).ToArray()),
                SprayedIndexes = command.SprayedIndexes,
                PilotName = command.PilotName,
                CreatedTime = DateTime.UtcNow,
                CustomerID = device.CustomerID,
                Device = device,
                DeviceName = device.Name,
                TaskLocation = command.TaskLocation,
                TaskArea = command.TaskArea.GetValueOrDefault(),
                TeamID = device.TeamID,
                ExecutionCompanyID = device.ExecutionCompanyID,
                GCSVersion = command.GCSVersion,
                AdditionalInformation = command.AdditionalInformation,
                BatteryPercentRemaining = command.BatteryPercentRemaining,
                IsOnline = true,
            };
            if (!String.IsNullOrEmpty(command.BatterySerialNumber))
            {
                var battery = await batteryRepository.GetOrCreateBySerialNumberAsync(command.BatterySerialNumber);
                stat.Battery = battery;
                stat.CycleCount = command.BatteryCycleCount.GetValueOrDefault();
            }
            if (device.ExecutionCompanyID.HasValue)
            {
                var latestSetting = executionCompanySettingRepository.GetLatestSetting(device.ExecutionCompanyID.GetValueOrDefault());
                if (latestSetting is not null)
                {
                    stat.Cost = stat.TaskArea / 10000 * latestSetting.CostPerHectare;
                }
            }
            try
            {
                stat.TaskLocation = await BingLocationHelper.UpdateFlightStatLocation(stat, httpClientFactory);
                stat.IsBingLocation = true;
            }
            catch (Exception)
            {

            }

            await flightStatRepository.CreateAsync(stat);
            if (device.Team is not null)
            {
                var team = device.Team;
                List<TeamUser>? teamUsers = team?.TeamUsers?.ToList();
                if (teamUsers != null)
                {
                    var executionCompanyUserFlightStats = await executionCompanyUserFlightStatRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.FlightStatID == stat.ID);
                    await executionCompanyUserFlightStatRepository.DeleteRangeAsync(executionCompanyUserFlightStats);

                    foreach (var teamUser in teamUsers)
                    {
                        ExecutionCompanyUserFlightStat executionCompanyUserFlightStat = new ExecutionCompanyUserFlightStat
                        {
                            ExecutionCompanyUserID = teamUser.ExecutionCompanyUserID,
                            FlightStatID = stat.ID,
                            Type = teamUser.Type,
                        };
                        await executionCompanyUserFlightStatRepository.CreateAsync(executionCompanyUserFlightStat);
                    }
                }
            }
            await deviceRepository.UpdateAsync(device);
            if (stat.BatteryPercentRemaining.HasValue)
            {
                if (stat.BatteryPercentRemaining.GetValueOrDefault() < 30)
                {
                    await emailService.SendLowBatteryReportAsync(stat, true);
                }
            }
            response.SetCreatedObject(stat);

            return response.ToIActionResult();
        }
        [HttpPost("me/StreamingLinks")]

        public async Task<IActionResult> CreateStreamingLink([FromServices] DeviceRepository deviceRepository, [FromBody] AddingStreamingLinkCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = await deviceRepository.GetAsync(ww => ww.ID == CurrentDevice.ID);

            if (device is null)
            {
                response.AddNotFoundErr("Device");
                return response.ToIActionResult();
            }

            StreamingLink streamingLink = new StreamingLink()
            {
                Link = command.Link
            };



            device.StreamingLinks?.Add(streamingLink);


            await deviceRepository.UpdateAsync(device);

            response.SetCreatedObject(streamingLink);
            var result = await FirebaseMessaging.DefaultInstance.SendAsync(new Message() { Topic = "dronehub_message", Data = new Dictionary<String, String>() { { "update", "streaming_links" }, } });
            return response.ToIActionResult();
        }

        [HttpPost("me/RemoveAllStreamingLinks")]
        public async Task<IActionResult> RemoveAllStreamingLinks([FromServices] DeviceRepository deviceRepository, [FromServices] StreamingLinkRepository streamingLinkRepository)
        {
            var response = actionResponseFactory.CreateInstance();

            var device = await deviceRepository.GetAsync(ww => ww.ID == CurrentDevice.ID);

            if (device is null)
            {
                response.AddNotFoundErr("Device");
                return response.ToIActionResult();
            }



            if (device.StreamingLinks != null)
            {
                await streamingLinkRepository.DeleteRangeAsync(device.StreamingLinks.ToList());
                response.SetUpdatedMessage();

                var result = await FirebaseMessaging.DefaultInstance.SendAsync(new Message() { Topic = "dronehub_message", Data = new Dictionary<String, String>() { { "update", "streaming_links" }, } });
            }
            return response.ToIActionResult();
        }

        [HttpPost("me/Plans")]
        public async Task<IActionResult> CreatePlan([FromServices] DeviceRepository deviceRepository, [FromServices] PlanRepository planRepository, [FromForm] AddingPlanCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = await deviceRepository.GetAsync(ww => ww.ID == CurrentDevice.ID);

            if (device is null)
            {
                response.AddNotFoundErr("Device");
                return response.ToIActionResult();
            }
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var plan = await planRepository.GetAsync(ww => ww.FileName == (command.File == null ? "" : command.File.FileName) && ww.Device == device);
            if (plan is null)
            {
                plan = new Plan { FileName = (command.File == null ? "" : command.File.FileName), Device = device };
            }

            plan.Location = geometryFactory.CreatePoint(new Coordinate(command.Longitude.GetValueOrDefault(), command.Latitude.GetValueOrDefault()));
            plan.FileName = (command.File == null ? "" : command.File.FileName);
            plan.Area = command.Area.GetValueOrDefault();
            plan.FileBytes = command.GetFileBytes();

            if (plan.ID == 0)
            {
                await planRepository.CreateAsync(plan);
            }
            else
            {
                await planRepository.UpdateAsync(plan);
            }
            response.SetCreatedObject(plan);

            return response.ToIActionResult();
        }
        [HttpGet("RetrievePlans")]
        public async Task<IActionResult> GetPlans([FromServices] PlanRepository planRepository,
        [FromServices] DatabaseContext databaseContext, [FromServices] DeviceRepository deviceRepository,
        [FromQuery] PageCommand pageCommand, [FromQuery] String? search, [FromQuery] Double? latitude, [FromQuery] Double? longitude, [FromQuery] Double? range = 5000)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = await deviceRepository.GetAsync(ww => ww.ID == CurrentDevice.ID);

            if (device is null)
            {
                response.AddNotFoundErr("Device");
                return response.ToIActionResult();
            }

            Point? centerLocation = null;
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            if (latitude.HasValue && longitude.HasValue && range.HasValue)
            {
                centerLocation = geometryFactory.CreatePoint(new Coordinate(longitude.GetValueOrDefault(), latitude.GetValueOrDefault()));
            }
            Expression<Func<Plan, Boolean>> query = ww => (ww.Device == null || ww.Device.ExecutionCompanyID == device.ExecutionCompanyID)
            && (String.IsNullOrWhiteSpace(search) ?
            ((centerLocation != null) ? ((ww.Location != null ? ww.Location.Distance(centerLocation) < range.GetValueOrDefault() : false)) : true)
            : (ww.FileName ?? "").ToLower().Contains(search.ToLower()));
            var listResponse = await planRepository.GetListResponseViewAsync<SmallPlanViewModel>(pageCommand, query, ww => (centerLocation != null && ww.Location != null) ? ww.Location.Distance(centerLocation) : ww.CreatedTime, true);
            if (centerLocation is not null)
            {
                foreach (var item in listResponse.Data ?? new List<SmallPlanViewModel>())
                {
                    if (item.Point != null)
                        item.Distance = DistanceHelper.CalculateDistance(databaseContext, item.Point, centerLocation);
                }
            }
            listResponse.SetResponse(response);

            return response.ToIActionResult();

        }

        [HttpPost("RetrievePlanFile")]
        public async Task<IActionResult> GetFile([FromServices] PlanRepository planRepository, [FromServices] DeviceRepository deviceRepository, [FromBody] RetrievingPlanFileCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = await deviceRepository.GetAsync(ww => ww.ID == CurrentDevice.ID);

            if (device is null)
            {
                response.AddNotFoundErr("Device");
                return response.ToIActionResult();
            }
            Expression<Func<Plan, Boolean>> query = ww => (ww.Device == null || ww.Device.ExecutionCompanyID == device.ExecutionCompanyID)
                       && (ww.ID == command.PlanID);
            var plan = await planRepository.GetAsync(query);
            if (plan is null)
            {
                response.AddNotFoundErr("Plan");
                return response.ToIActionResult();
            }

            response.SetFile(plan.FileBytes ?? new Byte[0], "application/octet-stream", plan.FileName ?? "example.plan");



            return response.ToIActionResult();
        }



    }
}