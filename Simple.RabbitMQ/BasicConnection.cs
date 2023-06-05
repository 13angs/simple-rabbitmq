using RabbitMQ.Client;

namespace Simple.RabbitMQ
{
    public class BasicConnection : IBasicConnection
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private bool _disposed;

        /*
         * Constructor for the BasicConnection class.
         *
         * @param url: The URL used to establish the RabbitMQ connection.
         */
        public BasicConnection(string url, bool isAsync=false)
        {
            _factory = new ConnectionFactory
            {
                Uri = new Uri(url),
                DispatchConsumersAsync=isAsync,
            };
            _connection = _factory.CreateConnection();
        }

        /*
         * Retrieves the RabbitMQ connection.
         *
         * @return: The RabbitMQ connection.
         */
        public IConnection GetConnection()
        {
            return _connection;
        }

        /*
         * Disposes of the BasicConnection object.
         */
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /*
         * Disposes of the BasicConnection object.
         *
         * @param disposed: A flag indicating whether the object is being disposed.
         */
        protected virtual void Dispose(bool disposed)
        {
            if (_disposed)
                return;

            if (disposed)
                _connection?.Close();

            _disposed = true;
        }
    }
}
