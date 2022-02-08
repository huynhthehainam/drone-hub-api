using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.ObjectPool;
using MiSmart.Infrastructure.Constants;
using RabbitMQ.Client;

namespace MiSmart.Infrastructure.RabbitMQ
{
    public class RabbitManager : IRabbitManager
    {
        private readonly DefaultObjectPool<IModel> objectPool;

        public RabbitManager(IPooledObjectPolicy<IModel> objectPolicy)
        {
            objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
        }

        public void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey)
            where T : class
        {
            if (message == null)
                return;

            var channel = objectPool.Get();

            // try
            // {
            //     channel.ExchangeDeclare(exchangeName, exchangeType, true, false, null);

            //     var sendBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            //     var properties = channel.CreateBasicProperties();
            //     properties.Persistent = true;

            //     channel.BasicPublish(exchangeName, routeKey, properties, sendBytes);
            // }
            // catch (Exception ex)
            // {
            //     throw ex;
            // }
            // finally
            // {
            //     objectPool.Return(channel);
            // }

            channel.ExchangeDeclare(exchangeName, exchangeType, true, false, null);

            var sendBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, JsonSerializerDefaultOptions.CamelOptions));

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchangeName, routeKey, properties, sendBytes);
            objectPool.Return(channel);
        }
    }
}