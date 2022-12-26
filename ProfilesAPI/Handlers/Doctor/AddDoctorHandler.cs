using AutoMapper;
using MediatR;
using ProfilesAPI.Commands.Doctor;
using ProfilesAPI.Repositories;

namespace ProfilesAPI.Handlers.Doctor;

public class AddDoctorHandler : IRequestHandler<AddDoctorCommand, Models.Doctor>
{
    private readonly IRavenRepository<Models.Doctor> _ravenRepository;
    private readonly IMapper _mapper;

    public AddDoctorHandler(IRavenRepository<Models.Doctor> ravenRepository, IMapper mapper)
    {
        _ravenRepository = ravenRepository;
        _mapper = mapper;
    }
    
    public async Task<Models.Doctor> Handle(AddDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = _mapper.Map<Models.Doctor>(request.DoctorViewModel); 
        return await _ravenRepository.Create(doctor);
    }
}