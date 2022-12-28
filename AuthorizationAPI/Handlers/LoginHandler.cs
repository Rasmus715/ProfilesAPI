using System.Net;
using AuthorizationAPI.Data;
using AuthorizationAPI.Exceptions;
using AuthorizationAPI.Infrastructure;
using AuthorizationAPI.Models;
using AuthorizationAPI.Queries;
using AuthorizationAPI.ViewModels;
using MediatR;
using Raven.Client.Documents;

namespace AuthorizationAPI.Handlers;

public class LoginHandler : IRequestHandler<LoginQuery, JwtToken>
{
    private readonly IJwtGenerator _jwtGenerator;
    private readonly RavenDbContext _ravenDbContext;
    private readonly IPasswordHashGenerator _passwordHashGenerator;
    private readonly HttpClient _httpClient;

    public LoginHandler(RavenDbContext ravenDbContext, IJwtGenerator jwtGenerator, IPasswordHashGenerator passwordHashGenerator, HttpClient httpClient)
    {
        _ravenDbContext = ravenDbContext;
        _jwtGenerator = jwtGenerator;
        _passwordHashGenerator = passwordHashGenerator;
        _httpClient = httpClient;
    }

    public async Task<JwtToken> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var store = _ravenDbContext.Store;
        using var session = store.OpenAsyncSession();
        var account = await session.Query<Account>()
            .Search(a => a.Email, request.LoginViewModel.Email)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (account == null || !_passwordHashGenerator.VerifyHash(request.LoginViewModel.Password, account.PasswordHash))
            throw new UnsuccessfulLoginException();
        
        if (request.LoginViewModel.IsDoctor)
        {
            var response = await _httpClient.GetAsync($"Doctor/account/{account.Id}", cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Something went wrong with ProfilesAPI");
                
            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException("There's no Doctor with such AccountID");
        }
        
        return new JwtToken
        {
            Token = _jwtGenerator.CreateToken(account, request.LoginViewModel.IsDoctor)
        };
     
    }
}