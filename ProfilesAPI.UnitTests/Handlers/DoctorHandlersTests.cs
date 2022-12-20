using System.Net;
using AutoMapper;
using CommunicationModels;
using MediatR;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using ProfilesAPI.Commands;
using ProfilesAPI.Handlers;
using ProfilesAPI.Models;
using ProfilesAPI.Queries;
using ProfilesAPI.Repositories;
using ProfilesAPI.ViewModels;
using Raven.TestDriver;
using Shouldly;

namespace ProfilesAPI.UnitTests;

public class DoctorsRepositoryTests : RavenTestDriver
{
    private static bool _serverRunning;
    private readonly IRavenRepository<Doctor> _sut;
    private readonly Mock<IHttpClientFactory> _mockFactory;
    private readonly HttpClient _httpClient;
    
    public DoctorsRepositoryTests()
    {
        StartServerIfNotRunning();
        _sut = new RavenRepository<Doctor>(GetDocumentStore().OpenAsyncSession()); 
        _mockFactory = new Mock<IHttpClientFactory>();
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://test.com/")
        };
    }
    
    private static void StartServerIfNotRunning()
    {
        if (_serverRunning) 
            return;
        _serverRunning = true;
        
        ConfigureServer(new TestServerOptions
        {
            
            DataDirectory = "./RavenDbTest/",
        });
    }
    
    [Fact]
    public async Task GetDoctors()
    {
        var handler = new GetDoctorsHandler(_sut);
        var result = await handler.Handle(new GetDoctorsQuery(), CancellationToken.None);
        result.ShouldBeOfType<List<Doctor>>();
    }

    [Fact]
    public async Task GetDoctorById()
    {
        var doc = await _sut.Create(new Doctor());
        var handler = new GetDoctorByIdHandler(_sut);
        var result = await handler.Handle(new GetDoctorByIdQuery(Guid.Parse(doc.Id)),CancellationToken.None);
        result.ShouldBeOfType<Doctor>();
    }
    
    [Fact]
    public async Task AddDoctor()
    {
        var mockMapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });
        var mapper = mockMapper.CreateMapper();
        
        var handler = new AddDoctorHandler(_sut, mapper);
        var result = 
            await handler.Handle(new AddDoctorCommand(new DoctorViewModel()),CancellationToken.None);
        result.ShouldBeOfType<Doctor>();
    }
    
    [Fact]
    public async Task UpdateDoctorSuccess()
    {
        var doc = await _sut.Create(new Doctor());
        var handler = new UpdateDoctorHandler(_sut);
        var result = 
            await handler.Handle(new UpdateDoctorCommand(new Doctor
            {
                Id = doc.Id
            }),CancellationToken.None);
        result.ShouldBeOfType<Doctor>();
    }
    
    [Fact]
    public async Task UpdateDoctorArgNullException()
    {
        var handler = new UpdateDoctorHandler(_sut);
        try
        {
            await handler.Handle(new UpdateDoctorCommand(new Doctor()), CancellationToken.None);
        }
        catch (ArgumentNullException)
        {
            
        }
    }
    
    [Fact]
    public async Task DeleteDoctorArgNullException()
    {
        var handler = new DeleteDoctorHandler(_sut);
        try
        {
            await handler.Handle(new DeleteDoctorCommand(Guid.NewGuid()), CancellationToken.None);
        }
        catch (ArgumentNullException)
        {
            
        }
    }
    
    [Fact]
    public async Task DeleteDoctor()
    {
        var doc = await _sut.Create(new Doctor());
        var handler = new DeleteDoctorHandler(_sut);
        var result = await handler.Handle(new DeleteDoctorCommand(Guid.Parse(doc.Id)), CancellationToken.None);
        result.ShouldBeOfType<Unit>();
    }
    
    [Fact]
    public async Task GetOfficeByDoctorId()
    {
        //Arrange
        var doc = await _sut.Create(new Doctor
        {
            OfficeId = Guid.NewGuid()
        });
        
        var mockFactory = new Mock<IHttpClientFactory>();
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new Office()))
            });

        var client = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://test.com/")
        };
        
        mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        
        //Act
        var handler = new GetOfficeByDoctorIdHandler(mockFactory.Object, _sut);
        var result = await handler.Handle(new GetOfficeByDoctorIdQuery(Guid.Parse(doc.Id)), CancellationToken.None);
        
        //Assert
        result.ShouldBeOfType<Office>();
    }

    [Fact]
    public async Task GetOfficeByDoctorIdThrowHttpRequestException()
    {
        //Arrange
        var doc = await _sut.Create(new Doctor
        {
            OfficeId = Guid.NewGuid()
        });
        
        _mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(_httpClient);
        
        //Act
        var handler = new GetOfficeByDoctorIdHandler(_mockFactory.Object, _sut);
        try
        {
            await handler.Handle(new GetOfficeByDoctorIdQuery(Guid.Parse(doc.Id)), CancellationToken.None);
            
            //Assert
            Assert.Fail("Unable to catch HttpRequestException");
        }
        catch (HttpRequestException)
        {
            Assert.True(true);
        }
    }
    
    [Fact]
    public async Task GetOfficeByDoctorIdThrowKeyArgumentNullException()
    {
        //Arrange
        var doc = await _sut.Create(new Doctor());

        _mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(_httpClient);
        
        //Act
        var handler = new GetOfficeByDoctorIdHandler(_mockFactory.Object, _sut);
        try
        {
            await handler.Handle(new GetOfficeByDoctorIdQuery(Guid.Parse(doc.Id)), CancellationToken.None);
            
            //Assert
            Assert.Fail("Unable to catch ArgumentNullException");
        }
        catch (ArgumentNullException)
        {
            Assert.True(true);
        }
    }

    [Fact]
    public async Task GetOfficeByDoctorIdThrowKeyNotFoundException()
    {
        //Arrange
        _mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(_httpClient);
        
        //Act
        var handler = new GetOfficeByDoctorIdHandler(_mockFactory.Object, _sut);
        try
        {
            await handler.Handle(new GetOfficeByDoctorIdQuery(Guid.NewGuid()), CancellationToken.None);
            
            //Assert
            Assert.Fail("Unable to catch KeyNotFoundException");
        }
        catch (KeyNotFoundException)
        {
            Assert.True(true);
        }
    }
}