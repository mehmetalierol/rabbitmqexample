using RabbitMQ.Client;
using RabbitMQ.Common;
using System.Text;

var manager = new RabbitMqManager();
void StartSending()
{
    var channel = manager.CreateConnection();

    for (int i = 0; i < 10000; i++)
    {
        var message = "Message " + i.ToString();
        try
        {
            SendMessageToQueue(channel, message);
        }
        catch
        {
            while (channel == null || channel.IsClosed)
            {
                try
                {
                    manager.CloseAndDisposeChannel(channel);
                    channel = manager.CreateConnection();
                    SendMessageToQueue(channel, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Hata {ex.Message}. Retrying ...");
                }
            }
        }
    }
}

void SendMessageToQueue(IModel? channel, string message)
{
    var body = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(exchange: string.Empty,
                     routingKey: "test.quorrum.queue",
                     basicProperties: null,
                     body: body);
    Console.WriteLine($" [x] Sent {message} - endpoint : {manager.CurrentEndPoint}");
    Thread.Sleep(200);
}

StartSending();

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();