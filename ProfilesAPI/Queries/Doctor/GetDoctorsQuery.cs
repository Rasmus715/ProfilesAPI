using MediatR;

namespace ProfilesAPI.Queries.Doctor;

public record GetDoctorsQuery : IRequest<IEnumerable<Models.Doctor>>;