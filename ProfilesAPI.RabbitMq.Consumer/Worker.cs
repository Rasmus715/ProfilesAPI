using System.Text;
using CommunicationModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProfilesAPI.RabbitMq.Consumer.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ProfilesAPI.RabbitMq.Consumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IDocumentStore _documentStore;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public Worker(ILogger<Worker> logger, IOptions<RavenSettings> ravenSettings, IOptions<RabbitMqSettings> rabbitSettings)
    {
        _logger = logger;
        
        _documentStore = new DocumentStore
        {
            Urls = ravenSettings.Value.Urls,
            Database = ravenSettings.Value.DatabaseName
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
        
        var connectionFactory = new ConnectionFactory
        {
            HostName = rabbitSettings.Value.HostName,
            DispatchConsumersAsync = true
        };
        
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.ExchangeDeclare("ProfilesExchange", ExchangeType.Direct);
        
        _channel.QueueDeclare("PatientsQueue",
            false,
            false,
            false,
            null);  
        
        _channel.QueueBind("PatientsQueue",
            "ProfilesExchange",
            "Patients",
            null);
        
        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //Storing patient on message receiving. Overhaul login later. Maybe add dead letter queue & processing
        stoppingToken.ThrowIfCancellationRequested();
               
               var consumer = new AsyncEventingBasicConsumer(_channel);
               consumer.Received += async (_, ea) =>
               {
                   try
                   {
                       var obj = JsonSerializer.Deserialize<Patient>(Encoding.UTF8.GetString(ea.Body.ToArray()));
                       _logger.LogInformation("[x] Received {obj}", obj);
                       
                       using var session = _documentStore.OpenAsyncSession();
                       await session.StoreAsync(obj, stoppingToken);
                       await session.SaveChangesAsync(stoppingToken);
                       _channel.BasicAck(ea.DeliveryTag, false);
                   }
                   catch (JsonException)
                   {
                       _channel.BasicNack(ea.DeliveryTag, false, false);
                   }
               };
               
               _channel.BasicConsume("PatientsQueue",
                   false,
                   consumer);
               
               await Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        _connection.Close();
        _logger.LogInformation("RabbitMQ connection is closed.");
    }
}