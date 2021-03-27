namespace OrderConsumer.Interfaces
{
    public interface IProducer
    {
        void PublishMessage(
            string routingKey,
            object message
        );
    }
}
