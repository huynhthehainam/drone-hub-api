
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Constants;
using MiSmart.Infrastructure.ScheduledTasks;

namespace MiSmart.API.ScheduledTasks
{
    public class DroneLogAPIResponse
    {
        public sealed class DroneLogResult
        {
            public class DroneLogAPIData
            {
                public class Location
                {
                    public Double Lat { get; set; }
                    public Double Lng { get; set; }
                }
                public class XYZ
                {
                    public Double X { get; set; }
                    public Double Y { get; set; }
                    public Double Z { get; set; }
                }
                public class EdgeData
                {
                    public Double Roll { get; set; }
                    public Double Pitch { get; set; }
                }
                [JsonPropertyName("flight_time")]
                public Double FlightTime { get; set; }
                [JsonPropertyName("fuel_avg_per")]
                public Double FuelAvgPercent { get; set; }
                [JsonPropertyName("max_accel")]
                public XYZ? MaxAccel { get; set; }
                [JsonPropertyName("max_angle")]
                public EdgeData? MaxAngle { get; set; }
                [JsonPropertyName("max_battery_deviation")]
                public Double MaxBatteryDeviation { get; set; }
                [JsonPropertyName("max_cordinate")]
                public Location? MaxCoordinate { get; set; }
                [JsonPropertyName("max_height")]
                public Double MaxHeight { get; set; }
                [JsonPropertyName("max_speed")]
                public Double MaxSpeed { get; set; }
                [JsonPropertyName("max_vibe")]
                public XYZ? MaxVibe { get; set; }
                [JsonPropertyName("pin_min_per")]
                public Double BatteryMinPercent { get; set; }
            }

            public DroneLogAPIData? Analysis { get; set; }
            [JsonPropertyName("error_analysis")]
            public JsonDocument? Error { get; set; }
        }

        public DroneLogResult? Result { get; set; }
        [JsonPropertyName("msg")]
        public String? Message { get; set; }
    }
    public sealed class CreateDroneLogTaskResponse
    {
        [JsonPropertyName("task_id")]
        public Double TaskID { get; set; }
    }
    public class UpdatingLogDetail : CronJobService
    {
        private IServiceProvider serviceProvider;
        public UpdatingLogDetail(IScheduleConfig<UpdatingLogDetail> options, IServiceProvider serviceProvider) : base(options)
        {
            this.serviceProvider = serviceProvider;
        }
        public override Task DoWork(CancellationToken cancellationToken)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                IHttpClientFactory clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

                using (DatabaseContext databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>())
                {
                    var client = clientFactory.CreateClient();
                    var logs = databaseContext.LogFiles.Where(ww => (ww.FileBytes != null ? ww.FileBytes.Length > 500000 : false) && (ww.FileBytes != null ? ww.FileBytes.Length < 50000000 : false) && ww.DroneLogAnalyzingTaskID == null).OrderByDescending(ww => ww.LoggingTime).Take(10).ToList();
                    foreach (var log in logs)
                    {
                        var detail = databaseContext.LogDetails.Where(ww => ww.LogFileID == log.ID);
                        var formData = new MultipartFormDataContent();
                        formData.Add(new StreamContent(new MemoryStream(log.FileBytes ?? new Byte[0])), "file", log.FileName ?? "ex.bin");
                        try
                        {
                            var response = client.PostAsync("http://node2.dronelog.mismart.ai/analysis", formData).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                String responseString = response.Content.ReadAsStringAsync().Result;

                                CreateDroneLogTaskResponse? createDroneLogTaskResponse = JsonSerializer.Deserialize<CreateDroneLogTaskResponse>(responseString);
                                Console.WriteLine($"create analyzing task  {responseString} {createDroneLogTaskResponse?.TaskID}");
                                if (createDroneLogTaskResponse != null && createDroneLogTaskResponse.TaskID > 0)
                                {

                                    log.DroneLogAnalyzingTaskID = createDroneLogTaskResponse.TaskID;
                                    log.AnalyzingTime = DateTime.UtcNow;

                                    databaseContext.LogFiles.Update(log);
                                    databaseContext.SaveChanges();
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }

                    }
                }
            }

            return Task.CompletedTask;
        }
    }
    public sealed class GetResultLogDetail : CronJobService
    {
        private IServiceProvider serviceProvider;
        public GetResultLogDetail(IScheduleConfig<GetResultLogDetail> options, IServiceProvider serviceProvider) : base(options)
        {
            this.serviceProvider = serviceProvider;
        }
        public override Task DoWork(CancellationToken cancellationToken)
        {

            using (var scope = serviceProvider.CreateScope())
            {
                IHttpClientFactory clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                using (DatabaseContext databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>())
                {
                    var client = clientFactory.CreateClient();
                    var logs = databaseContext.LogFiles.Where(ww => ww.DroneLogAnalyzingTaskID != null && !ww.IsAnalyzed).OrderBy(ww => ww.AnalyzingTime).Take(10).ToList();
                    foreach (var log in logs)
                    {
                        var response = client.GetAsync($"http://node2.dronelog.mismart.ai/get_result/{log.DroneLogAnalyzingTaskID}").Result;
                        if (response.IsSuccessStatusCode)
                        {
                            String responseString = response.Content.ReadAsStringAsync().Result;
                            Console.WriteLine($"get log result detail: {responseString}");
                            DroneLogAPIResponse? droneLogAPIResponse = JsonSerializer.Deserialize<DroneLogAPIResponse>(responseString, JsonSerializerDefaultOptions.CamelOptions);
                            if (droneLogAPIResponse != null && droneLogAPIResponse.Message == null)
                            {
                                if (droneLogAPIResponse.Result is not null)
                                {
                                    var model = new LogDetail()
                                    {
                                        AccelX = droneLogAPIResponse?.Result?.Analysis?.MaxAccel?.X ?? 0,
                                        AccelY = droneLogAPIResponse?.Result?.Analysis?.MaxAccel?.Y ?? 0,
                                        AccelZ = droneLogAPIResponse?.Result?.Analysis?.MaxAccel?.Z ?? 0,
                                        PercentFuel = droneLogAPIResponse?.Result?.Analysis?.FuelAvgPercent ?? 0,
                                        BatteryCellDeviation = droneLogAPIResponse?.Result?.Analysis?.MaxBatteryDeviation ?? 0,
                                        VibeX = droneLogAPIResponse?.Result?.Analysis?.MaxVibe?.X ?? 0,
                                        VibeY = droneLogAPIResponse?.Result?.Analysis?.MaxVibe?.Y ?? 0,
                                        VibeZ = droneLogAPIResponse?.Result?.Analysis?.MaxVibe?.Z ?? 0,
                                        FlySpeed = droneLogAPIResponse?.Result?.Analysis?.MaxSpeed ?? 0,
                                        Height = droneLogAPIResponse?.Result?.Analysis?.MaxHeight ?? 0,
                                        Roll = droneLogAPIResponse?.Result?.Analysis?.MaxAngle?.Roll ?? 0,
                                        Pitch = droneLogAPIResponse?.Result?.Analysis?.MaxAngle?.Pitch ?? 0,
                                        FlightDuration = droneLogAPIResponse?.Result?.Analysis?.FlightTime ?? 0,
                                        PercentBattery = droneLogAPIResponse?.Result?.Analysis?.BatteryMinPercent ?? 0,
                                        LogFileID = log.ID,
                                        Latitude = droneLogAPIResponse?.Result?.Analysis?.MaxCoordinate?.Lat ?? 0,
                                        Longitude = droneLogAPIResponse?.Result?.Analysis?.MaxCoordinate?.Lng ?? 0,
                                        IsBingLocation = false,
                                        Error = droneLogAPIResponse?.Result?.Error,
                                    };
                                    databaseContext.LogDetails.Add(model);

                                }
                                log.IsAnalyzed = true;
                                databaseContext.LogFiles.Update(log);
                                databaseContext.SaveChanges();
                            }
                        }
                        else
                        {
                            log.IsAnalyzed = true;
                            databaseContext.LogFiles.Update(log);
                            databaseContext.SaveChanges();
                        }

                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}