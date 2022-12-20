using AuthorizationAPI.ViewModels;
using MediatR;

namespace AuthorizationAPI.Commands;

public record RegisterCommand(RegisterViewModel RegisterViewModel) : IRequest;