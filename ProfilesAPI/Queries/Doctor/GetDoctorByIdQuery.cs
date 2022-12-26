using MediatR;
using ProfilesAPI.Models;

namespace ProfilesAPI.Queries;

public record GetDoctorByIdQuery(Guid Id) : IRequest<Doctor>;