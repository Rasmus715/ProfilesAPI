using ProfilesAPI.RabbitMq.Consumer;
using ProfilesAPI.RabbitMq.Consumer.Settings;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.Configure<RavenSettings>(configuration.GetSection(nameof(RavenSettings)));
        services.Configure<RabbitMqSettings>(configuration.GetSection(nameof(RabbitMqSettings)));
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();