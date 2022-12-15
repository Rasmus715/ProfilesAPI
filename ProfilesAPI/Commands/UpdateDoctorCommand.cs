using MediatR;
using ProfilesAPI.Models;

namespace ProfilesAPI.Commands;

public record UpdateDoctorCommand(Doctor Doctor) : IRequest<Doctor>;