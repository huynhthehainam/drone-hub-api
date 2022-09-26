
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
        public class Location{
            public Double Lag {get; set; }
            public Double Lng {get; set; }
        }
        public class DroneLogAPIData
        {
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
                                    Accel = JsonDocument.Parse(JsonSerializer.Serialize(droneLogAPIResponse.Result.max_accel, JsonSerializerDefaultOptions.CamelOptions)),
                                    PercentFuel = droneLogAPIResponse.Result.fuel_avg_per,
                                    BatteryCellDeviation = droneLogAPIResponse.Result.max_battery_deviation,
                                    Vibe = JsonDocument.Parse(JsonSerializer.Serialize(droneLogAPIResponse.Result.max_vibe, JsonSerializerDefaultOptions.CamelOptions)),
                                    FlySpeed = droneLogAPIResponse.Result.max_speed,
                                    Heigh = droneLogAPIResponse.Result.max_height,
                                    Edge = JsonDocument.Parse(JsonSerializer.Serialize(droneLogAPIResponse.Result.max_angle, JsonSerializerDefaultOptions.CamelOptions)),
                                    FlightDuration = droneLogAPIResponse.Result.flight_time,
                                    PercentBattery = droneLogAPIResponse.Result.pin_min_per,
                                    LogFileID = log.ID,
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