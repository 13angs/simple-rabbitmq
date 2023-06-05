
using RabbitMQ.Client;

namespace Simple.RabbitMQ
{
    public interface IBasicConnection : IDisposable
    {
        IConnection GetConnection();
    }
}