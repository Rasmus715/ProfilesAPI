using CommunicationModels;
using LoggingAPI.ViewModels;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace LoggingAPI.Services;

public interface ILoggingService
{
    Task<PagingList<RabbitLog>> Get(int page, int pageSize);
}

public class LoggingService : ILoggingService
{
    private readonly IAsyncDocumentSession _dbSession;

    public LoggingService(IAsyncDocumentSession dbSession)
    {
        _dbSession = dbSession;
    }

    public async Task<PagingList<RabbitLog>> Get(int page, int pageSize)
    {
        var logs =  await _dbSession
            .Query<RabbitLog>()
            .OrderBy(log => log.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalItems = await _dbSession
            .Query<RabbitLog>().CountAsync();

        return new PagingList<RabbitLog>
        {
            TotalElements = totalItems,
            TotalPages = (int) Math.Ceiling(totalItems / (double) pageSize),
            Items = logs
        };
    }
}