using AuthorizationAPI.Data;
using AuthorizationAPI.Exceptions;
using AuthorizationAPI.Infrastructure;
using AuthorizationAPI.Models;
using AuthorizationAPI.Queries;
using AuthorizationAPI.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents;

namespace AuthorizationAPI.Handlers;

public class LoginHandler : IRequestHandler<LoginQuery, JwtToken>
{
    private readonly IJwtGenerator _jwtGenerator;
    private readonly RavenDbContext _ravenDbContext;
    private readonly IPasswordHashGenerator _passwordHashGenerator;

    public LoginHandler(RavenDbContext ravenDbContext, IJwtGenerator jwtGenerator, IPasswordHashGenerator passwordHashGenerator)
    {
        _ravenDbContext = ravenDbContext;
        _jwtGenerator = jwtGenerator;
        _passwordHashGenerator = passwordHashGenerator;
    }

    public async Task<JwtToken> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var store = _ravenDbContext.Store;
        using var session = store.OpenAsyncSession();
        var account = await session.Query<Account>()
            .Search(a => a.Email, request.LoginViewModel.Email)
            .FirstOrDefaultAsync(cancellationToken);

        if (account == null)
        {
            throw new ArgumentNullException(nameof(request.LoginViewModel), "User is not found");
        }

        if (!_passwordHashGenerator.VerifyHash(request.LoginViewModel.Password, account.PasswordHash))
            throw new WrongPasswordException();
        
        return new JwtToken
        {
            Token = _jwtGenerator.CreateToken(account)
        };
     
    }
}