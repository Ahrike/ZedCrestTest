

namespace Infrastructure.RabbitMQ
{
    public class RabbitMqConfiguration
    {
        public string RabbitUserName { get; set; }
        public string Hostname { get; set; }
        public string QueueName { get; set; }
        public string MqPassword { get; set; }
    }
}
