namespace Core.Constants
{
    public static class RabbitMqConstants
    {
        public const string DefaultExchange = "default.exchange";
        public const string DefaultOrderQueue = "order.persistence.queue";
        public const string DefaultOrderRoutingKey = "order.persistance.routing.key";
    }
}
