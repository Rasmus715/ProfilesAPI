using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using Raven.DependencyInjection;

namespace AuthorizationAPI.Data;

public class RavenDbContext
{

    public RavenDbContext(IDocumentStore store)
    {
        EnsureDatabaseIsCreated(store);
        Store = store;
    }
    
    private void EnsureDatabaseIsCreated(IDocumentStore store)
    {
        try
        {
            store.Maintenance.ForDatabase(store.Database).Send(new GetStatisticsOperation());
        }
        catch (DatabaseDoesNotExistException)
        {
            store.Maintenance.Server.Send(
                new CreateDatabaseOperation(new DatabaseRecord(store.Database)));
        }
    }

    public IDocumentStore Store {get; set;}
}