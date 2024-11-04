using System;
using RabbitMQ.Client;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace RabbitMQ.RabbitMQ
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private readonly string _hostName = "localhost";

        public void SendProductMessage<T>(T message, string messageType)
        {
            var factory = new ConnectionFactory { HostName = _hostName };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            try
            {
                // Declare the main product queue
                channel.QueueDeclare(queue: "product", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var messageContent = new
                {
                    MessageType = messageType,
                    Content = message,
                    Timestamp = DateTime.UtcNow
                };

                var json = JsonConvert.SerializeObject(messageContent);
                var body = System.Text.Encoding.UTF8.GetBytes(json);

                // Publish message to the "product" queue
                channel.BasicPublish(exchange: "", routingKey: "product", basicProperties: null, body: body);

                Console.WriteLine($" [x] Sent {messageType} Product message");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [!] Failed to {messageType} Product: " + ex.Message);

                // Handle failure by sending to a 'failed-operations' queue
                SendFailedOperationMessage(ex.Message, message, messageType);
            }
        }

        private void SendFailedOperationMessage<T>(string error, T failedMessage, string messageType)
        {
            var factory = new ConnectionFactory { HostName = _hostName };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare a queue for failed operations
            channel.QueueDeclare(queue: "failed-operations", durable: false, exclusive: false, autoDelete: false, arguments: null);

            // Create a structured failure message with error details and operation type
            var failureMessage = new
            {
                Error = error,
                FailedMessageType = messageType,
                FailedMessage = failedMessage,
                Timestamp = DateTime.UtcNow
            };

            var json = JsonConvert.SerializeObject(failureMessage);
            var body = System.Text.Encoding.UTF8.GetBytes(json);

            // Publish the failure message to the "failed-operations" queue
            channel.BasicPublish(exchange: "", routingKey: "failed-operations", basicProperties: null, body: body);

            Console.WriteLine($" [x] Sent Failed {messageType} message to 'failed-operations' queue");
        }
    }
}