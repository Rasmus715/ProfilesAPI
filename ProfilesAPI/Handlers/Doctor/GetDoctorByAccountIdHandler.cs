using MediatR;
using ProfilesAPI.Data;
using ProfilesAPI.Queries.Doctor;
using ProfilesAPI.Repositories;
using Raven.Client.Documents;

namespace ProfilesAPI.Handlers.Doctor;

public class GetDoctorByAccountIdHandler : IRequestHandler<GetDoctorByAccountIdQuery, Models.Doctor>
{
    private readonly IRavenContext _ravenRepository;

    public GetDoctorByAccountIdHandler(IRavenContext ravenRepository)
    {
        _ravenRepository = ravenRepository;
    }

    public async Task<Models.Doctor> Handle(GetDoctorByAccountIdQuery request, CancellationToken cancellationToken)
    {
        using var session = _ravenRepository.GetSession();
        return await session.Query<Models.Doctor>()
            .Where(doctor => doctor.AccountId == request.Id)
            .FirstOrDefaultAsync(token: cancellationToken);
    }
}