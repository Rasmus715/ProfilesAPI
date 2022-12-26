using MediatR;
using ProfilesAPI.ViewModels;

namespace ProfilesAPI.Commands.Doctor;

public record AddDoctorCommand(DoctorViewModel DoctorViewModel) : IRequest<Models.Doctor>;