using AuthorizationAPI.Models;

namespace AuthorizationAPI.Infrastructure.Security;

public interface IJwtGenerator
{
    string CreateToken(Account account);
}