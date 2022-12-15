using System.Diagnostics;
using System.Text.Json;
using CommunicationModels;
using MediatR;
using ProfilesAPI.RabbitMq;

namespace ProfilesAPI.Behaviours;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IRabbitMqService _rabbitMqService;
    public LoggingBehavior(IRabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        //_rabbitMqService.SendMessage($"ProfilesAPI handling {typeof(TRequest).Name} with body \n {JsonSerializer.Serialize(request)}");
        //_logger.LogInformation($"ProfilesAPI handling {typeof(TRequest).Name} with body: {JsonSerializer.Serialize(request)}");
        
        var requestName = request.GetType().Name;
        var requestGuid = Guid.NewGuid().ToString();
        //var stopwatch = Stopwatch.StartNew();
        TResponse response;
        
        //var requestNameWithGuid = $"{requestName} [{requestGuid}]";
        
        var log = new RabbitLog
        {
            Id = requestGuid,
            Date = DateTime.Now,
            ServiceName = "ProfilesAPI",
            MethodName = requestName
        };

        try
        {
            try
            {
                log.MethodBody = JsonSerializer.Serialize(request);
                _rabbitMqService.SendMessage(log);
            }
            catch (NotSupportedException)
            {
                _rabbitMqService.SendMessage(log);
            }
            
            response = await next();
        }
        finally
        {
            //stopwatch.Stop();
           // _rabbitMqService.SendMessage($"[END] {requestNameWithGuid}; ExecutionTime = {stopwatch.ElapsedMilliseconds} ms");
           
        }
        
        return response;
    }
}