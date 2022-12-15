// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using CommunicationModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;


using IDocumentStore store = new DocumentStore
{
    Urls = new[]
    {
        "http://127.0.0.1:8080/"
    },
    Database = "LogsDb"
};

store.Initialize();

try
{
    store.Maintenance.ForDatabase(store.Database).Send(new GetStatisticsOperation());
}
catch (DatabaseDoesNotExistException)
{
    store.Maintenance.Server.Send(
        new CreateDatabaseOperation(new DatabaseRecord(store.Database)));
}

var factory = new ConnectionFactory
{
    HostName = "localhost"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.QueueDeclare("LogsQueue",
    false,
    false,
    false,
    null);

Console.WriteLine("Listening for messages...");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (_, ea) =>
{
    var body = ea.Body.Span;
    var obj = JsonSerializer.Deserialize<RabbitLog>(body);
    Console.WriteLine($"[x] Received {obj}");

    // ReSharper disable once AccessToDisposedClosure
    using var session = store.OpenSession();
    
    session.Store(obj);
    session.SaveChanges();
};
channel.BasicConsume("LogsQueue",
    true,
    consumer);

Process.GetCurrentProcess().WaitForExit();
