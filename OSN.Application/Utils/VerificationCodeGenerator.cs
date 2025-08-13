namespace OSN.Application.Utils;

public static class VerificationCodeGenerator
{
    public static string GenerateVerificationCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}