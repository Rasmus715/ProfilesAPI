using CommunicationModels;
using MediatR;

namespace ProfilesAPI.Queries;

public record GetOfficeByDoctorIdQuery(Guid Id) : IRequest<Office?>;