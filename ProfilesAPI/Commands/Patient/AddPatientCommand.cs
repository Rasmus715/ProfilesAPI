using MediatR;

namespace ProfilesAPI.Commands.Patient;

public record AddPatientCommand(Models.Patient Patient) : IRequest<Models.Patient>;