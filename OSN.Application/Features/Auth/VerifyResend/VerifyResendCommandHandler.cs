using MediatR;
using Microsoft.EntityFrameworkCore;
using OSN.Application.Utils;
using OSN.Infrastructure;
using OSN.Infrastructure.Services;

namespace OSN.Application.Features.Auth.VerifyResend;

public class VerifyResendCommandHandler : IRequestHandler<VerifyResendCommand, Result<VerifyResendResponse>>
{
    private readonly AppDbContext _db;
    private readonly EmailService _emailService;

    public VerifyResendCommandHandler(AppDbContext db, EmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }

    public async Task<Result<VerifyResendResponse>> Handle(VerifyResendCommand command, CancellationToken ct)
    {
        var request = command.Request;

        // Check if user already exists
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower(), ct);
        if (existingUser != null)
        {
            return Result<VerifyResendResponse>.Failure("User with this email is already verified.");
        }

        // Find pending verification
        var pendingVerification = await _db.PendingVerifications
            .FirstOrDefaultAsync(p => p.Email.ToLower() == request.Email.ToLower(), ct);

        if (pendingVerification == null)
        {
            return Result<VerifyResendResponse>.Failure("No pending verification found for this email. Please register first.");
        }

        // Generate new verification code and extend expiration
        var newVerificationCode = VerificationCodeGenerator.GenerateVerificationCode();
        var newExpirationTime = DateTime.UtcNow.AddMinutes(15);

        pendingVerification.VerificationCode = newVerificationCode;
        pendingVerification.ExpiresAt = newExpirationTime;

        await _db.SaveChangesAsync(ct);

        try
        {
            await _emailService.SendVerificationEmailAsync(request.Email, newVerificationCode);
        }
        catch (Exception ex)
        {
            return Result<VerifyResendResponse>.Failure($"Failed to resend verification email: {ex.Message}");
        }

        return Result<VerifyResendResponse>.Success(new VerifyResendResponse("Verification code resent. Please check your email."));
    }
}