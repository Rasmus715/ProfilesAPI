namespace AuthorizationAPI.Infrastructure;

public interface IPasswordHashGenerator
{
    string GenerateHash(string password);
}

public class PasswordHashGenerator : IPasswordHashGenerator
{
    public string GenerateHash(string password)
    {
        throw new NotImplementedException();
    }
    
    //TODO: Implement Database access, Hash generator
}