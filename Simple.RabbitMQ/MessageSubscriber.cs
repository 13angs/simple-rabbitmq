using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Simple.RabbitMQ
{
    public class MessageSubscriber : IMessageSubscriber
    {
        private IConnection? connection;
        private readonly IConfiguration configuration;
        private IModel? channel;
        private string? exchange;
        private string? queue;
        private string? routeKey;
        private IDictionary<string, object>? headers;
        private string? exchangeType;

        public MessageSubscriber(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Subscribe(Func<string, IDictionary<string, object>, bool> callback)
        {
            channel.ExchangeDeclare(exchange, this.exchangeType);
            channel!.QueueDeclare(queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            channel.QueueBind(queue, exchange, routeKey);
            channel.BasicQos(0, 10, false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) => {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                bool success = callback.Invoke(message, e.BasicProperties.Headers);

                // if (success)
                // {
                //     channel.BasicAck(e.DeliveryTag, true);
                // }
            };

            channel.BasicConsume(queue, true, consumer);
        }
        public void SubscribeAsync(Func<string, IDictionary<string, object>, Task<bool>> callback)
        {
            channel.ExchangeDeclare(exchange, this.exchangeType);
            channel!.QueueDeclare(queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            channel.QueueBind(queue, exchange, routeKey);
            channel.BasicQos(0, 10, false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async(sender, e) => {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                bool success = await callback.Invoke(message, e.BasicProperties.Headers);
                if (success)
                {
                    channel.BasicAck(e.DeliveryTag, true);
                }
                await Task.Yield();
            };

            channel.BasicConsume(queue, true, consumer);
        }

        public void Connect(
                            string hostName,
                            string exchange, 
                            string queue, 
                            string routeKey, 
                            IDictionary<string, object>? headers, 
                            string exchangeType = ExchangeType.Fanout
                                )
        {
            var factory = new ConnectionFactory{
                Uri=new Uri(hostName),
            };
            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();
            this.exchange = exchange;
            this.queue = queue;
            this.routeKey = routeKey;
            this.headers = headers;
            this.exchangeType = exchangeType;
        }
        public void ConnectAsync(
                            string hostName,
                            string exchange, 
                            string queue, 
                            string routeKey, 
                            IDictionary<string, object>? headers, 
                            string exchangeType = ExchangeType.Fanout
                                )
        {
            var factory = new ConnectionFactory{
                Uri=new Uri(hostName),
                DispatchConsumersAsync=true,
            };
            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();
            this.exchange = exchange;
            this.queue = queue;
            this.routeKey = routeKey;
            this.headers = headers;
            this.exchangeType = exchangeType;
        }

        public void Disconnect()
        {
            this.channel!.Dispose();
            this.connection!.Dispose();
        }
    }
}