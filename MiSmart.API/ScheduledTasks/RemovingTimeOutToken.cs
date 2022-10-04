
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
    public class RemovingTimeOutToken : CronJobService
    {
        private IServiceProvider serviceProvider;
        public RemovingTimeOutToken(IScheduleConfig<RemovingOldRecordsTask> options, IServiceProvider serviceProvider) : base(options)
        {
            this.serviceProvider = serviceProvider;
        }
        public override Task DoWork(CancellationToken cancellationToken)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                using (DatabaseContext databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>())
                {
                    List<LogToken> tokens = databaseContext.LogTokens.Where(g => g.CreateTime < DateTime.UtcNow.AddHours(-24)).ToList();
                    databaseContext.LogTokens.RemoveRange(tokens);
                    databaseContext.SaveChanges();
                }
            }
            return Task.CompletedTask;
        }
    }
}