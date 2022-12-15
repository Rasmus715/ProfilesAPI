// See https://aka.ms/new-console-template for more information

using System.Text;
using RabbitMQ.Client;

Console.WriteLine("Hello, World!");

var factory = new ConnectionFactory
{
    HostName = "localhost"
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.QueueDeclare("hello",
    false,
    false,
    false,
    null);

const string message = "Hello World!";

var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(exchange: "",
     "hello",
     null,
     body);
Console.WriteLine(" [x] Sent {0}", message);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();