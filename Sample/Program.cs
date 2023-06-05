using RabbitMQ.Client;
using Simple.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<IBasicConnection>(new BasicConnection("amqp://guest:guest@rabbitmq-management:5672", true));
builder.Services.AddSingleton<IMessageSubscriber>(x => 
    new MessageSubscriber(
        x.GetRequiredService<IBasicConnection>(),
        "simple_rabbitmq_exchange",
        "simple_rabbitmq_queue",
        "simple.rabbitmq",
        ExchangeType.Fanout
        ));
builder.Services.AddScoped<IMessagePublisher>(x=>
    new MessagePublisher(
        x.GetRequiredService<IBasicConnection>(),
        "simple_rabbitmq_exchange",
        ExchangeType.Fanout
    ));
// builder.Services.AddHostedService<Subscriber>();
builder.Services.AddHostedService<AsyncSubscriber>();

var app = builder.Build();

app.MapGet("/{message}", (string message, IMessagePublisher publisher) => {
    Dictionary<string, object> headers = new Dictionary<string, object>();
    headers.Add("name", "sync sub");

    publisher.Publish(message, "simple.rabbitmq", headers);
    return $"Publisher: {message}";
});

app.MapGet("/async/{message}", (string message, IMessagePublisher publisher) => {
    Dictionary<string, object> headers = new Dictionary<string, object>();
    headers.Add("name", "async sub");

    publisher.Publish(message, "async.simple.rabbitmq", headers);
    return $"Async Publisher: {message}";
});

app.Run();
