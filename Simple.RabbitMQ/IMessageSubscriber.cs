using RabbitMQ.Client;

namespace Simple.RabbitMQ
{
    public interface IMessageSubscriber
    {
        public void Subscribe(Func<string, IDictionary<string, object>, bool> callback);
        public void Connect(
                            string hostName,
                            string exchange, 
                            string queue, 
                            string routeKey, 
                            IDictionary<string, object>? headers, 
                            string exchangeType = ExchangeType.Fanout
                                );
        public void Disconnect();
    }
}