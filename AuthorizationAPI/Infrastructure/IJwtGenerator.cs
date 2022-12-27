using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthorizationAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthorizationAPI.Infrastructure;

public interface IJwtGenerator
{
    string CreateToken(Account account);
}

public class JwtGenerator : IJwtGenerator
{
    private readonly SymmetricSecurityKey _key;
    private readonly IConfiguration _configuration;
    
    public JwtGenerator(IConfiguration config, IConfiguration configuration)
    {
        _configuration = configuration;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("Jwt:Key")!));
    }

    public string CreateToken(Account account)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Email, account.Email),
            new ("Role", "Patient")
        };
        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _configuration.GetValue<string>("Jwt:Issuer"),
            Audience = _configuration.GetValue<string>("Jwt:Audience"),
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = credentials
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}