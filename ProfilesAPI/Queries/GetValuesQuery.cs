using MediatR;
using ProfilesAPI.Models;

namespace ProfilesAPI.Queries;

public record GetDoctorsQuery : IRequest<IEnumerable<Doctor>>;