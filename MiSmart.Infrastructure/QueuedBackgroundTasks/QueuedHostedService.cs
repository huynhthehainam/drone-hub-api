
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MiSmart.Infrastructure.QueuedBackgroundTasks
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger<QueuedHostedService> logger;
        protected Int32 index = 0;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue,
            ILogger<QueuedHostedService> logger)
        {
            TaskQueue = taskQueue;
            this.logger = logger;
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation(
                $"Queued Hosted Service is running.{Environment.NewLine}" +
                $"{Environment.NewLine}Tap W to add a work item to the " +
                $"background queue.{Environment.NewLine}");

            return BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(stoppingToken);

                // try
                // {
                Console.WriteLine($"Executing {nameof(workItem)} of {index}");
                await workItem(stoppingToken);
                // }
                // catch (Exception ex)
                // {
                //     logger.LogError(ex,
                //         "Error occurred executing {WorkItem}.", nameof(workItem));
                // }
            }



        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Queued Hosted Service is stopping.");

            return base.StopAsync(stoppingToken);
        }
    }
}