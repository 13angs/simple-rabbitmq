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

        public async Task<bool> processMessage(string message, IDictionary<string, object> headers)
        {   
            await Task.Yield();
            _logger.LogInformation("Name: ");
            _logger.LogInformation("Method: ");
            _logger.LogInformation("Message: " + message);
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}