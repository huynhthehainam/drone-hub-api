
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MiSmart.API.Helpers;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.Infrastructure.ScheduledTasks;

namespace MiSmart.API.ScheduledTasks
{
    public class UpdatingTaskBingLocation : CronJobService
    {
        private IServiceProvider serviceProvider;
        public UpdatingTaskBingLocation(IScheduleConfig<UpdatingTaskBingLocation> options, IServiceProvider serviceProvider) : base(options)
        {
            this.serviceProvider = serviceProvider;
        }
        public override async Task DoWork(CancellationToken cancellationToken)
        {

            using (var scope = serviceProvider.CreateScope())
            {
                IHttpClientFactory clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                using (DatabaseContext databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>())
                {
                    var flightStats = databaseContext.FlightStats.Where(ww => !ww.IsBingLocation).OrderByDescending(ww => ww.FlightTime).Take(10).ToList();
                    foreach (var flightStat in flightStats)
                    {
                        flightStat.TaskLocation = await BingLocationHelper.UpdateFlightStatLocation(flightStat, clientFactory);
                        flightStat.IsBingLocation = true;

                        databaseContext.FlightStats.Update(flightStat);
                        databaseContext.SaveChanges();
                    }
                }
            }
        }
    }
}