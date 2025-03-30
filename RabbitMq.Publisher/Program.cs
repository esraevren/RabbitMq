// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using System.Threading.Tasks;
using System;

internal class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqps://zildxkuy:ZRZBuEUMuNRG7NM2eu8qU_JKyioBK8-X@jackal.rmq.cloudamqp.com/zildxkuy");

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare("hello-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

        string message = "Hello World!";
        var body = System.Text.Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "", routingKey: "hello-queue", basicProperties: null, body: body);

        Console.WriteLine("Message sent!");
        Console.ReadLine();
    }
}