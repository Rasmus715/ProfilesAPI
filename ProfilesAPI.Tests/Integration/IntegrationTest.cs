using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using ProfilesAPI.Data;
using ProfilesAPI.RabbitMq;
using Raven.Client.Documents.Session;
using Raven.TestDriver;

namespace ProfilesAPI.UnitTests.Integration;

public class IntegrationTest
{
    protected readonly HttpClient HttpClient;
    public class RavenEmbeddedContext : RavenTestDriver, IRavenContext
    {
        static RavenEmbeddedContext()
        {
            ConfigureServer(new TestServerOptions
            {
                DataDirectory = "./RavenDbTest/",
            });
        }
        public IAsyncDocumentSession GetSession()
        { 
            return GetDocumentStore().OpenAsyncSession();
        }
    }
    
    public class DummyRabbitMqService : IRabbitMqService
    {
        public void SendMessage(object obj) { }
    }
    
    protected IntegrationTest()
    {
        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll(typeof(IRavenContext));
                    services.RemoveAll(typeof(IRabbitMqService));
                    
                    services.AddSingleton<IRavenContext, RavenEmbeddedContext>();
                    services.AddSingleton<IRabbitMqService, DummyRabbitMqService>();
                });
            });

        HttpClient = appFactory.CreateClient();
    }
}