using System.Text;
using CommunicationModels;
using System.Text.Json;
using MassTransit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace RabbitMqHostedService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private ConnectionFactory _connectionFactory = null!;
    private IConnection _connection = null!;
    private IModel _channel = null!;
    private readonly IDocumentStore _documentStore;

    public Worker(ILogger<Worker> logger)
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

        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken ctx)
    {
        // var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        // {
        //     cfg.ReceiveEndpoint("order-created-event", e =>
        //     {
        //         e.Consumer<Consumer>();
        //         e.BindDeadLetterQueue("DeadLetterExchange", "MTDeadLetterQueue", config =>
        //         {
        //             config.SetExchangeArgument("x-message-ttl", TimeSpan.FromSeconds(60));
        //         });
        //     });
        // });
        //
        // await busControl.StartAsync(ctx);

        await StartAsyncRabbitMq(ctx);
    }

    private Task StartAsyncRabbitMq(CancellationToken cancellationToken)
    {
        _connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            DispatchConsumersAsync = true
        };
        
        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("DeadLetterExchange", ExchangeType.Fanout);
        
        _channel.QueueDeclare("DeadLetterQueue",
            false,
            false,
            false,
            null);  
        
        _channel.QueueBind("DeadLetterQueue",
            "DeadLetterExchange",
            string.Empty,
            null);

        _channel.ExchangeDeclare("LogsExchangeHeaders", ExchangeType.Headers);
        
        var logsArguments = new Dictionary<string, object>
        {
            {
                "message-ttl" , 10000
            }, 
            {
                "x-dead-letter-exchange", "DeadLetterExchange"
            }
        };
        
        _channel.QueueDeclare("LogsQueue",
            false,
            false,
            false,
            logsArguments);  
        
        
        var headers = new Dictionary<string, object>
        {
            {
                "type", "logs"
            }
        };

        _channel.QueueBind("LogsQueue",
            "LogsExchangeHeaders",
            string.Empty,
            headers);
        
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            try
            {
                var obj = JsonSerializer.Deserialize<RabbitLog>(Encoding.UTF8.GetString(ea.Body.ToArray()));
                Console.WriteLine($"[x] Received {obj}");

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
        
        _channel.BasicConsume("LogsQueue",
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