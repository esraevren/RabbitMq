
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqps://zildxkuy:ZRZBuEUMuNRG7NM2eu8qU_JKyioBK8-X@jackal.rmq.cloudamqp.com/zildxkuy");

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        // channel.QueueDeclare("hello-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);


        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        var consumer = new EventingBasicConsumer(channel);

        channel.BasicConsume("hello-queue", autoAck: false, consumer);

        consumer.Received += (sender, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = System.Text.Encoding.UTF8.GetString(body);
            Console.WriteLine($"Message received: {message}");

            channel.BasicAck(eventArgs.DeliveryTag, multiple: false);

        };

    }
}