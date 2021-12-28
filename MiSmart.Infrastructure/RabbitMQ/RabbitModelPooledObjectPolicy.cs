using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using MiSmart.Infrastructure.Settings;
using System;

namespace MiSmart.Infrastructure.RabbitMQ
{
    public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>
    {
        private readonly RabbitOptions rabbitOptions;

        private readonly IConnection connection;

        public RabbitModelPooledObjectPolicy(IOptions<RabbitOptions> options)
        {
            rabbitOptions = options.Value;
            connection = GetConnection();
        }

        private IConnection GetConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = rabbitOptions.HostName,
                UserName = rabbitOptions.UserName,
                Password = rabbitOptions.Password,
                Port = rabbitOptions.Port,
                VirtualHost = rabbitOptions.VHost,
            };

            return factory.CreateConnection();
        }

        public IModel Create()
        {
            return connection.CreateModel();
        }

        public Boolean Return(IModel obj)
        {
            if (obj.IsOpen)
            {
                return true;
            }
            else
            {
                obj?.Dispose();
                return false;
            }
        }
    }
}