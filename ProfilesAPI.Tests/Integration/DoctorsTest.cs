using System.Net;
using System.Net.Http.Json;
using ProfilesAPI.Models;
using ProfilesAPI.ViewModels;

namespace ProfilesAPI.UnitTests.Integration;

public class DoctorsTest : IntegrationTest
{
    [Fact]
    public async Task GetAll_Returns200()
    {
        var result = await HttpClient.GetAsync("/api/Doctors");
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }
    
    [Fact]
    public async Task Create_Returns201()
    {
        var docVm = new DoctorViewModel
        {
            FirstName = "Test",
            MiddleName = "Test",
            LastName = "Test"
        };
        
        var response = await HttpClient.PostAsync("/api/Doctors", JsonContent.Create(docVm));
        var content = await response.Content.ReadAsAsync<Doctor>();

        
        Assert.Equal(docVm.FirstName, content.FirstName);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    [Fact]
    public async Task Create_ValidationError_Returns400()
    {
        var response = await HttpClient.PostAsync("/api/Doctors", JsonContent.Create(new DoctorViewModel()));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_NotFound_Returns204()
    {
        var getResponse = await HttpClient.GetAsync($"/api/Doctors/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NoContent, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetById_Returns200()
    {
        //Doesn't work, unfortunately
        // var doctor = new Doctor
        // {
        //     Id = Guid.NewGuid().ToString(),
        //     FirstName = "Test",
        //     MiddleName = "Test",
        //     LastName = "Test"
        // };
        // await _asyncDocumentSession.StoreAsync(doctor);
   
        //Arrange
        var docVm = new DoctorViewModel
        {
            FirstName = "Test",
            MiddleName = "Test",
            LastName = "Test"
        };
        var createResponse = await HttpClient.PostAsync("/api/Doctors", JsonContent.Create(docVm));
        var doctor = await createResponse.Content.ReadAsAsync<Doctor>();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal(docVm.FirstName, doctor.FirstName);

        //Act
        var getResponse = await HttpClient.GetAsync($"/api/Doctors/{doctor.Id}");
        var doctorFromGet = await getResponse.Content.ReadAsAsync<Doctor>();
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal(doctor.FirstName, doctorFromGet.FirstName);
    }

    [Fact]
    public async Task Delete_Returns400()
    {
        var response = await HttpClient.DeleteAsync($"/api/Doctors/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Delete_Returns200()
    {
        var docVm = new DoctorViewModel
        {
            FirstName = "Test",
            MiddleName = "Test",
            LastName = "Test"
        };
        
        var createResponse = await HttpClient.PostAsync("/api/Doctors", JsonContent.Create(docVm));
        var doctor = await createResponse.Content.ReadAsAsync<Doctor>();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal(docVm.FirstName, doctor.FirstName);
        
        var response = await HttpClient.DeleteAsync($"/api/Doctors/{doctor.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_ValidationError_Returns400()
    {
        var response = await HttpClient.PatchAsync("/api/Doctors", JsonContent.Create(new Doctor()));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Update_Returns200()
    {
        
        var docVm = new DoctorViewModel
        {
            FirstName = "Test",
            MiddleName = "Test",
            LastName = "Test"
        };
        
        var createResponse = await HttpClient.PostAsync("/api/Doctors", JsonContent.Create(docVm));
        var doctor = await createResponse.Content.ReadAsAsync<Doctor>();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal(docVm.FirstName, doctor.FirstName);

        doctor.FirstName = "Test2";
        
        var response = await HttpClient.PatchAsync("/api/Doctors", JsonContent.Create(doctor));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_NotFound_Returns400()
    {
        var response = await HttpClient.PatchAsync("/api/Doctors", JsonContent.Create(new Doctor
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "Test",
            MiddleName = "Test",
            LastName = "Test"
        }));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}