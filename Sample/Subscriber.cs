namespace Simple.RabbitMQ
{
    public class Subscriber : IHostedService
    {
        private readonly ILogger<Subscriber> _logger;
        private readonly IMessageSubscriber _subscriber;

        public Subscriber(IMessageSubscriber subscriber, ILogger<Subscriber> logger)
        {
            _subscriber = subscriber;
            _logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(processMessage);
            return Task.CompletedTask;
        }

        public bool processMessage(string message, IDictionary<string, object> headers)
        {   
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