using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProfilesAPI.Commands;
using ProfilesAPI.Commands.Doctor;
using ProfilesAPI.Controllers;
using ProfilesAPI.Models;
using ProfilesAPI.Queries;
using ProfilesAPI.Queries.Doctor;
using ProfilesAPI.ViewModels;

namespace ProfilesAPI.UnitTests.Unit.Controllers;

public class DoctorControllersTests
{
    private readonly DoctorController _doctorController;
    private readonly Mock<IMediator> _mediator;
    public DoctorControllersTests()
    {
        _mediator = new Mock<IMediator>();
        _doctorController = new DoctorController(_mediator.Object);
    }

    [Fact]
    public async Task Get()
    {
        var result = await _doctorController.Get();
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task GetById()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetDoctorByIdQuery>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult(new Doctor()));
        var result = await _doctorController.Get(Guid.NewGuid());  
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task Delete()
    {
        var result = await _doctorController.Delete(Guid.NewGuid());  
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task Create()
    { 
        //Arrange
        _mediator.Setup(m => m.Send(It.IsAny<AddDoctorCommand>(), It.IsAny<CancellationToken>()))
             .Returns(() => Task.FromResult(new Doctor()));
       
        //Act
        var result = await _doctorController.Create(new DoctorViewModel());  
        
        //Assert
        Assert.IsType<CreatedAtActionResult>(result);
    }
    
    [Fact]
    public async Task Update()
    {
        //Arrange
        _mediator.Setup(m => m.Send(It.IsAny<UpdateDoctorCommand>(), It.IsAny<CancellationToken>()))
             .ThrowsAsync(new ArgumentNullException());
        
        //Act
        var result = await _doctorController.Update(new Doctor());
         
        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task GetOfficeByDoctorId()
    { 
        
        //Act 
        var result = await _doctorController.GetOffice(Guid.NewGuid());
        
        //Assert
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task GetOfficeByDoctorIdThrowsKeyNotFoundException()
    { 
        //Arrange
        _mediator.Setup(m => m.Send(It.IsAny<GetOfficeByDoctorIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException());
        
        //Act 
        var result = await _doctorController.GetOffice(Guid.NewGuid());
        
        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task GetOfficeByDoctorIdThrowsArgumentNullException()
    { 
        //Arrange
        _mediator.Setup(m => m.Send(It.IsAny<GetOfficeByDoctorIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentNullException());
        
        //Act 
        var result = await _doctorController.GetOffice(Guid.NewGuid());
        
        //Assert
        Assert.IsType<NoContentResult>(result);
    }
}