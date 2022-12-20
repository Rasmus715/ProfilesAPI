using AuthorizationAPI.Queries;
using AuthorizationAPI.ViewModels;
using MediatR;
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
    public async Task<IActionResult> Authenticate(LoginViewModel vm)
    {
        try
        {
            return Ok(await _mediator.Send(new LoginQuery(vm)));
        }
        catch
        {
            return BadRequest(new { errorMessage = "Email or password are incorrect"});
        }
    }
}