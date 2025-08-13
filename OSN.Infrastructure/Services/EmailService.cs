using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace OSN.Infrastructure.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendVerificationEmailAsync(string toEmail, string verificationCode)
    {
        var noreplyEmail = _configuration["NoreplyEmail:Email"];
        var appPassword = _configuration["NoreplyEmail:AppPassword"];

        if (string.IsNullOrEmpty(noreplyEmail) || string.IsNullOrEmpty(appPassword))
        {
            throw new InvalidOperationException("Email configuration is missing.");
        }

        var subject = "Verify Your Email - OpenSimpleNotes";
        var body = $@"
            <html>
            <body>
                <h2>Email Verification</h2>
                <p>Thank you for registering with Open Simple Notes!</p>
                <p>Your verification code is: <strong>{verificationCode}</strong></p>
                <p>This code will expire in 15 minutes.</p>
                <p>If you didn't request this verification, please ignore this email.</p>
            </body>
            </html>";

        using var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(noreplyEmail, appPassword)
        };

        var message = new MailMessage(noreplyEmail, toEmail, subject, body)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(message);
    }
}