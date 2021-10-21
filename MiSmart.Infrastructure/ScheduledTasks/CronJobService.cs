using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Hosting;
namespace MiSmart.Infrastructure.ScheduledTasks
{
    public class CronJobService : IHostedService, IDisposable
    {
        private readonly CronExpression expression;
        private readonly TimeZoneInfo timeZoneInfo;
        private System.Timers.Timer timer;
        public CronJobService(IScheduleConfig<CronJobService> options)
        {
            this.expression = CronExpression.Parse(options.CronExpression, CronFormat.IncludeSeconds);
            this.timeZoneInfo = options.TimeZoneInfo;
        }
        public void Dispose()
        {
            timer?.Dispose();
        }
        protected virtual Task ScheduleJob(CancellationToken cancellationToken)
        {
            var next = expression.GetNextOccurrence(DateTimeOffset.Now, timeZoneInfo);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                if (delay.TotalMilliseconds <= 0)
                {
                    ScheduleJob(cancellationToken);
                }
                timer = new System.Timers.Timer(delay.TotalMilliseconds);
                timer.Elapsed += (sender, args) =>
                {
                    timer.Dispose();
                    timer = null;
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        DoWork(cancellationToken);
                    }
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        ScheduleJob(cancellationToken);    // reschedule next
                    }
                };
                timer.Start();
            }
            return Task.CompletedTask;
        }
        public virtual Task DoWork(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return ScheduleJob(cancellationToken);
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Stop();
            return Task.CompletedTask;
        }
    }
}