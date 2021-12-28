using System;
namespace MiSmart.Infrastructure.RabbitMQ
{
    public class RabbitOptions
    {
        public String UserName { get; set; }
        public String Password { get; set; }
        public String HostName { get; set; }
        public Int32 Port { get; set; } = 5672;
        public String VHost { get; set; } = "/";
    }
}