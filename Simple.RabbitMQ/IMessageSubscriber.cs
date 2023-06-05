namespace Simple.RabbitMQ
{
    public interface IMessageSubscriber
    {
        public void Subscribe(Func<string, IDictionary<string, object>, bool> callback);
        public void SubscribeAsync(Func<string, IDictionary<string, object>, Task<bool>> callback);
    }
}