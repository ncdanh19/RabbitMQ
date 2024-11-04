using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    HostName = "localhost"
};

var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.QueueDeclare("product", durable: false, exclusive: false, autoDelete: false, arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = System.Text.Encoding.UTF8.GetString(body);

    var messageContent = JsonConvert.DeserializeObject<MessageContent>(message);

    if (messageContent == null)
    {
        Console.WriteLine("Received an invalid message format.");
        return;
    }

    Console.WriteLine($"[Received {messageContent.MessageType} Message]");
    Console.WriteLine($"Content: {messageContent.Content.ToString(Formatting.Indented)}");
};

channel.BasicConsume(queue: "product", autoAck: true, consumer: consumer);
Console.WriteLine("Waiting for messages...");
Console.ReadLine();

public class MessageContent
{
    public string MessageType { get; set; }
    public JToken Content { get; set; }
}
