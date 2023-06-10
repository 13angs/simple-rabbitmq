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

// Register message publisher
builder.Services.AddScoped<IMessagePublisher>(x =>
    new MessagePublisher(
        x.GetRequiredService<IBasicConnection>(),
        "simple_rabbitmq_exchange", // exhange name
        ExchangeType.Fanout // exchange type
    ));

// Register the subscriber as a hosted service
builder.Services.AddHostedService<AsyncSubscriber>(); // consume/subscribe synchronously
// builder.Services.AddHostedService<AsyncSubscriber>(); // consume/subscribe asynchronously

var app = builder.Build();

// Handle GET request with a message parameter
// app.MapPost("/user", async ([FromBody] object body, IMessagePublisher publisher) => {
//     await Task.Yield();
//     Dictionary<string, object> headers = new Dictionary<string, object>();
//     headers.Add("name", "sync sub");

//     // Publish message to RabbitMQ
//     string message = JsonSerializer.Serialize(body);
//     publisher.Publish(message, "post.user._", headers);
//     return;
// });

int counter = 0;
// Handle GET request with an "async" prefix and a message parameter
app.MapPut("/user/{id}", ([FromRoute] long id, [FromBody] object body, IMessagePublisher publisher) => {
    Dictionary<string, object> headers = new Dictionary<string, object>();
    headers.Add("name", "async sub");
    counter+=1;
    headers.Add("count", counter);

    // Publish message to RabbitMQ
    string strBody = JsonSerializer.Serialize(body);
    publisher.Publish(strBody, "put.user.update_name", headers);
    return $"Async Publisher: {id}";
});

app.Run();
