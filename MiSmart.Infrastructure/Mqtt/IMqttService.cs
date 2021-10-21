

using MQTTnet.AspNetCore;
using MQTTnet.Client.Receiving;
using MQTTnet.Server;

namespace MiSmart.Infrastructure.Mqtt
{
    public interface IMqttService : IMqttServerConnectionValidator,
        IMqttApplicationMessageReceivedHandler,
        IMqttServerApplicationMessageInterceptor,
        IMqttServerStartedHandler,
        IMqttServerStoppedHandler,
        IMqttServerClientConnectedHandler,
        IMqttServerClientDisconnectedHandler,
        IMqttServerClientSubscribedTopicHandler,
        IMqttServerClientUnsubscribedTopicHandler
    {
        void ConfigureMqttServerOptions(AspNetMqttServerOptionsBuilder options);
        void ConfigureMqttServer(IMqttServer mqtt);
    }
}