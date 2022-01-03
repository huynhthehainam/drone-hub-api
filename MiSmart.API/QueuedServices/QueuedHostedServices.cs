using System;
using Microsoft.Extensions.Logging;
using MiSmart.Infrastructure.QueuedBackgroundTasks;

namespace MiSmart.API.QueuedServices
{
    public class QueuedHostedService1 : QueuedHostedService
    {

        public QueuedHostedService1(IBackgroundTaskQueue taskQueue, IServiceProvider serviceProvider, ILogger<QueuedHostedService> logger) : base(taskQueue, serviceProvider, logger)
        {
            index = 1;
        }
    }
    public class QueuedHostedService2 : QueuedHostedService
    {

        public QueuedHostedService2(IBackgroundTaskQueue taskQueue, IServiceProvider serviceProvider, ILogger<QueuedHostedService> logger) : base(taskQueue, serviceProvider, logger)
        {
            index = 2;
        }
    }
}