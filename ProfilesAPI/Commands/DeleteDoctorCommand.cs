using MediatR;

namespace ProfilesAPI.Commands;

public record DeleteDoctorCommand(Guid Id) : IRequest;