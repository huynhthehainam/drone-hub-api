using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System.Threading.Tasks;
using System.Threading;
using RabbitMQ.Client.Events;
using System;
using Microsoft.Extensions.Options;
using System.Text.Json;
using MiSmart.Infrastructure.RabbitMQ;
using MiSmart.API.RabbitMQ.Models;
using MiSmart.DAL.DatabaseContexts;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using MiSmart.Infrastructure.Constants;

namespace MiSmart.Microservices.OrderService.RabbitMQ
{
    public class ConsumeAuthRabbitMQHostedService : BackgroundService
    {
        private IConnection connection;
        private IModel channel;
        private readonly IServiceProvider serviceProvider;
        private readonly RabbitOptions rabbitOptions;

        public ConsumeAuthRabbitMQHostedService(IServiceProvider serviceProvider, IOptions<RabbitOptions> options1)
        {
            this.serviceProvider = serviceProvider;
            this.rabbitOptions = options1.Value;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = rabbitOptions.HostName };

            // create connection  
            connection = factory.CreateConnection();

            // create channel  
            channel = connection.CreateModel();


            channel.ExchangeDeclare("mismart.exchange.auth.dotnetcore", ExchangeType.Topic, true, false, null);
            channel.QueueDeclare("mismart.queue.log", false, false, false, null);
            channel.QueueBind("mismart.queue.log", "mismart.exchange.auth.dotnetcore", "mismart.queue.*", null);
            channel.BasicQos(0, 1, false);

            connection.ConnectionShutdown += RabbitMQConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                // received message  
                var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());

                // handle the received message  
                HandleMessage(content);
                channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            channel.BasicConsume("mismart.queue.log", false, consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(String content)
        {
            ExchangeRequest<Object> contentModel = JsonSerializer.Deserialize<ExchangeRequest<Object>>(content, JsonSerializerDefaultOptions.CamelOptions);
            if (contentModel.Type == "RemoveUser")
            {
                RemovingUserModel model = JsonSerializer.Deserialize<RemovingUserModel>(JsonSerializer.Serialize(contentModel.Data, JsonSerializerDefaultOptions.CamelOptions), JsonSerializerDefaultOptions.CamelOptions);
                using (var context = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>())
                {
                    var customerUsers = context.CustomerUsers.Where(ww => ww.UserID == model.ID).ToList();
                    context.CustomerUsers.RemoveRange(customerUsers);
                    context.SaveChanges();
                }
            }
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            channel.Close();
            connection.Close();
            base.Dispose();
        }
    }
}