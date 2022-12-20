using System.Text;
using CommunicationModels;
using MassTransit;
using Newtonsoft.Json;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace RabbitMqHostedService;

internal class Consumer : IConsumer<RabbitLog>
{
    private IDocumentStore _documentStore = null!;

    public async Task Consume(ConsumeContext<RabbitLog> context)
    {
        _documentStore = new DocumentStore
        {
            Urls = new[]
            {
                "http://127.0.0.1:8080/"
            },
            Database = "LogsDb"
        };

        _documentStore.Initialize();

        try
        {
            _documentStore.Maintenance.ForDatabase(_documentStore.Database).Send(new GetStatisticsOperation());
        }
        catch (DatabaseDoesNotExistException)
        {
            _documentStore.Maintenance.Server.Send(
                new CreateDatabaseOperation(new DatabaseRecord(_documentStore.Database)));
        }
        
        Console.WriteLine($"[x] Received {context.Message}");
        
        using var session = _documentStore.OpenAsyncSession();
        await session.StoreAsync(context.Message);
        await session.SaveChangesAsync();
     

        var jsonMessage = JsonConvert.SerializeObject(context.Message);
        Console.WriteLine($"OrderCreated message: {jsonMessage}");
    }
}