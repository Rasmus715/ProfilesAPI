using System.Security.Cryptography;

namespace AuthorizationAPI.Infrastructure;

public interface IPasswordHashGenerator
{
    string GenerateHash(string password);
    bool VerifyHash(string password, string hashString);
}   

public class PasswordHashGenerator : IPasswordHashGenerator
{
    private const int SaltSize = 16; // 128 bits
    private const int KeySize = 32; // 256 bits
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    private const char SegmentDelimiter = ':';
    
    public string GenerateHash(string password)
    {
        
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            Algorithm,
            KeySize
        );
        
        return string.Join(
            SegmentDelimiter,
            Convert.ToHexString(hash),
            Convert.ToHexString(salt),
            Iterations,
            Algorithm
        );
    }

    public bool VerifyHash(string password, string hashString)
    {
        var segments = hashString.Split(SegmentDelimiter);
        var hash = Convert.FromHexString(segments[0]);
        var salt = Convert.FromHexString(segments[1]);
        var iterations = int.Parse(segments[2]);
        var algorithm = new HashAlgorithmName(segments[3]);
        var inputHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            algorithm,
            hash.Length
        );
        
        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }

    //TODO: Implement Database access, Hash generator
}