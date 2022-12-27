using AuthorizationAPI.Commands;
using AuthorizationAPI.Queries;
using AuthorizationAPI.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
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
    [Route("auth")]
    public async Task<IActionResult> Authenticate(LoginViewModel vm)
    {
        return Ok(await _mediator.Send(new LoginQuery(vm)));
    }
    
    [HttpPost]
    [Route("identity")]
    [Authorize]
    public IActionResult Identity()
    {
        return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
    }
}