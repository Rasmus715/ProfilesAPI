using MediatR;
using ProfilesAPI.Commands;
using ProfilesAPI.Models;
using ProfilesAPI.Repositories;

namespace ProfilesAPI.Handlers;

public class DeleteDoctorHandler : IRequestHandler<DeleteDoctorCommand, Unit>
{
    private readonly IRavenRepository<Doctor> _ravenRepository;

    public DeleteDoctorHandler(IRavenRepository<Doctor> ravenRepository)
    {
        _ravenRepository = ravenRepository;
    }

    public async Task<Unit> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        await _ravenRepository.Delete(request.Id);
        return Unit.Value;
    }
}