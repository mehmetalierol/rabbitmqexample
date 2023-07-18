using RabbitMQ.Client;

namespace RabbitMQ.Common
{
    public class RabbitMqManager
    {
        public string? CurrentEndPoint { get; private set; }

        private readonly ConnectionFactory factory = new() { UserName = "admin", Password = "123456" };

        private readonly List<AmqpTcpEndpoint> listHostnames = new()
        {
            new AmqpTcpEndpoint("127.0.0.1", 5670),
            new AmqpTcpEndpoint("127.0.0.1", 5671),
            new AmqpTcpEndpoint("127.0.0.1", 5672)
        };

        public IModel? CreateConnection()
        {
            var connection = factory.CreateConnection(listHostnames);
            var channel = connection.CreateModel();
            CurrentEndPoint = connection.Endpoint.HostName + ":" + connection.Endpoint.Port.ToString();
            var queueArgs = new Dictionary<string, object>
            {
                { "x-queue-type", "quorum" }
            };

            channel.QueueDeclare(queue: "test.quorrum.queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: queueArgs);

            return channel;
        }

        public void CloseAndDisposeChannel(IModel? channel)
        {
            try
            {
                channel?.Close();
                channel?.Dispose();
            }
            catch { }
        }
    }
}