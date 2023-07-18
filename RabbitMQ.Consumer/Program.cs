using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Common;
using System.Text;

var manager = new RabbitMqManager();
var channel = manager.CreateConnection();

Console.WriteLine(" [*] Waiting for messages.");

void CreateConsumer(IModel? channel)
{
    var consumer = new EventingBasicConsumer(channel);

    consumer.Shutdown += (o, e) =>
    {
        Console.WriteLine("Error with RabbitMQ: {0}", e.Cause);
        while (channel == null || channel.IsClosed)
        {
            try
            {
                manager.CloseAndDisposeChannel(channel);
                channel = manager.CreateConnection();
                CreateConsumer(channel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Hata {ex.Message}. Retrying ...");
            }
        }
    };

    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($" [x] Received {message} - endpoint : {manager.CurrentEndPoint}");
    };
    channel.BasicConsume(queue: "test.quorrum.queue",
                         autoAck: true,
                         consumer: consumer);
}

CreateConsumer(channel);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();