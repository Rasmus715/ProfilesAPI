using AuthorizationAPI.ViewModels;
using MediatR;

namespace AuthorizationAPI.Queries;

public record LoginQuery(LoginViewModel Vm) : IRequest<JwtToken>;