using MediatR;
using ProfilesAPI.ViewModels;

namespace ProfilesAPI.Queries.Doctor;

public record GetDoctorByIdQuery(Guid Id) : IRequest<DoctorViewModel>;