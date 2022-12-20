using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProfilesAPI.Commands;
using ProfilesAPI.Controllers;
using ProfilesAPI.Models;
using ProfilesAPI.Queries;
using ProfilesAPI.ViewModels;

namespace ProfilesAPI.UnitTests;

public class DoctorControllersTests
{
    private readonly DoctorsController _doctorsController;
    private readonly Mock<IMediator> _mediator;
    public DoctorControllersTests()
    {
        _mediator = new Mock<IMediator>();
        _doctorsController = new DoctorsController(_mediator.Object);
    }

    [Fact]
    public async Task Get()
    {
        var result = await _doctorsController.Get();
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task GetById()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetDoctorByIdQuery>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult(new Doctor()));
        var result = await _doctorsController.Get(Guid.NewGuid());  
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task Delete()
    {
        var result = await _doctorsController.Delete(Guid.NewGuid());  
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task Create()
    { 
        //Arrange
        _mediator.Setup(m => m.Send(It.IsAny<AddDoctorCommand>(), It.IsAny<CancellationToken>()))
             .Returns(() => Task.FromResult(new Doctor()));
       
        //Act
        var result = await _doctorsController.Create(new DoctorViewModel());  
        
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
        var result = await _doctorsController.Update(new Doctor());
         
        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task GetOfficeByDoctorId()
    { 
        
        //Act 
        var result = await _doctorsController.GetOffice(Guid.NewGuid());
        
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
        var result = await _doctorsController.GetOffice(Guid.NewGuid());
        
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
        var result = await _doctorsController.GetOffice(Guid.NewGuid());
        
        //Assert
        Assert.IsType<NoContentResult>(result);
    }
}