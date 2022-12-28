using MediatR;


namespace ProfilesAPI.Queries.Doctor;

public record GetDoctorByAccountIdQuery(Guid Id) : IRequest<Models.Doctor>;