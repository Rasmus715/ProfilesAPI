using AutoMapper;
using MediatR;
using ProfilesAPI.Commands;
using ProfilesAPI.Models;
using ProfilesAPI.Repositories;

namespace ProfilesAPI.Handlers;

public class AddDoctorHandler : IRequestHandler<AddDoctorCommand, Doctor>
{
    private readonly IRavenRepository<Doctor> _ravenRepository;
    private readonly IMapper _mapper;

    public AddDoctorHandler(IRavenRepository<Doctor> ravenRepository, IMapper mapper)
    {
        _ravenRepository = ravenRepository;
        _mapper = mapper;
    }
    
    public async Task<Doctor> Handle(AddDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = _mapper.Map<Doctor>(request.DoctorViewModel); 
        return await _ravenRepository.Create(doctor);
    }
}