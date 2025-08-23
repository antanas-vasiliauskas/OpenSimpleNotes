namespace OSN.Application.Services;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string toEmail, string verificationCode);
}