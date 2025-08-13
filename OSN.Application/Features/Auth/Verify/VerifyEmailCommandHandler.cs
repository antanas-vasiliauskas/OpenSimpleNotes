using MediatR;
using Microsoft.EntityFrameworkCore;
using OSN.Domain.Models;
using OSN.Domain.ValueObjects;
using OSN.Infrastructure;
using OSN.Infrastructure.Services;

namespace OSN.Application.Features.Auth.Verify;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result<VerifyEmailResponse>>
{
    private readonly AppDbContext _db;
    private readonly AuthService _authService;

    public VerifyEmailCommandHandler(AppDbContext db, AuthService authService)
    {
        _db = db;
        _authService = authService;
    }

    public async Task<Result<VerifyEmailResponse>> Handle(VerifyEmailCommand command, CancellationToken ct)
    {
        var request = command.Request;

        // Create and normalize email
        var emailString = EmailString.Create(request.Email);

        var pendingVerification = await _db.PendingVerifications
            .FirstOrDefaultAsync(p => p.Email == emailString, ct);

        if (pendingVerification == null)
        {
            return Result<VerifyEmailResponse>.Failure("No pending verification found for this email.");
        }

        if (pendingVerification.ExpiresAt < DateTime.UtcNow)
        {
            _db.PendingVerifications.Remove(pendingVerification);
            await _db.SaveChangesAsync(ct);
            return Result<VerifyEmailResponse>.Failure("Verification code has expired. Please register again.");
        }

        if (pendingVerification.VerificationCode != request.VerificationCode)
        {
            return Result<VerifyEmailResponse>.Failure("Invalid verification code.");
        }

        // Check if user already exists (shouldn't happen but safety check)
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == emailString, ct);
        if (existingUser != null)
        {
            _db.PendingVerifications.Remove(pendingVerification);
            await _db.SaveChangesAsync(ct);
            return Result<VerifyEmailResponse>.Failure("User with this email already exists.");
        }

        // Create the new user with normalized email
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = emailString,
            PasswordHash = pendingVerification.PasswordHash,
            Role = RoleHierarchy.UserRole,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(newUser);
        _db.PendingVerifications.Remove(pendingVerification);
        await _db.SaveChangesAsync(ct);

        var token = _authService.GenearateToken(newUser);

        return Result<VerifyEmailResponse>.Success(new VerifyEmailResponse(token, newUser.Role));
    }
}