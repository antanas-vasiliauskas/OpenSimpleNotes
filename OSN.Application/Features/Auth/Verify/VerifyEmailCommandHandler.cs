using MediatR;
using OSN.Application.Repositories;
using OSN.Application.Services;
using OSN.Domain.Core;
using OSN.Domain.Models;
using OSN.Domain.ValueObjects;

namespace OSN.Application.Features.Auth.Verify;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result<VerifyEmailResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPendingVerificationRepository _pendingVerificationRepository;
    private readonly IAuthService _authService;

    public VerifyEmailCommandHandler(
        IUserRepository userRepository, 
        IPendingVerificationRepository pendingVerificationRepository, 
        IAuthService authService)
    {
        _userRepository = userRepository;
        _pendingVerificationRepository = pendingVerificationRepository;
        _authService = authService;
    }

    public async Task<Result<VerifyEmailResponse>> Handle(VerifyEmailCommand command, CancellationToken ct)
    {
        var emailString = EmailString.Create(command.Email);

        var pendingVerification = await _pendingVerificationRepository.GetByEmailAsync(emailString, ct);

        if (pendingVerification == null)
        {
            return Result<VerifyEmailResponse>.Failure("No pending verification found for this email.");
        }

        if (pendingVerification.ExpiresAt < DateTime.UtcNow)
        {
            _pendingVerificationRepository.Remove(pendingVerification);
            await _pendingVerificationRepository.UnitOfWork.SaveChangesAsync(ct);
            return Result<VerifyEmailResponse>.Failure("Verification code has expired. Please register again.");
        }

        if (pendingVerification.VerificationCode != command.VerificationCode)
        {
            return Result<VerifyEmailResponse>.Failure("Invalid verification code.");
        }

        var existingUser = await _userRepository.GetUserByEmailAsync(emailString, ct);
        if (existingUser != null)
        {
            _pendingVerificationRepository.Remove(pendingVerification);
            await _pendingVerificationRepository.UnitOfWork.SaveChangesAsync(ct);
            return Result<VerifyEmailResponse>.Failure("User with this email already exists.");
        }

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = emailString,
            PasswordHash = pendingVerification.PasswordHash,
            Role = RoleHierarchy.UserRole,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _userRepository.Add(newUser);
        _pendingVerificationRepository.Remove(pendingVerification);
        await _userRepository.UnitOfWork.SaveChangesAsync(ct);

        var token = _authService.GenearateJwtToken(newUser);
        return Result<VerifyEmailResponse>.Success(new VerifyEmailResponse(token, newUser.Role));
    }
}