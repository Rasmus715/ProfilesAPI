using MediatR;
using ProfilesAPI.Models;
using ProfilesAPI.ViewModels;

namespace ProfilesAPI.Commands;

public record AddDoctorCommand(DoctorViewModel DoctorViewModel) : IRequest<Doctor>;