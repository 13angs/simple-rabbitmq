using RabbitMQ.Client;
using Simple.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Register RabbitMQ connection
builder.Services.AddSingleton<IBasicConnection>(new BasicConnection("amqp://guest:guest@rabbitmq-management:5672", true));

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

// Register the subscriber as a hosted service
// builder.Services.AddHostedService<Subscriber>(); // consume/subscribe synchronously
builder.Services.AddHostedService<AsyncSubscriber>(); // consume/subscribe asynchronously

var app = builder.Build();

// Handle GET request with a message parameter
app.MapGet("/{message}", (string message, IMessagePublisher publisher) => {
    Dictionary<string, object> headers = new Dictionary<string, object>();
    headers.Add("name", "sync sub");

    // Publish message to RabbitMQ
    publisher.Publish(message, "simple.rabbitmq", headers);
    return $"Publisher: {message}";
});

// Handle GET request with an "async" prefix and a message parameter
app.MapGet("/async/{message}", (string message, IMessagePublisher publisher) => {
    Dictionary<string, object> headers = new Dictionary<string, object>();
    headers.Add("name", "async sub");

    // Publish message to RabbitMQ
    publisher.Publish(message, "async.simple.rabbitmq", headers);
    return $"Async Publisher: {message}";
});

app.Run();
