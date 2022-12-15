using System.Text.Json;
using CommunicationModels;
using MediatR;
using ProfilesAPI.Models;
using ProfilesAPI.Queries;
using ProfilesAPI.Repositories;

namespace ProfilesAPI.Handlers;

public class GetOfficeByDoctorIdHandler : IRequestHandler<GetOfficeByDoctorIdQuery, Office?>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IRavenRepository<Doctor> _ravenRepository;

    public GetOfficeByDoctorIdHandler(IHttpClientFactory httpClientFactory, IRavenRepository<Doctor> ravenRepository)
    {
        _httpClientFactory = httpClientFactory;
        _ravenRepository = ravenRepository;
    }
    public async Task<Office?> Handle(GetOfficeByDoctorIdQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _ravenRepository.Get(request.Id);
        
        if (doctor is null)
            throw new KeyNotFoundException("Doctor is not found");
        
        if (doctor.OfficeId is null)
            throw new ArgumentNullException($"OfficeID of Doctor {doctor.Id} is null");
        
        var httpClient = _httpClientFactory.CreateClient("OfficesAPI");

        var httpResponseMessage = await httpClient
            .GetAsync(
            "?id=" + doctor.OfficeId, cancellationToken);

        if (!httpResponseMessage.IsSuccessStatusCode) 
            throw new HttpRequestException("Something went wrong");
        
        await using var contentStream =
            await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken);

        var office = await JsonSerializer.DeserializeAsync
            <Office>(contentStream, cancellationToken: cancellationToken, 
                options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

        return office;
    }
}