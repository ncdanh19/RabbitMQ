namespace RabbitMQ.RabbitMQ
{
    public interface IRabbitMQProducer
    {
        public void SendProductMessage<T>(T message, string messageType);
    }
}
