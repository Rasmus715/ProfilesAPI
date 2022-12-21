using AuthorizationAPI.Exceptions;
using AuthorizationAPI.Infrastructure.Security;
using AuthorizationAPI.Models;
using AuthorizationAPI.Queries;
using AuthorizationAPI.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationAPI.Handlers;

public class LoginHandler : IRequestHandler<LoginQuery, JwtToken>
{
    private readonly UserManager<Account> _userManager;
    private readonly SignInManager<Account> _signInManager;
    private readonly IJwtGenerator _jwtGenerator;

    public LoginHandler(UserManager<Account> userManager, SignInManager<Account> signInManager, IJwtGenerator jwtGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtGenerator = jwtGenerator;
    }

    public async Task<JwtToken> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Vm.Email);
        
        if (user == null)
        {
            throw new ArgumentNullException(nameof(request.Vm), "User is not found");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Vm.Password, false);

        if (result.Succeeded)
        {
            return new JwtToken
            {
                Token = _jwtGenerator.CreateToken(user)
            };
        }

        throw new WrongPasswordException();
    }
}