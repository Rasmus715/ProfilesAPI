using MediatR;
using ProfilesAPI.Queries;
using ProfilesAPI.Repositories;

namespace ProfilesAPI.Handlers.Doctor;

public class GetDoctorsHandler : IRequestHandler<GetDoctorsQuery, IEnumerable<Models.Doctor>>
{
    private readonly IRavenRepository<Models.Doctor> _ravenRepository;

    public GetDoctorsHandler(IRavenRepository<Models.Doctor> ravenRepository)
    {
        _ravenRepository = ravenRepository;
    }

    public async Task<IEnumerable<Models.Doctor>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
    {
       return await _ravenRepository.Get();
    }
}