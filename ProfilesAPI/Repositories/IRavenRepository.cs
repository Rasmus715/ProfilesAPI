using ProfilesAPI.Data;
using ProfilesAPI.Models;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace ProfilesAPI.Repositories;

public interface IRavenRepository<T> where T : class, IEntity
{
    Task<T> Create(T obj);
    Task<T> Update(T obj);
    Task Delete(Guid id);
    Task<T> Get(Guid id);
    Task<List<T>> Get();
}

public class RavenRepository<T> : IRavenRepository<T> where T : class, IEntity
{
    private readonly IAsyncDocumentSession _documentSession;

    public RavenRepository(IRavenContext context)
    {
        _documentSession = context.GetSession();
    }

    public async Task<T> Create(T obj)
    {
        obj.Id = Guid.NewGuid().ToString();
        await _documentSession.StoreAsync(obj);
        await _documentSession.SaveChangesAsync();
        return obj;
    }

    public async Task<T> Update(T obj)
    {
        var element = await _documentSession.LoadAsync<T>(obj.Id);

        if (element is null)
            throw new ArgumentNullException(nameof(element), "There is no such element in database");

        _documentSession.Delete(element);
        await _documentSession.SaveChangesAsync();
        
        await _documentSession.StoreAsync(obj);
        await _documentSession.SaveChangesAsync();
        
        // element = _mapper.Map<T>(obj);
        //
        // Console.WriteLine(_documentSession.Advanced.HasChanged(element));
        
        //Entity is being tracked, so there is no need to push updates back
        return obj;
    }

    public async Task Delete(Guid id)
    {
        var element = await _documentSession.LoadAsync<T>(id.ToString());
        
        if (element is null)
            throw new ArgumentNullException(nameof(id), "Element is not found");
        
        _documentSession.Delete(element);
        await _documentSession.SaveChangesAsync();
    }

    public async Task<T> Get(Guid id)
    {
        var element = await _documentSession.LoadAsync<T>(id.ToString());
        return element;
    }

    public async Task<List<T>> Get()
    {
        var elements = await _documentSession.Query<T>().ToListAsync();
        return elements;
    }
}