using Asp.Versioning;
using AuthorizationAPI.Commands;
using AuthorizationAPI.Exceptions;
using AuthorizationAPI.Queries;
using AuthorizationAPI.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationAPI.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    [ApiVersion("1")]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        try
        {
            await _mediator.Send(new RegisterCommand(vm));
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = ex.Message
            });
        }
    }

    [HttpPost]
    [ApiVersion("1")]
    [Route("auth")]
    public async Task<IActionResult> Authenticate(LoginViewModel vm)
    {
        try
        {
            return Ok(await _mediator.Send(new LoginQuery(vm)));
        }
        catch (UnsuccessfulLoginException)
        {
            return BadRequest(new
            {
                error = "Either an email or a password is incorrect"
            });
        }
    }
    
    [HttpPost]
    [Route("identity")]
    [Authorize]
    public IActionResult Identity()
    {
        return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
    }
}