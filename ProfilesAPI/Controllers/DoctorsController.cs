using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProfilesAPI.Commands;
using ProfilesAPI.Models;
using ProfilesAPI.Queries;
using ProfilesAPI.ViewModels;

namespace ProfilesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DoctorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<Doctor> Get(Guid id)
    {
        return await _mediator.Send(new GetDoctorByIdQuery(id));
    }
    
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteDoctorCommand(id));
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(DoctorViewModel vm)
    {
        var doctor = await _mediator.Send(new AddDoctorCommand(vm));
        return CreatedAtAction(nameof(Create), new {id = doctor.Id}, doctor);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var doctors = await _mediator.Send(new GetDoctorsQuery());
        return Ok(doctors);
    }
    
    [HttpPatch]
    public async Task<IActionResult> Update(Doctor doctor)
    {
        try
        {
            var result = await _mediator.Send(new UpdateDoctorCommand(doctor));
            return Ok(result);
        }
        catch (ArgumentNullException)
        {
            return BadRequest("There is no doctor with such ID");
        }
    }

    [HttpGet]
    [Route("{id:guid}/Office")]
    public async Task<IActionResult> GetOffice(Guid id)
    {
        try
        {
            return Ok(await _mediator.Send(new GetOfficeByDoctorIdQuery(id)));
        }
        catch (KeyNotFoundException ex)  
        {
            return BadRequest(new { ErrorMessage = ex.Message });
        }
        catch (ArgumentNullException)
        {
            return NoContent();
        }
    }
}