using RabbitMQ.Client;

namespace Simple.RabbitMQ
{
    public interface IMessagePublisher
    {
        public void Publish(string message, string? routeKey, IDictionary<string, object>? headers);
        public void Connect(
                            string hostName,
                            string exchange, 
                            string exchangeType = ExchangeType.Fanout
                                );
        public void Disconnect();
  }
}