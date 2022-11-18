# Simple RabbitMQ

## Usage:

- install the package

- Register into DI container

```bash
builder.Services.AddScoped<IMessagePublisher, MessagePublisher>();
builder.Services.AddScoped<IMessageSubscriber, MessageSubscriber>();
```

### Publisher

- inject the IMessagePublisher using constructor in injection

```csharp
app.MapGet("/{message}", (string message, IMessagePublisher publisher) => {
    publisher.Connect(
        "amqp://guest:guest@rabbitmq-management:5672",
        "simple_rabbitmq_exchange",
        ExchangeType.Fanout
    );

    publisher.Publish(message, "simple.rabbitmq", null);
    return $"Publisher: {message}";
});
```

### Subscriber

- create a console app or create a background to consume the message

```csharp
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
```

## Sample demo:

- cd intro Sample/ and run the RabbitMQ server using docker compose

```bash
cd Sample/ && \
docker compose up -d
```

- run the project

```bash
dotnet run
```

- navigate to http://localhost:5010/<any_message_here>

- check the Console if there any message printed there

```bash
info: Simple.RabbitMQ.Subscriber[0]
      <any_message_here>
```
