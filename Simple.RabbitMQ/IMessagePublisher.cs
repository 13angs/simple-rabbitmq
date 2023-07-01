namespace Simple.RabbitMQ
{
    public interface IMessagePublisher : IDisposable
    {
        public void Publish(string message, string? routeKey, IDictionary<string, object>? headers, int? ttl = 30000);
  }
}