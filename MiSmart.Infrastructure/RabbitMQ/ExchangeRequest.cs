using System;

namespace MiSmart.Infrastructure.RabbitMQ
{
    public class ExchangeRequest<T> where T : class
    {
        public String Type { get; set; }
        public T Data { get; set; }
    }
}