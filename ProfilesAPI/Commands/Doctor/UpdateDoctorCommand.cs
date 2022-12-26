using MediatR;

namespace ProfilesAPI.Commands.Doctor;

public record UpdateDoctorCommand(Models.Doctor Doctor) : IRequest<Models.Doctor>;