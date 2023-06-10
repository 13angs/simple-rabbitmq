namespace Simple.RabbitMQ
{
    public class OrderPublisher : PlainPublisher, IOrderPublisher
    {
        public OrderPublisher(
            IBasicConnection basicConnection, string exchange, string exchangeType, int timeToLive = 30000) : base(
                basicConnection, exchange, exchangeType, timeToLive = 30000){}
    }
}