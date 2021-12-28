using System;

namespace MiSmart.Infrastructure.RabbitMQ
{
    public interface IRabbitManager
    {
        void Publish<T>(T message, String exchangeName, String exchangeType, String routeKey)
            where T : class;
    }
}