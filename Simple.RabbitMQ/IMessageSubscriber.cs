namespace Simple.RabbitMQ
{
    public interface IMessageSubscriber : IDisposable
    {
        public void Subscribe(Func<string, IDictionary<string, object>, string, bool> callback);
        public void SubscribeAsync(Func<string, IDictionary<string, object>, string, Task<bool>> callback);
    }
}