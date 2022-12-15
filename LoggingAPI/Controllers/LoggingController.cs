using CommunicationModels;
using LoggingAPI.Services;
using LoggingAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LoggingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoggingController : ControllerBase
{
    private readonly ILoggingService _loggingService;

    public LoggingController(ILoggingService loggingService)
    {
        _loggingService = loggingService;
    }

    [HttpGet]
    public async Task<PagingList<RabbitLog>> Get([FromQuery]int page, int pageSize)
    {
        return await _loggingService.Get(page, pageSize);
    }
}