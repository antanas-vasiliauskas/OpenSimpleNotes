using System.Security.Cryptography;
using System.Text;

namespace OSN.Infrastructure.Services;
public class PasswordHasher
{
    public bool VerifySHA256Password(string hashedPassword, string inputPassword)
    {
        var inputHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(inputPassword)));
        return hashedPassword == inputHash;
    }
    public string HashSHA256Password(string password)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes("password")));
    }
}