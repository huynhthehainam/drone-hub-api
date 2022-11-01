
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.Infrastructure.ScheduledTasks;

namespace MiSmart.API.ScheduledTasks
{
    public class UpdatingCostFlightStatsTask : CronJobService
    {
        private IServiceProvider serviceProvider;
        public UpdatingCostFlightStatsTask(IScheduleConfig<UpdatingCostFlightStatsTask> options, IServiceProvider serviceProvider) : base(options)
        {
            this.serviceProvider = serviceProvider;
        }
        public override Task DoWork(CancellationToken cancellationToken)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                using (DatabaseContext databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>())
                {
                    var flightStats = databaseContext.FlightStats.Where(ww => ww.Cost == 0 && ww.ExecutionCompanyID != null && ((ww.ExecutionCompany != null && ww.ExecutionCompany.Settings != null) ? ww.ExecutionCompany.Settings.Any() : false) && ww.TaskArea > 0).ToList();
                    foreach (var flightStat in flightStats)
                    {
                        if (flightStat.ExecutionCompanyID.HasValue)
                        {
                            var latestSetting = databaseContext.ExecutionCompanySettings.Where(ww => ww.ExecutionCompanyID == flightStat.ExecutionCompanyID.GetValueOrDefault()).OrderByDescending(ww => ww.CreatedTime).FirstOrDefault();
                            if (latestSetting is not null)
                            {
                                flightStat.Cost = flightStat.TaskArea / 10000 * latestSetting.CostPerHectare;
                                databaseContext.Update(flightStat);
                                databaseContext.SaveChanges();
                            }
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}