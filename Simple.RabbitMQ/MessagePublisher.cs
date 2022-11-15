using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Simple.RabbitMQ
{
    public class MessagePublisher : IMessagePublisher
    {
        private IConnection? connection;
        private readonly IConfiguration configuration;
        private IModel? channel;
        private string? exchange;
        private string? exchangeType;

        public MessagePublisher(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Publish(string message, string? routeKey, IDictionary<string, object>? headers)
        {
             var ttl = new Dictionary<string, object>
            {
                {"x-message-ttl", 30000}
            };
            channel.ExchangeDeclare(exchange, exchangeType, arguments: ttl);

            // while(true)
            // {
            //     count++;
            //     Thread.Sleep(1000);
            // }
            var body = Encoding.UTF8.GetBytes(message);
            channel!.BasicPublish(exchange, routeKey, null, body);
        }
        public void Connect(
                            string hostName,
                            string exchange, 
                            string exchangeType = ExchangeType.Fanout
                                )
        {
            var factory = new ConnectionFactory{
                Uri=new Uri(hostName)
            };
            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();
            this.exchange = exchange;
            this.exchangeType = exchangeType;
        }

        public void Disconnect()
        {
            this.channel!.Dispose();
            this.connection!.Dispose();
        }
  }
}