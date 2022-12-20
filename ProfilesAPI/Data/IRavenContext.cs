using Microsoft.Extensions.Options;
using ProfilesAPI.Models;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace ProfilesAPI.Data;

public interface IRavenContext
{
    public IDocumentStore Store { get; set; }
}
public class RavenContext : IRavenContext
{
    public IDocumentStore Store { get; set; }

    private readonly PersistenceSettings _persistenceSettings;

    public RavenContext(IOptionsMonitor<PersistenceSettings> settings)
    {   
        _persistenceSettings = settings.CurrentValue;
        
        Store = new DocumentStore
        {
            Database = _persistenceSettings.DatabaseName,
            Urls = _persistenceSettings.Urls
        };
        
        Store.Initialize();

        EnsureDatabaseIsCreated();
    }

    private void EnsureDatabaseIsCreated()
    {
        try
        {
            Store.Maintenance.ForDatabase(_persistenceSettings.DatabaseName).Send(new GetStatisticsOperation());
        }
        catch (DatabaseDoesNotExistException)
        {
            Store.Maintenance.Server.Send(
                new CreateDatabaseOperation(new DatabaseRecord(_persistenceSettings.DatabaseName)));
        }
    }
    
}
