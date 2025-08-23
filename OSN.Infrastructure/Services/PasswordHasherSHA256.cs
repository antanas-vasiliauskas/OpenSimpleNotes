using System.Security.Cryptography;
using System.Text;
using OSN.Application.Services;

namespace OSN.Infrastructure.Services;

public class PasswordHasherSHA256 : IPasswordHasher
{
    public bool VerifyPassword(string hashedPassword, string inputPassword)
    {
        var inputHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(inputPassword)));
        return hashedPassword == inputHash;
    }
    public string HashPassword(string password)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
    }
}