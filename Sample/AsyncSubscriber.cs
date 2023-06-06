namespace Simple.RabbitMQ
{
    public class AsyncSubscriber : IHostedService
    {
        private readonly ILogger<AsyncSubscriber> _logger;
        private readonly IMessageSubscriber _subscriber;

        public AsyncSubscriber(ILogger<AsyncSubscriber> logger, IMessageSubscriber subscriber)
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
            _logger.LogInformation("Routing key: " + routingKey + "\n" + 
                                    "Message: " + message + "\n" + 
                                    "Method: " + props[0] + "\n" + 
                                    "Context: " + props[1]  + "\n" + 
                                    "Action: " + props[2]
                                    );
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}