using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;

namespace RabbitMqWeb.WatermarkApp.Services
{
    public class RabbitMqClientService : IDisposable
    {

        public RabbitMQ.Client.IModel Channel => channel;
        public IConnection Connection => connection;
        private readonly ConnectionFactory connectionFactory;
        private IConnection connection;
        private RabbitMQ.Client.IModel channel;
        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWatermark = "watermark-route-image";
        public static string QueueName = "queue-watermark-image";

        private readonly ILogger<RabbitMqClientService> logger;

        public RabbitMqClientService(ConnectionFactory connectionFactory, ILogger<RabbitMqClientService> logger)
        {
            this.connectionFactory = connectionFactory;
            this.logger = logger;
        }

        public RabbitMQ.Client.IModel Connect()
        {
            connection = connectionFactory.CreateConnection();

            if (channel != null && channel.IsOpen)
            {
                return channel;
            }

            channel = connection.CreateModel();
            channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, true, false, null);
            channel.QueueDeclare(QueueName, true, false, false, null);
            channel.QueueBind(QueueName, ExchangeName, RoutingWatermark, null);
            logger.LogInformation("RabbitMQ connected and channel created.");
            return channel;
        }

        public void Dispose()
        {
            channel?.Close();
            channel?.Dispose();

            connection?.Close();
            connection?.Dispose();

            logger.LogInformation("RabbitMQ connection and channel disposed.");
        }
    }
}
