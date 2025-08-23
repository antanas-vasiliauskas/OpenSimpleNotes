namespace OSN.Application.Utils;

public static class VerificationCode
{
    public const int ExpirationMinutes = 15;
    public static string Generate()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}