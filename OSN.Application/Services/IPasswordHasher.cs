namespace OSN.Application.Services;

public interface IPasswordHasher
{
    bool VerifyPassword(string hashedPassword, string inputPassword);
    string HashPassword(string password);
}