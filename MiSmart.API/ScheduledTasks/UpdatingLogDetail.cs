
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
            public XYZ MaxAccel { get; set; }
            [JsonPropertyName("max_angle")]
            public EdgeData MaxAngle { get; set; }
            [JsonPropertyName("max_battery_deviation")]
            public Double MaxBatteryDeviation { get; set; }
            [JsonPropertyName("max_cordinate")]
            public Location MaxCoordinate { get; set; }
            [JsonPropertyName("max_height")]
            public Double MaxHeight { get; set; }
            [JsonPropertyName("max_speed")]
            public Double MaxSpeed { get; set; }
            [JsonPropertyName("max_vibe")]
            public XYZ MaxVibe { get; set; }
            [JsonPropertyName("pin_min_per")]
            public Double BatteryMinPercent { get; set; }
        }
        public DroneLogAPIData Result { get; set; }
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
                    var logs = databaseContext.LogFiles.Where(ww => ww.FileBytes.Length > 500000 && ww.FileBytes.Length < 50000000 && !ww.isAnalyzed).OrderByDescending(ww => ww.LoggingTime).Take(2).ToList();
                    foreach (var log in logs)
                    {
                        var detail = databaseContext.LogDetails.Where(ww => ww.LogFileID == log.ID);
                        var formData = new MultipartFormDataContent();
                        formData.Add(new StreamContent(new MemoryStream(log.FileBytes)), "file", log.FileName);
                        try
                        {
                            var response = client.PostAsync("http://node1.dronelog.mismart.ai/analysis", formData).GetAwaiter().GetResult();
                            if (response.IsSuccessStatusCode)
                            {
                                string responseString = response.Content.ReadAsStringAsync().Result;
                                DroneLogAPIResponse droneLogAPIResponse = JsonSerializer.Deserialize<DroneLogAPIResponse>(responseString, JsonSerializerDefaultOptions.CamelOptions);
                                if (droneLogAPIResponse.Result is not null)
                                {
                                    var model = new LogDetail()
                                    {
                                        AccelX = droneLogAPIResponse.Result.MaxAccel.X,
                                        AccelY = droneLogAPIResponse.Result.MaxAccel.Y,
                                        AccelZ = droneLogAPIResponse.Result.MaxAccel.Z,
                                        PercentFuel = droneLogAPIResponse.Result.FuelAvgPercent,
                                        BatteryCellDeviation = droneLogAPIResponse.Result.MaxBatteryDeviation,
                                        VibeX = droneLogAPIResponse.Result.MaxVibe.X,
                                        VibeY = droneLogAPIResponse.Result.MaxVibe.Y,
                                        VibeZ = droneLogAPIResponse.Result.MaxVibe.Z,
                                        FlySpeed = droneLogAPIResponse.Result.MaxSpeed,
                                        Height = droneLogAPIResponse.Result.MaxHeight,
                                        Roll = droneLogAPIResponse.Result.MaxAngle.Roll,
                                        Pitch = droneLogAPIResponse.Result.MaxAngle.Pitch,
                                        FlightDuration = droneLogAPIResponse.Result.FlightTime,
                                        PercentBattery = droneLogAPIResponse.Result.BatteryMinPercent,
                                        LogFileID = log.ID,
                                        Latitude = droneLogAPIResponse.Result.MaxCoordinate.Lat,
                                        Longitude = droneLogAPIResponse.Result.MaxCoordinate.Lng,
                                        IsBingLocation = false,
                                    };
                                    databaseContext.LogDetails.Add(model);
                                }
                            }
                        }
                        catch (AggregateException)
                        {

                        }
                        log.isAnalyzed = true;
                        databaseContext.Update(log);
                        databaseContext.SaveChanges();
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}