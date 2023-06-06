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

        public bool processMessage(string message, IDictionary<string, object> headers, string routingKey)
        {   
            string[] props = routingKey.Split(".");
            _logger.LogInformation("Routing key: " + routingKey);
            _logger.LogInformation("Message: " + message);
            _logger.LogInformation("Method: " + props[0]);
            _logger.LogInformation("Context: " + props[1]);
            _logger.LogInformation("Action: " + props[2]);
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}