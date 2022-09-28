
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
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
            public class Location{
                public Double Lat {get; set; }
                public Double Lng {get; set; }
            }
            public class XYZ {
                public Double X {get; set; }
                public Double Y {get; set; }
                public Double Z {get; set; }
            }
            public class EdgeData {
                public Double Roll {get; set; }
                public Double Pitch {get; set; }
            }
            public Double flight_time {get; set; }
            public Double fuel_avg_per {get; set; }
            public XYZ max_accel {get; set; }
            public EdgeData max_angle {get; set; }
            public Double max_battery_deviation {get; set; }
            public Location max_cordinate {get; set;}
            public Double max_height {get; set; }
            public Double max_speed {get; set; }
            public XYZ max_vibe {get; set; }
            public Double pin_min_per {get; set; }
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
                    var logs = databaseContext.LogFiles.Where(ww => ww.FileBytes.Length > 500000 && ww.FileBytes.Length < 5000000 && !ww.isAnalyzed).OrderByDescending(ww => ww.LoggingTime).Take(2).ToList();
                    foreach (var log in logs)
                    {
                        var detail = databaseContext.LogDetails.Where(ww => ww.LogFileID == log.ID);
                        var formData = new MultipartFormDataContent();
                        formData.Add(new StreamContent(new MemoryStream(log.FileBytes)), "file", log.FileName);
                        try{
                        var response = client.PostAsync("http://node1.dronelog.mismart.ai/analysis", formData).GetAwaiter().GetResult();
                        if (response.IsSuccessStatusCode)
                        {
                            string responseString = response.Content.ReadAsStringAsync().Result;
                            DroneLogAPIResponse droneLogAPIResponse = JsonSerializer.Deserialize<DroneLogAPIResponse>(responseString, JsonSerializerDefaultOptions.CamelOptions);
                            if (droneLogAPIResponse.Result is not null){
                                var model = new LogDetail(){
                                    AccelX = droneLogAPIResponse.Result.max_accel.X,
                                    AccelY = droneLogAPIResponse.Result.max_accel.Y,
                                    AccelZ = droneLogAPIResponse.Result.max_accel.Z,
                                    PercentFuel = droneLogAPIResponse.Result.fuel_avg_per,
                                    BatteryCellDeviation = droneLogAPIResponse.Result.max_battery_deviation,
                                    VibeX = droneLogAPIResponse.Result.max_vibe.X,
                                    VibeY = droneLogAPIResponse.Result.max_vibe.Y,
                                    VibeZ = droneLogAPIResponse.Result.max_vibe.Z,
                                    FlySpeed = droneLogAPIResponse.Result.max_speed,
                                    Height = droneLogAPIResponse.Result.max_height,
                                    Roll = droneLogAPIResponse.Result.max_angle.Roll,
                                    Pitch =droneLogAPIResponse.Result.max_angle.Pitch,
                                    FlightDuration = droneLogAPIResponse.Result.flight_time,
                                    PercentBattery = droneLogAPIResponse.Result.pin_min_per,
                                    LogFileID = log.ID,
                                    Latitude = droneLogAPIResponse.Result.max_cordinate.Lat,
                                    Longitude = droneLogAPIResponse.Result.max_cordinate.Lng,
                                    IsBingLocation = false,
                                };
                                databaseContext.LogDetails.Add(model);
                            }
                        }
                        }catch (AggregateException)
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