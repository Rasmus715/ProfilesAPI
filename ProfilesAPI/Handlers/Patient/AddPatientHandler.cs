using MediatR;
using ProfilesAPI.Commands.Patient;
using ProfilesAPI.Repositories;

namespace ProfilesAPI.Handlers.Patient;

public class AddPatientHandler : IRequestHandler<AddPatientCommand, Models.Patient>
{
    private readonly IRavenRepository<Models.Patient> _ravenRepository;

    public AddPatientHandler(IRavenRepository<Models.Patient> ravenRepository)
    {
        _ravenRepository = ravenRepository;
    }

    public async Task<Models.Patient> Handle(AddPatientCommand request, CancellationToken cancellationToken)
    { 
        return await _ravenRepository.Create(request.Patient);
    }
}