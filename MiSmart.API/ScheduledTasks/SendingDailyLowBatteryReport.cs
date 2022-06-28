
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MiSmart.API.Services;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.Infrastructure.ScheduledTasks;

namespace MiSmart.API.ScheduledTasks
{
    public class SendingDailyLowBatteryReport : CronJobService
    {
        private IServiceProvider serviceProvider;
        public SendingDailyLowBatteryReport(IScheduleConfig<SendingDailyLowBatteryReport> options, IServiceProvider serviceProvider) : base(options)
        {
            this.serviceProvider = serviceProvider;
        }
        public override async Task DoWork(CancellationToken cancellationToken)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                MyEmailService emailService = scope.ServiceProvider.GetRequiredService<MyEmailService>();
                TimeZoneInfo seaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var utcNow = DateTime.UtcNow;
                var localNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, seaTimeZone);
                var localStartTime = new DateTime(localNow.Year, localNow.Month, localNow.Day, 0, 0, 0);
                var localEndTime = new DateTime(localNow.Year, localNow.Month, localNow.Day, 23, 59, 59);
                var utcStartTime = TimeZoneInfo.ConvertTimeToUtc(localStartTime, seaTimeZone);
                var utcEndTime = TimeZoneInfo.ConvertTimeToUtc(localEndTime, seaTimeZone);
                using (DatabaseContext databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>())
                {
                    var flightStats = databaseContext.FlightStats.Where(ww => ww.FlightTime >= utcStartTime && ww.FlightTime <= utcEndTime && ww.BatteryPercentRemaining.GetValueOrDefault(100) < 30).OrderBy(ww => ww.FlightTime).ToList();

                    await emailService.SendLowBatteryDailyReport(flightStats, localNow);
                }

            }
        }
    }
}