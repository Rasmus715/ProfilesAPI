using MediatR;
using ProfilesAPI.Commands.Doctor;
using ProfilesAPI.Repositories;

namespace ProfilesAPI.Handlers.Doctor;

public class UpdateDoctorHandler : IRequestHandler<UpdateDoctorCommand, Models.Doctor>
{
    private readonly IRavenRepository<Models.Doctor> _ravenRepository;

    public UpdateDoctorHandler(IRavenRepository<Models.Doctor> ravenRepository)
    {
        _ravenRepository = ravenRepository;
    }

    public async Task<Models.Doctor> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        return await _ravenRepository.Update(request.Doctor);
    }
}