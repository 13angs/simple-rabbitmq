using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Simple.RabbitMQ
{
    public class PlainSubscriber
    {
        private readonly IBasicConnection _basicConnection;
        private readonly string _exchange;
        private readonly string _queue;
        private readonly IModel _model;
        private bool _disposed;
        private readonly bool _autoAck;

        /*
         * Constructor for the PlainSubscriber class.
         *
         * @param basicConnection: The basic RabbitMQ connection used for subscribing to messages.
         * @param exchange: The name of the exchange to bind the queue to.
         * @param queue: The name of the queue to subscribe to.
         * @param routingKey: The routing key for binding the queue to the exchange.
         * @param exchangeType: The type of the exchange.
         * @param timeToLive: The time-to-live (TTL) value in milliseconds for the messages in the exchange (optional, default is 30000 milliseconds).
         * @param prefetchSize: The maximum number of messages that can be prefetched (optional, default is 10).
         */
        public PlainSubscriber(
            IBasicConnection basicConnection,
            string exchange,
            string queue,
            string? routingKey,
            string exchangeType,
            uint  prefetchSize = 0,
            ushort prefetchCount = 10,
            bool global = false,
            bool autoAck=true,
            int timeToLive = 30000)
        {
            _basicConnection = basicConnection;
            _exchange = exchange;
            _queue = queue;
            _autoAck = autoAck;
            var connection = _basicConnection.GetConnection();
            _model = connection.CreateModel();
            _model.BasicQos(prefetchSize, prefetchCount, global);
            var ttl = new Dictionary<string, object>
            {
                {"x-message-ttl", timeToLive }
            };
            _model.ExchangeDeclare(_exchange, exchangeType, arguments: ttl);
            _model.QueueDeclare(_queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            _model.QueueBind(_queue, _exchange, routingKey);
        }

        /*
         * Subscribes to messages and executes the provided callback function.
         *
         * @param callback: The callback function that handles the received message.
         */
        public void Subscribe(Func<string, IDictionary<string, object>, string, bool> callback)
        {
            var consumer = new EventingBasicConsumer(_model);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                bool success = callback.Invoke(message, e.BasicProperties.Headers, e.RoutingKey);
                if (!_autoAck && success)
                {
                    Console.WriteLine("Message acknowledged!");
                    _model.BasicAck(e.DeliveryTag, true);
                }
            };

            _model.BasicConsume(_queue, _autoAck, consumer);
        }

        /*
         * Asynchronously subscribes to messages and executes the provided callback function.
         *
         * @param callback: The async callback function that handles the received message.
         */
        public void SubscribeAsync(Func<string, IDictionary<string, object>, string, Task<bool>> callback)
        {
            var consumer = new AsyncEventingBasicConsumer(_model);
            consumer.Received += async (sender, e) =>
            {
                await Task.Yield();
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                bool success = await callback.Invoke(message, e.BasicProperties.Headers, e.RoutingKey);
                if (!_autoAck && success)
                {
                    _model.BasicAck(e.DeliveryTag, true);
                }
            };

            _model.BasicConsume(_queue, _autoAck, consumer);
        }

        /*
         * Disposes of the PlainSubscriber object.
         */
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /*
         * Disposes of the PlainSubscriber object.
         *
         * @param disposing: A flag indicating whether the object is being disposed.
         */
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _model?.Close();

            _disposed = true;
        }
    }
}
