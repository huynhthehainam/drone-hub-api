
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MiSmart.DAL.DatabaseContexts;
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
                    // List<TelemetryGroup> groups =  databaseContext.TelemetryGroups.Where(g=> g.CreatedTime < DateTime.Now)
                }
            }




            return Task.CompletedTask;
        }
    }
}