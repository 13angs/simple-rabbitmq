using RabbitMQ.Client;
using System.Text;

namespace Simple.RabbitMQ
{
    public class PlainPublisher
    {
        private readonly IBasicConnection _basicConnection;
        private readonly string _exchange;
        private readonly IModel _model;
        private bool _disposed;

        /*
         * Constructor for the PlainPublisher class.
         *
         * @param basicConnection: The basic RabbitMQ connection used for publishing messages.
         * @param exchange: The name of the exchange to publish messages to.
         * @param exchangeType: The type of the exchange.
         * @param timeToLive: The time-to-live (TTL) value in milliseconds for the published messages (optional, default is 30000 milliseconds).
         */
        public PlainPublisher(IBasicConnection basicConnection, string exchange, string exchangeType, int timeToLive = 30000)
        {
            _basicConnection = basicConnection;
            _exchange = exchange;
            var connection = _basicConnection.GetConnection();
            _model = connection.CreateModel();
            var ttl = new Dictionary<string, object>
            {
                {"x-message-ttl", timeToLive }
            };
            _model.ExchangeDeclare(_exchange, exchangeType, arguments: ttl);
        }

        /*
         * Publishes a message to the specified exchange.
         *
         * @param message: The message to be published.
         * @param routeKey: The routing key for the message (optional).
         * @param headers: Additional headers for the message (optional).
         * @param timeToLive: The time-to-live (TTL) value in milliseconds for the published message (optional, default is 30000 milliseconds).
         */
        public void Publish(string message, string? routeKey, IDictionary<string, object>? headers, int? timeToLive = 30000)
        {
            var body = Encoding.UTF8.GetBytes(message);
            var properties = _model.CreateBasicProperties();
            properties.Persistent = true;
            properties.Headers = headers;
            properties.Expiration = timeToLive.ToString();

            _model.BasicPublish(_exchange, routeKey, properties, body);
        }

        /*
         * Disposes of the PlainPublisher object.
         */
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /*
         * Disposes of the PlainPublisher object.
         *
         * @param disposed: A flag indicating whether the object is being disposed.
         */
        protected virtual void Dispose(bool disposed)
        {
            if (_disposed)
                return;

            if (disposed)
                _model?.Close();

            _disposed = true;
        }
    }
}
