using RabbitMQ.Client;
using Simple.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IMessagePublisher, MessagePublisher>();
builder.Services.AddScoped<IMessageSubscriber, MessageSubscriber>();
builder.Services.AddHostedService<Subscriber>();

var app = builder.Build();

app.MapGet("/{message}", (string message, IMessagePublisher publisher) => {
    publisher.Connect(
        "amqp://guest:guest@rabbitmq-management:5672",
        "simple_rabbitmq_exchange",
        ExchangeType.Topic
    );

    publisher.Publish(message, "simple.rabbitmq", null);
    return $"Publisher: {message}";
});

app.Run();
