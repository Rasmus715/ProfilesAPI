using MediatR;
using ProfilesAPI.Commands;
using ProfilesAPI.Models;
using ProfilesAPI.Repositories;

namespace ProfilesAPI.Handlers;

public class UpdateDoctorHandler : IRequestHandler<UpdateDoctorCommand, Doctor>
{
    private readonly IRavenRepository<Doctor> _ravenRepository;

    public UpdateDoctorHandler(IRavenRepository<Doctor> ravenRepository)
    {
        _ravenRepository = ravenRepository;
    }

    public async Task<Doctor> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        return await _ravenRepository.Update(request.Doctor);
    }
}