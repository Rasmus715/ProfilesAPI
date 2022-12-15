using MediatR;
using ProfilesAPI.Models;
using ProfilesAPI.Queries;
using ProfilesAPI.Repositories;

namespace ProfilesAPI.Handlers;

public class GetDoctorsHandler : IRequestHandler<GetDoctorsQuery, IEnumerable<Doctor>>
{
    private readonly IRavenRepository<Doctor> _ravenRepository;

    public GetDoctorsHandler(IRavenRepository<Doctor> ravenRepository)
    {
        _ravenRepository = ravenRepository;
    }

    public async Task<IEnumerable<Doctor>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
    {
       return await _ravenRepository.Get();
    }
}