using MediatR;
using ProfilesAPI.Models;
using ProfilesAPI.Queries;
using ProfilesAPI.Repositories;

namespace ProfilesAPI.Handlers;

public class GetDoctorByIdHandler : IRequestHandler<GetDoctorByIdQuery, Doctor>
{
    private readonly IRavenRepository<Doctor> _ravenRepository;
    
    public GetDoctorByIdHandler(IRavenRepository<Doctor> ravenRepository)
    {
        _ravenRepository = ravenRepository;
    }
    
    public async Task<Doctor> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
    {
        return await _ravenRepository.Get(request.Id);
    }
}