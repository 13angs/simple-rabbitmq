using System.Text;

namespace Simple.RabbitMQ
{
    public class SimpleMessageSubscriber : IHostedService
    {
        private readonly ILogger<SimpleMessageSubscriber> _logger;
        private readonly IMessageSubscriber _subscriber;

        public SimpleMessageSubscriber(ILogger<SimpleMessageSubscriber> logger, IMessageSubscriber subscriber)
        {
            _logger = logger;
            _subscriber = subscriber;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.SubscribeAsync(processMessage);
            return Task.CompletedTask;
        }

        public async Task<bool> processMessage(string message, IDictionary<string, object> headers, string routingKey)
        {   
            await Task.Yield();
            string[] props = routingKey.Split(".");
            
            byte[] bname = (byte[])headers["name"];
            string name = Encoding.UTF8.GetString(bname);

            _logger.LogInformation($"Routing key: {routingKey}\nHeader name: {name}\nHeader count: {headers["count"]}");
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}