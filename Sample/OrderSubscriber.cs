namespace Simple.RabbitMQ
{
    public class OrderSubscriber : PlainSubscriber, IOrderSubscriber
    {
        public OrderSubscriber(
            IBasicConnection basicConnection, string exchange,string queue, string? routingKey, string exchangeType, uint prefetchSize = 0, ushort prefetchCount = 10, bool global = false, bool autoAck = true, int timeToLive = 30000) : base(
                basicConnection, exchange, queue, routingKey, exchangeType, prefetchSize = 0, prefetchCount = 10, global = false, autoAck = true, timeToLive = 30000){}
    }
}