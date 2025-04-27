using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqWeb.WatermarkApp.Services;
using System.Drawing;
using System.Text.Json;

namespace RabbitMqWeb.WatermarkApp.BackgroundServices
{
    public class ImageWatermarkBackgroundService : BackgroundService
    {
        private readonly RabbitMqClientService rabbitMqClientService;
        private readonly ILogger<ImageWatermarkBackgroundService> logger;
        private IModel channel;

        public ImageWatermarkBackgroundService(
            RabbitMqClientService rabbitMqClientService,
            ILogger<ImageWatermarkBackgroundService> logger)
        {
            this.rabbitMqClientService = rabbitMqClientService;
            this.logger = logger;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            channel = rabbitMqClientService.Connect();
            channel.BasicQos(0, 1, false);

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(RabbitMqClientService.QueueName, false, consumer);

            consumer.Received += async (sender, e) => await Consumer_Received(sender, e);

            return Task.CompletedTask;
        }

        private async Task Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var imageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>(e.Body.ToArray());

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", imageCreatedEvent.ImageName);

                using var image = Image.FromFile(path);

                using var graphics = Graphics.FromImage(image);

                var font = new Font("Arial", 40, FontStyle.Bold, GraphicsUnit.Pixel);

                var textSize = graphics.MeasureString("www.mysite.com", font);

                var color = Color.FromArgb(128, 255, 255, 255);
                var brush = new SolidBrush(color);

                var position = new Point(image.Width - ((int)textSize.Width + 30), image.Height - ((int)textSize.Height + 30));

                graphics.DrawString("www.mysite.com", font, brush, position);

                image.Save("wwwroot/images/watermarks/" + imageCreatedEvent.ImageName);

                image.Dispose();
                graphics.Dispose();

                channel.BasicAck(e.DeliveryTag, false);

            }
            catch (Exception ex) 
            {
                logger.LogError(ex, "Error processing image watermarking");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
