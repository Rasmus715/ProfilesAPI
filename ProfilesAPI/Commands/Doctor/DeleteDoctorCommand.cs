using MediatR;

namespace ProfilesAPI.Commands.Doctor;

public record DeleteDoctorCommand(Guid Id) : IRequest;