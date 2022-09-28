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
    public class UpdatingLogBingLocation : CronJobService
    {
        private IServiceProvider serviceProvider;
        public UpdatingLogBingLocation(IScheduleConfig<UpdatingTaskBingLocation> options, IServiceProvider serviceProvider) : base(options)
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
                    var logs = databaseContext.LogDetails.Where(ww => !ww.IsBingLocation).Take(10).ToList();
                    foreach (var log in logs)
                    {
                        log.Location = await BingLocationHelper.UpdateLogLocation(log, clientFactory);
                        log.IsBingLocation = true;

                        databaseContext.LogDetails.Update(log);
                        databaseContext.SaveChanges();
                    }
                }
            }
        }
    }
}