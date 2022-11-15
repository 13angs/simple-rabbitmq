namespace Simple.RabbitMQ
{
    public class Subscriber : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Subscriber> _logger;

        public Subscriber(IServiceProvider serviceProvider, ILogger<Subscriber> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            using(var scope = _serviceProvider.CreateScope())
            {
                IMessageSubscriber subscriber = scope.ServiceProvider.GetRequiredService<IMessageSubscriber>();
                subscriber.Connect(
                    "amqp://guest:guest@rabbitmq-management:5672",
                    "simple_rabbitmq_exchange",
                    "simple_rabbitmq_queue",
                    "simple.rabbitmq",
                    null
                );

                subscriber.Subscribe(processMessage);
                return Task.CompletedTask;
            }
        }

        public bool processMessage(string message, IDictionary<string, object> headers)
        {   
            _logger.LogInformation(message);
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}