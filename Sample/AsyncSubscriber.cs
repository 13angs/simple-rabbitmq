using System.Text.Json;
using RabbitMQ.Client;

namespace Simple.RabbitMQ
{
    public class AsyncSubscriber : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AsyncSubscriber> _logger;

        public AsyncSubscriber(IServiceProvider serviceProvider, ILogger<AsyncSubscriber> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            using(var scope = _serviceProvider.CreateScope())
            {
                IMessageSubscriber subscriber = scope.ServiceProvider.GetRequiredService<IMessageSubscriber>();
                subscriber.ConnectAsync(
                    "amqp://guest:guest@rabbitmq-management:5672",
                    "async_simple_rabbitmq_exchange",
                    "async_simple_rabbitmq_queue",
                    "async.simple.rabbitmq",
                    null,
                    ExchangeType.Topic
                );

                subscriber.SubscribeAsync(processMessage);
                return Task.CompletedTask;
            }
        }

        public async Task<bool> processMessage(string message, IDictionary<string, object> headers)
        {   
            _logger.LogInformation(message);
            _logger.LogInformation(headers["key"].ToString());
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}