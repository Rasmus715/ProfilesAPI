using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProfilesAPI.Commands.Patient;
using ProfilesAPI.Models;

namespace ProfilesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IMediator _mediator;

    public PatientController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Patient patient)
    {
        return Ok(await _mediator.Send(new AddPatientCommand(patient)));
    }
}