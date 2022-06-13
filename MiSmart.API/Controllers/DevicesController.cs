

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
using MiSmart.Infrastructure.Permissions;
using MiSmart.API.Permissions;
using System.Collections.Generic;
using MiSmart.Infrastructure.Minio;
using Microsoft.AspNetCore.Authorization;
using MiSmart.Infrastructure.Services;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using MiSmart.DAL.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using System.Data;
using MiSmart.API.Helpers;

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
            CustomerUser customerUser = await customerUserRepository.GetByPermissionAsync(CurrentUser.UUID);
            if (customerUser is null)
            {
                response.AddNotAllowedErr();
            }

            var executionCompany = await executionCompanyRepository.GetAsync(ww => ww.ID == command.ExecutionCompanyID.GetValueOrDefault());
            if (executionCompany is null)
            {
                response.AddInvalidErr("ExecutionCompanyID");
            }
            Expression<Func<Device, Boolean>> query = ww => (ww.ID == id) && (ww.CustomerID == customerUser.CustomerID);
            var device = await deviceRepository.GetAsync(query);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
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
            ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.UUID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }
            var device = await deviceRepository.GetAsync(ww => ww.ID == id && ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }

            var team = await teamRepository.GetAsync(ww => ww.ID == command.TeamID.GetValueOrDefault() && ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID);
            if (team is null)
            {
                response.AddInvalidErr("TeamID");
            }

            device.Team = team;
            await deviceRepository.UpdateAsync(device);
            response.SetUpdatedMessage();

            return response.ToIActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromServices] CustomerUserRepository customerUserRepository,
        [FromServices] DeviceRepository deviceRepository,
        [FromServices] BatteryGroupLogRepository batteryGroupLogRepository,
    [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
         [FromServices] TeamUserRepository teamUserRepository, [FromQuery] PageCommand pageCommand, [FromQuery] String search,
         [FromQuery] String relation = "Owner")
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            Expression<Func<Device, Boolean>> query = ww => false;
            if (relation == "Owner")
            {
                CustomerUser customerUser = await customerUserRepository.GetByPermissionAsync(CurrentUser.UUID);

                if (customerUser is null)
                {
                    response.AddNotAllowedErr();
                }

                query = ww => (ww.CustomerID == customerUser.CustomerID)
                && (!String.IsNullOrWhiteSpace(search) ? ww.Name.ToLower().Contains(search.ToLower()) : true);

            }
            else if (relation == "Administrator")
            {
                if (!CurrentUser.IsAdministrator)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => true && (!String.IsNullOrWhiteSpace(search) ? ww.Name.ToLower().Contains(search.ToLower()) : true);
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
                && (executionCompanyUser.Type == ExecutionCompanyUserType.Member ? (teamIDs.Contains(ww.TeamID.GetValueOrDefault())) : true)
                 && (!String.IsNullOrWhiteSpace(search) ? ww.Name.ToLower().Contains(search.ToLower()) : true);
            }
            var listResponse = await deviceRepository.GetListResponseViewAsync<SmallDeviceViewModel>(pageCommand, query);
            foreach (var item in listResponse.Data)
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
         [FromServices] DeviceRepository deviceRepository, [FromRoute] Int32 id, [FromQuery] String relation = "Owner")
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            Expression<Func<Device, Boolean>> query = ww => false;
            if (relation == "Owner")
            {

                CustomerUser customerUser = await customerUserRepository.GetByPermissionAsync(CurrentUser.UUID);
                if (customerUser is null)
                {
                    response.AddNotAllowedErr();
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
            else
            {
                ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.UUID);
                if (executionCompanyUser is null)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => (ww.ID == id) && (ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID);
            }
            var device = await deviceRepository.GetAsync(query);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }

            response.SetData(ViewModelHelpers.ConvertToViewModel<Device, SuperSmallDeviceViewModel>(device));

            return response.ToIActionResult();
        }
        [HttpPatch("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> PatchDevice([FromServices] DeviceRepository deviceRepository, [FromServices] DeviceModelRepository deviceModelRepository, [FromRoute] Int32 id, [FromBody] PatchingDeviceCommand command)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();

            var device = await deviceRepository.GetAsync(ww => ww.ID == id);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }


            device.Name = String.IsNullOrWhiteSpace(command.Name) ? device.Name : command.Name;

            if (command.DeviceModelID.HasValue)
            {
                var deviceModel = await deviceModelRepository.GetAsync(ww => ww.ID == command.DeviceModelID.GetValueOrDefault());
                if (deviceModel is null)
                {
                    response.AddInvalidErr("DeviceModelID");
                }
                device.DeviceModel = deviceModel;
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
        [FromServices] EmailService emailService,
         [FromServices] DeviceRepository deviceRepository, [FromServices] JWTService jwtService)
        {
            var response = actionResponseFactory.CreateInstance();
            List<FlightStat> flightStats = new List<FlightStat>();
            foreach (var item in command.Data)
            {
                var deviceJWT = jwtService.GetUser(item.DeviceAccessToken);
                if (deviceJWT.Type == "Device")
                {

                    var device = await deviceRepository.GetAsync(ww => ww.ID == deviceJWT.ID);
                    if (device is not null)
                    {
                        if (!Constants.AllowedVersions.Contains(item.GCSVersion))
                        {
                            continue;
                        }
                        if (item.FlywayPoints.Count == 0)
                        {
                            await emailService.SendMailAsync(new String[] { "huynhthehainam@gmail.com" }, new String[] { }, new String[] { }, "Report flight stat", @$"
                                task area: {item.TaskArea},
                                sprayedIndexes: {item.SprayedIndexes.Count()}
                                flywayPoints: {item.FlywayPoints.Count()}
                                device: {device.Name}
                                flightDuration: {item.FlightDuration.GetValueOrDefault()}
                                flightTime: {item.FlightTime}
                            ");
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
                            DeviceID = device.ID,
                            DeviceName = device.Name,
                            TaskLocation = item.TaskLocation,
                            TeamID = device.TeamID,
                            TaskArea = item.TaskArea.GetValueOrDefault(),
                            ExecutionCompanyID = device.ExecutionCompanyID,
                            GCSVersion = item.GCSVersion,
                            AdditionalInformation = item.AdditionalInformation,
                        };

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
                            List<TeamUser> teamUsers = team.TeamUsers.ToList();

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
                        await deviceRepository.UpdateAsync(device);
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
        public async Task<IActionResult> CreateTelemetryRecord([FromServices] DeviceRepository deviceRepository,
        [FromServices] BatteryGroupLogRepository batteryGroupLogRepository,
        [FromServices] BatteryModelRepository batteryModelRepository,
        [FromServices] EmailService emailService,
        [FromServices] BatteryRepository batteryRepository, [FromServices] TelemetryGroupRepository telemetryGroupRepository, [FromBody] AddingBulkTelemetryRecordCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = await deviceRepository.GetAsync(ww => ww.ID == CurrentDevice.ID);

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


            await deviceRepository.UpdateAsync(device);


            response.SetCreatedObject(group);
            return response.ToIActionResult();
        }
        [HttpPost("me/Logs")]
        public async Task<IActionResult> UploadLogFile([FromServices] DeviceRepository deviceRepository, [FromServices] MinioService minioService, [FromForm] AddingLogFileCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = await deviceRepository.GetAsync(ww => ww.ID == CurrentDevice.ID);
            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }


            LogFile logFile = new LogFile
            {
                FileUrl = await minioService.PutFileAsync(command.File, new String[] { "drone-hub-api", "logs", $"{device.ID}_{device.Name}" }),
            };

            device.LogFiles.Add(logFile);

            await deviceRepository.UpdateAsync(device);


            response.SetCreatedObject(logFile);

            return response.ToIActionResult();
        }

        [HttpPost("me/FlightStats")]
        public async Task<IActionResult> CreateFlightStat([FromServices] DeviceRepository deviceRepository, [FromServices] FlightStatRepository flightStatRepository,
        [FromServices] ExecutionCompanySettingRepository executionCompanySettingRepository,
        [FromServices] DatabaseContext databaseContext,
        [FromServices] EmailService emailService,
        [FromServices] ExecutionCompanyUserFlightStatRepository executionCompanyUserFlightStatRepository, [FromBody] AddingFlightStatCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = await deviceRepository.GetAsync(ww => ww.ID == CurrentDevice.ID);
            if (!Constants.AllowedVersions.Contains(command.GCSVersion))
            {
                response.SetMessage("Invalid");
                return response.ToIActionResult();
            }
            if (command.FlywayPoints.Count == 0)
            {
                response.SetMessage("Invalid");
                await emailService.SendMailAsync(new String[] { "huynhthehainam@gmail.com" }, new String[] { }, new String[] { }, "Report flight stat", @$"
                                task area: {command.TaskArea},
                                sprayedIndexes: {command.SprayedIndexes.Count()}
                                device: {device.Name}
                                flightDuration: {command.FlightDuration.GetValueOrDefault()}
                                flywayPoints: {command.FlywayPoints.Count()}
                                flightTime: {command.FlightTime}
                            ");
                return response.ToIActionResult();
            }
            if (device is null)
            {
                response.AddNotFoundErr("Device");
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
                DeviceID = device.ID,
                DeviceName = device.Name,
                TaskLocation = command.TaskLocation,
                TaskArea = command.TaskArea.GetValueOrDefault(),
                TeamID = device.TeamID,
                ExecutionCompanyID = device.ExecutionCompanyID,
                GCSVersion = command.GCSVersion,
                AdditionalInformation = command.AdditionalInformation
            };
            if (device.ExecutionCompanyID.HasValue)
            {
                var latestSetting = executionCompanySettingRepository.GetLatestSetting(device.ExecutionCompanyID.GetValueOrDefault());
                if (latestSetting is not null)
                {
                    stat.Cost = stat.TaskArea / 10000 * latestSetting.CostPerHectare;
                }
            }


            await flightStatRepository.CreateAsync(stat);
            if (device.Team is not null)
            {
                var team = device.Team;
                List<TeamUser> teamUsers = team.TeamUsers.ToList();

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
            await deviceRepository.UpdateAsync(device);

            response.SetCreatedObject(stat);

            return response.ToIActionResult();
        }
        [HttpPost("me/StreamingLinks")]

        public async Task<IActionResult> CreateStreamingLink([FromServices] DeviceRepository deviceRepository, [FromBody] AddingStreamingLinkCommand command)
        {
            var actionResponse = actionResponseFactory.CreateInstance();
            var device = await deviceRepository.GetAsync(ww => ww.ID == CurrentDevice.ID);

            if (device is null)
            {
                actionResponse.AddNotFoundErr("Device");
            }

            StreamingLink streamingLink = new StreamingLink()
            {
                Link = command.Link
            };



            device.StreamingLinks.Add(streamingLink);


            await deviceRepository.UpdateAsync(device);

            actionResponse.SetCreatedObject(streamingLink);
            var result = await FirebaseMessaging.DefaultInstance.SendAsync(new Message() { Topic = "dronehub_message", Data = new Dictionary<String, String>() { { "update", "streaming_links" }, } });
            return actionResponse.ToIActionResult();
        }

        [HttpPost("me/RemoveAllStreamingLinks")]
        public async Task<IActionResult> RemoveAllStreamingLinks([FromServices] DeviceRepository deviceRepository, [FromServices] StreamingLinkRepository streamingLinkRepository)
        {
            var actionResponse = actionResponseFactory.CreateInstance();

            var device = await deviceRepository.GetAsync(ww => ww.ID == CurrentDevice.ID);

            if (device is null)
            {
                actionResponse.AddNotFoundErr("Device");
            }




            await streamingLinkRepository.DeleteRangeAsync(device.StreamingLinks.ToList());
            actionResponse.SetUpdatedMessage();

            var result = await FirebaseMessaging.DefaultInstance.SendAsync(new Message() { Topic = "dronehub_message", Data = new Dictionary<String, String>() { { "update", "streaming_links" }, } });
            return actionResponse.ToIActionResult();
        }

        [HttpPost("me/Plans")]
        public async Task<IActionResult> CreatePlan([FromServices] DeviceRepository deviceRepository, [FromServices] PlanRepository planRepository, [FromForm] AddingPlanCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = await deviceRepository.GetAsync(ww => ww.ID == CurrentDevice.ID);

            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var plan = await planRepository.GetAsync(ww => ww.FileName == command.File.FileName && ww.Device == device);
            if (plan is null)
            {
                plan = new Plan { FileName = command.File.FileName, Device = device };
            }

            plan.Location = geometryFactory.CreatePoint(new Coordinate(command.Longitude.GetValueOrDefault(), command.Latitude.GetValueOrDefault()));
            plan.FileName = command.File.FileName;
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
        [FromQuery] PageCommand pageCommand, [FromQuery] String search, [FromQuery] Double? latitude, [FromQuery] Double? longitude, [FromQuery] Double? range = 5000)
        {
            Console.WriteLine($"lat: {latitude} lng: {longitude}, range: {range}");
            var response = actionResponseFactory.CreateInstance();
            var device = await deviceRepository.GetAsync(ww => ww.ID == CurrentDevice.ID);

            if (device is null)
            {
                response.AddNotFoundErr("Device");
            }

            Point centerLocation = null;
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            if (latitude.HasValue && longitude.HasValue && range.HasValue)
            {
                centerLocation = geometryFactory.CreatePoint(new Coordinate(longitude.GetValueOrDefault(), latitude.GetValueOrDefault()));
            }
            Expression<Func<Plan, Boolean>> query = ww => (ww.Device.ExecutionCompanyID == device.ExecutionCompanyID)
            && (String.IsNullOrWhiteSpace(search) ?
            ((centerLocation != null) ? (ww.Location.Distance(centerLocation) < range.GetValueOrDefault()) : true)
            : ww.FileName.ToLower().Contains(search.ToLower()))
            && (ww.Device.ExecutionCompanyID == device.ExecutionCompanyID);
            var listResponse = await planRepository.GetListResponseViewAsync<SmallPlanViewModel>(pageCommand, query, ww => ww.CreatedTime, false);
            if (centerLocation is not null)
            {
                foreach (var item in listResponse.Data)
                {
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
            }
            Expression<Func<Plan, Boolean>> query = ww => (ww.Device.ExecutionCompanyID == device.ExecutionCompanyID)

                       && (ww.Device.ExecutionCompanyID == device.ExecutionCompanyID)
                       && (ww.ID == command.PlanID);
            var plan = await planRepository.GetAsync(query);
            if (plan is null)
            {
                response.AddNotFoundErr("Plan");
            }

            response.SetFile(plan.FileBytes, "application/octet-stream", plan.FileName);



            return response.ToIActionResult();
        }



    }
}