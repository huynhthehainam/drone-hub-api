

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.Infrastructure.Mqtt;
using MQTTnet;
using MQTTnet.AspNetCore;
using MQTTnet.AspNetCore.AttributeRouting;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace MiSmart.API.Services
{
    public class MqttService : IMqttService
    {
        public IMqttServer Server;
        private readonly IServiceProvider serviceProvider;
        public MqttService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public void ConfigureMqttServerOptions(AspNetMqttServerOptionsBuilder options)
        {
            Console.WriteLine("Configure Mqtt Server Options");

            options.WithoutDefaultEndpoint();
            options.WithConnectionValidator(this);
            options.WithApplicationMessageInterceptor(this);
            options.WithAttributeRouting(true);
        }
        public void ConfigureMqttServer(IMqttServer mqtt)
        {
            Console.WriteLine("Configure Mqtt Server");
            Server = mqtt;
            mqtt.ApplicationMessageReceivedHandler = this;
            mqtt.StartedHandler = this;
            mqtt.StoppedHandler = this;
            mqtt.ClientConnectedHandler = this;
            mqtt.ClientDisconnectedHandler = this;
            mqtt.ClientSubscribedTopicHandler = this;
            mqtt.ClientUnsubscribedTopicHandler = this;
        }

        public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            // return Task.Run(() =>
            //   {
            //       Console.WriteLine(
            //           $"{DateTime.Now.ToString(CultureInfo.InvariantCulture)} - Received MQTT Message Logged:");
            //       Console.WriteLine($"- Topic = {eventArgs.ApplicationMessage.Topic }");
            //       Console.WriteLine($"- Payload = {Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload) }");
            //       Console.WriteLine($"- QoS = {eventArgs.ApplicationMessage.QualityOfServiceLevel }");
            //       Console.WriteLine($"- Retain = {eventArgs.ApplicationMessage.Retain }");
            //   });
            return Task.CompletedTask;
        }

        public Task HandleClientConnectedAsync(MqttServerClientConnectedEventArgs eventArgs)
        {

            return Task.CompletedTask;
        }

        public Task HandleClientDisconnectedAsync(MqttServerClientDisconnectedEventArgs eventArgs)
        {
            return Task.CompletedTask;
        }

        public Task HandleClientSubscribedTopicAsync(MqttServerClientSubscribedTopicEventArgs eventArgs)
        {
            return Task.CompletedTask;
        }

        public Task HandleClientUnsubscribedTopicAsync(MqttServerClientUnsubscribedTopicEventArgs eventArgs)
        {
            return Task.CompletedTask;
        }

        public Task HandleServerStartedAsync(EventArgs eventArgs)
        {
            return Task.CompletedTask;
        }

        public Task HandleServerStoppedAsync(EventArgs eventArgs)
        {
            return Task.CompletedTask;
        }

        public Task InterceptApplicationMessagePublishAsync(MqttApplicationMessageInterceptorContext context)
        {
            return Task.CompletedTask;
        }

        public Task ValidateConnectionAsync(MqttConnectionValidatorContext context)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                using (var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>())
                {
                    var device = databaseContext.Devices.FirstOrDefault(ww => ww.Token == context.Username);
                    if (device is not null)
                    {
                        context.SessionItems.Add(context.ClientId, device);
                        Console.WriteLine($"ClientID {context.Username} is validated");
                    }
                    else
                    {
                        Console.WriteLine($"ClientID {context.Username} is invalidated");
                        context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}