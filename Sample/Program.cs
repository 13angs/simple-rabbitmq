using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
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
        ExchangeType.Fanout, // exchange type
        prefetchCount: 1,
        autoAck: false
    ));
// Register second message subscriber
builder.Services.AddSingleton<IOrderSubscriber>(x =>
    new OrderSubscriber(
        x.GetRequiredService<IBasicConnection>(),
        "order_message_exchange", // exhange name
        "order_message_queue", // queue name
        "order.message", // routing key
        ExchangeType.Fanout, // exchange type
        prefetchCount: 1,
        autoAck: false
    ));

// Register message publisher
builder.Services.AddScoped<IMessagePublisher>(x =>
    new MessagePublisher(
        x.GetRequiredService<IBasicConnection>(),
        "simple_rabbitmq_exchange", // exhange name
        ExchangeType.Fanout // exchange type
    ));
// Register message publisher
builder.Services.AddScoped<IOrderPublisher>(x =>
    new OrderPublisher(
        x.GetRequiredService<IBasicConnection>(),
        "order_message_exchange", // exhange name
        ExchangeType.Fanout // exchange type
    ));

// Register the subscriber as a hosted service
builder.Services.AddHostedService<SimpleMessageSubscriber>(); // consume/subscribe synchronously
builder.Services.AddHostedService<OrderMessageSubscriber>(); // consume/subscribe synchronously
// builder.Services.AddHostedService<AsyncSubscriber>(); // consume/subscribe asynchronously

var app = builder.Build();

int simpleCounter = 0;
// Handle GET request with an "async" prefix and a message parameter
app.MapPut("/simple/{id}", ([FromRoute] long id, [FromBody] object body, IMessagePublisher publisher, ILogger<Program> logger) => {
    Dictionary<string, object> headers = new Dictionary<string, object>();
    headers.Add("name", "simple pub");
    simpleCounter+=1;
    headers.Add("count", simpleCounter);

    // Publish message to RabbitMQ
    string strBody = JsonSerializer.Serialize(body);
    try{
        publisher.Publish(strBody, "put.simple.update_name", headers);

    }finally{
        publisher.Dispose();
        logger.LogInformation("Publisher disposed!");
    }
    return $"Simple Publisher: {id}";
});

int orderCounter = 0;
// Handle GET request with an "async" prefix and a message parameter
app.MapPut("/order/{id}", ([FromRoute] long id, [FromBody] object body, IOrderPublisher publisher, ILogger<Program> logger) => {
    Dictionary<string, object> headers = new Dictionary<string, object>();
    headers.Add("name", "order pub");
    orderCounter+=1;
    headers.Add("count", orderCounter);

    // Publish message to RabbitMQ
    string strBody = JsonSerializer.Serialize(body);
    try{
        publisher.Publish(strBody, "put.order.update_name", headers);
    }finally{
        publisher.Dispose();
        logger.LogInformation("Publisher disposed!");
    }
    return $"Order Publisher: {id}";
});

app.Run();
