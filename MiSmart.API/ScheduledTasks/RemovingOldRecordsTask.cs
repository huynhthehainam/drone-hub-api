
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ScheduledTasks;

namespace MiSmart.API.ScheduledTasks
{
    public class RemovingOldRecordsTask : CronJobService
    {
        private IServiceProvider serviceProvider;
        public RemovingOldRecordsTask(IScheduleConfig<RemovingOldRecordsTask> options, IServiceProvider serviceProvider) : base(options)
        {
            this.serviceProvider = serviceProvider;
        }
        public override Task DoWork(CancellationToken cancellationToken)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                using (DatabaseContext databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>())
                {
                    List<TelemetryGroup> groups = databaseContext.TelemetryGroups.Where(g => g.CreatedTime < DateTime.Now.AddDays(-7) && g.LastDevice == null).ToList();
                    databaseContext.TelemetryGroups.RemoveRange(groups);
                    databaseContext.SaveChanges();
                  
                }
            }




            return Task.CompletedTask;
        }
    }
}