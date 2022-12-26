using MediatR;
using ProfilesAPI.Queries;
using ProfilesAPI.Repositories;

namespace ProfilesAPI.Handlers.Doctor;

public class GetDoctorByIdHandler : IRequestHandler<GetDoctorByIdQuery, Models.Doctor>
{
    private readonly IRavenRepository<Models.Doctor> _ravenRepository;
    
    public GetDoctorByIdHandler(IRavenRepository<Models.Doctor> ravenRepository)
    {
        _ravenRepository = ravenRepository;
    }
    
    public async Task<Models.Doctor> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
    {
        return await _ravenRepository.Get(request.Id);
    }
}