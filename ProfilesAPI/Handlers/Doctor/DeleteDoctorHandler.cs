using MediatR;
using ProfilesAPI.Commands.Doctor;
using ProfilesAPI.Repositories;

namespace ProfilesAPI.Handlers.Doctor;

public class DeleteDoctorHandler : IRequestHandler<DeleteDoctorCommand, Unit>
{
    private readonly IRavenRepository<Models.Doctor> _ravenRepository;

    public DeleteDoctorHandler(IRavenRepository<Models.Doctor> ravenRepository)
    {
        _ravenRepository = ravenRepository;
    }

    public async Task<Unit> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        await _ravenRepository.Delete(request.Id);
        return Unit.Value;
    }
}