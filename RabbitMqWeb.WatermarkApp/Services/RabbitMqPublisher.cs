using RabbitMQ.Client;

namespace RabbitMqWeb.WatermarkApp.Services
{
    public class RabbitMqPublisher
    {
        private readonly RabbitMqClientService rabbitMqClientService;
        private readonly ILogger<RabbitMqPublisher> logger;

        public RabbitMqPublisher(RabbitMqClientService rabbitMqClientService, ILogger<RabbitMqPublisher> logger)
        {
            this.rabbitMqClientService = rabbitMqClientService;
            this.logger = logger;
        }

        public void Publish(ProductImageCreatedEvent productImageCreatedEvent)
        {
            var channel = rabbitMqClientService.Connect();
            var bodyString = System.Text.Json.JsonSerializer.Serialize(productImageCreatedEvent);
            var bodyBytes = System.Text.Encoding.UTF8.GetBytes(bodyString);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(RabbitMqClientService.ExchangeName, RabbitMqClientService.RoutingWatermark, properties, bodyBytes);
        }
    }
}
