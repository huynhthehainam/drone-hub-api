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
using MiSmart.Infrastructure.Minio;
using System.Collections.Generic;

namespace MiSmart.API.RabbitMQ
{
    public class ConsumeAuthRabbitMQHostedService : BackgroundService
    {
        private IConnection? connection;
        private IModel? channel;
        private readonly IServiceProvider serviceProvider;
        private readonly RabbitOptions rabbitOptions;
        private readonly MinioService minioService;

        public ConsumeAuthRabbitMQHostedService(IServiceProvider serviceProvider, MinioService minioService, IOptions<RabbitOptions> options1)
        {
            this.serviceProvider = serviceProvider;
            this.rabbitOptions = options1.Value;
            this.minioService = minioService;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitOptions.HostName,
                UserName = rabbitOptions.UserName,
                Password = rabbitOptions.Password
            };

            // create connection  
            connection = factory.CreateConnection();

            // create channel  
            channel = connection.CreateModel();
            channel.BasicQos(0, 1, false);
            var queueName = channel.QueueDeclare("mismart.queue.drone", false, false, false, null);

            channel.QueueBind(queueName, "mismart", "mismart.queue.*", null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
                       {
                           // received message  
                           var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());

                           // handle the received message  
                           HandleMessage(content);
                           channel.BasicAck(ea.DeliveryTag, false);
                       };
            channel.BasicConsume("mismart.queue.drone", false, consumer);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();




            return Task.CompletedTask;
        }

        private async void HandleMessage(String content)
        {
            try
            {
                ExchangeRequest<Object>? contentModel = JsonSerializer.Deserialize<ExchangeRequest<Object>>(content, JsonSerializerDefaultOptions.CamelOptions);
                if (contentModel != null && contentModel.Type == "RemoveUser")
                {
                    RemovingUserModel? model = JsonSerializer.Deserialize<RemovingUserModel>(JsonSerializer.Serialize(contentModel.Data, JsonSerializerDefaultOptions.CamelOptions), JsonSerializerDefaultOptions.CamelOptions);
                    if (model != null)
                        using (var context = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>())
                        {
                            var customerUsers = context.CustomerUsers.Where(ww => ww.UserUUID == model.UUID).ToList();
                            context.CustomerUsers.RemoveRange(customerUsers);
                            context.SaveChanges();

                            var executionCompanyUsers = context.ExecutionCompanyUsers.Where(ww => ww.UserUUID == model.UUID).ToList();
                            context.ExecutionCompanyUsers.RemoveRange(executionCompanyUsers);
                            context.SaveChanges();

                            var secondLogReports = context.SecondLogReports.Where(ww => ww.UserUUID == model.UUID).ToList();
                            context.SecondLogReports.RemoveRange(secondLogReports);
                            context.SaveChanges();

                            var logReports = context.LogReports.Where(ww => ww.UserUUID == model.UUID).ToList();
                            context.LogReports.RemoveRange(logReports);
                            context.SaveChanges();

                            var logTokens = context.LogTokens.Where(ww => ww.UserUUID == model.UUID).ToList();
                            context.LogTokens.RemoveRange(logTokens);
                            context.SaveChanges();
                        }
                }
            }
            catch (Exception)
            {

            }
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            channel?.Close();
            connection?.Close();
            base.Dispose();
        }
    }
}