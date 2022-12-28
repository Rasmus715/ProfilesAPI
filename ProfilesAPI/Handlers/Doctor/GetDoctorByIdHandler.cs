using MediatR;
using ProfilesAPI.Queries;
using ProfilesAPI.Queries.Doctor;
using ProfilesAPI.Repositories;
using ProfilesAPI.ViewModels;

namespace ProfilesAPI.Handlers.Doctor;

public class GetDoctorByIdHandler : IRequestHandler<GetDoctorByIdQuery, DoctorViewModel>
{
    private readonly IRavenRepository<Models.Doctor> _ravenRepository;
    
    public GetDoctorByIdHandler(IRavenRepository<Models.Doctor> ravenRepository)
    {
        _ravenRepository = ravenRepository;
    }
    
    public async Task<DoctorViewModel> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _ravenRepository.Get(request.Id);
        return new DoctorViewModel
        {
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            MiddleName = doctor.MiddleName,
            DateOfBirth = doctor.DateOfBirth,
            AccountId = doctor.AccountId,
            SpecializationId = doctor.SpecializationId,
            OfficeId = doctor.OfficeId,
            CareerStartYear = doctor.CareerStartYear,
            Status = doctor.Status.ToString()
        };
    }
}