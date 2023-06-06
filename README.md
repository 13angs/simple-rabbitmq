# Simple RabbitMQ

## Usage:

- install the package

```bash
dotnet add package 13angs.Simple.RabbitMQ --version 0.1.1
```

- Register into DI container

```bash
// Register RabbitMQ connection
builder.Services.AddSingleton<IBasicConnection>(new BasicConnection("amqp://guest:guest@rabbitmq-management:5672"));

// Register message subscriber
builder.Services.AddSingleton<IMessageSubscriber>(x =>
    new MessageSubscriber(
        x.GetRequiredService<IBasicConnection>(),
        "simple_rabbitmq_exchange", // exhange name
        "simple_rabbitmq_queue", // queue name
        "simple.rabbitmq", // routing key
        ExchangeType.Fanout // exchange type
    ));

// Register message publisher
builder.Services.AddScoped<IMessagePublisher>(x =>
    new MessagePublisher(
        x.GetRequiredService<IBasicConnection>(),
        "simple_rabbitmq_exchange", // exhange name
        ExchangeType.Fanout // exchange type
    ));
```

- to subscribe asynchronously you need set DispatchConsumersAsync=true

```csharp
// Register RabbitMQ connection
builder.Services.AddSingleton<IBasicConnection>(new BasicConnection("amqp://guest:guest@rabbitmq-management:5672", true));

...
```

### Publish the message

- inject the IMessagePublisher using constructor injection

```csharp
app.MapGet("/{message}", (string message, IMessagePublisher publisher) => {
    publisher.Publish(message, "simple.rabbitmq", null);
    return $"Publisher: {message}";
});
```

### Consume/Subscribe to the message

- create a console app or create a background service to consume the message

```csharp
namespace Simple.RabbitMQ
{
    public class Subscriber : IHostedService
    {
        ...
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(processMessage);
            return Task.CompletedTask;
        }

        public bool processMessage(string message, IDictionary<string, object> headers, string routingKey)
        {   
            _logger.LogInformation("Routing key: " + routingKey);
            return true;
        }
        ...
    }
}
```
- For more detail refer to [this](https://github.com/13angs/simple-rabbitmq/blob/main/Sample/Subscriber.cs)
- And [this](https://github.com/13angs/simple-rabbitmq/blob/main/Sample/AsyncSubscriber.cs) for consume the message asynchronously

## Sample demo:

- cd intro Sample/ 
- Run the RabbitMQ server using docker compose

```bash
cd Sample/ && \
docker compose up -d
```

- run the project

```bash
dotnet run
```

- navigate to http://localhost:5010/<any_message_here>

- check the Console you will the message similar to this

```bash
info: Simple.RabbitMQ.Subscriber[0]
      Routing key: put.user.update_name
      Message: {"name":"don","action":"update_name"}
      Method: put
      Context: user
      Action: update_name
```
