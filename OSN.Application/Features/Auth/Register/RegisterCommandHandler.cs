using MediatR;
using Microsoft.EntityFrameworkCore;
using OSN.Application.Utils;
using OSN.Domain.Models;
using OSN.Infrastructure;
using OSN.Infrastructure.Services;

namespace OSN.Application.Features.Auth.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher _passwordHasher;
    private readonly EmailService _emailService;

    public RegisterCommandHandler(AppDbContext db, PasswordHasher passwordHasher, EmailService emailService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand command, CancellationToken ct)
    {
        var request = command.Request;

        // Check if a verified user with this email already exists
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower(), ct);
        if (existingUser != null)
        {
            if (existingUser.GoogleSignIn != null)
            {
                return Result<RegisterResponse>.Failure("This email is linked to a Google account. Please sign in with Google instead.");
            }
            return Result<RegisterResponse>.Failure("User with this email already exists.");
        }

        var hashedPassword = _passwordHasher.HashSHA256Password(request.Password);
        var verificationCode = VerificationCodeGenerator.GenerateVerificationCode();
        var expirationTime = DateTime.UtcNow.AddMinutes(15);

        // Check for existing pending verification
        var existingPendingVerification = await _db.PendingVerifications
            .FirstOrDefaultAsync(p => p.Email.ToLower() == request.Email.ToLower(), ct);

        if (existingPendingVerification != null)
        {
            // Update existing pending verification
            existingPendingVerification.PasswordHash = hashedPassword;
            existingPendingVerification.VerificationCode = verificationCode;
            existingPendingVerification.ExpiresAt = expirationTime;
            existingPendingVerification.CreatedAt = DateTime.UtcNow;
        }
        else
        {
            // Create new pending verification
            var newPendingVerification = new PendingVerification
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expirationTime,
                VerificationCode = verificationCode
            };

            _db.PendingVerifications.Add(newPendingVerification);
        }

        await _db.SaveChangesAsync(ct);

        try
        {
            await _emailService.SendVerificationEmailAsync(request.Email, verificationCode);
        }
        catch (Exception ex)
        {
            // If email sending fails, we should still return success but log the error
            // In a real application, you might want to implement retry logic or queue the email
            // For now, we'll just return an error
            return Result<RegisterResponse>.Failure($"Failed to send verification email: {ex.Message}");
        }

        return Result<RegisterResponse>.Success(new RegisterResponse("Verification email sent. Please check your email and verify your account."));
    }
}