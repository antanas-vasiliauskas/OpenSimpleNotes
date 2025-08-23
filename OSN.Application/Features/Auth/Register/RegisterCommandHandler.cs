using MediatR;
using OSN.Application.Repositories;
using OSN.Application.Services;
using OSN.Application.Utils;
using OSN.Domain.Models;
using OSN.Domain.ValueObjects;

namespace OSN.Application.Features.Auth.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPendingVerificationRepository _pendingVerificationRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPendingVerificationRepository pendingVerificationRepository,
        IPasswordHasher passwordHasher,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _pendingVerificationRepository = pendingVerificationRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand command, CancellationToken ct)
    {
        var emailString = EmailString.Create(command.Email);

        var existingUser = await _userRepository.GetUserByEmailAsync(emailString, ct);
        if (existingUser != null)
        {
            if (existingUser.GoogleSignIn != null)
            {
                return Result<RegisterResponse>.Failure("This email is linked to a Google account. Please sign in with Google instead.");
            }
            return Result<RegisterResponse>.Failure("User with this email already exists.");
        }

        var hashedPassword = _passwordHasher.HashPassword(command.Password);
        var verificationCode = VerificationCode.Generate();
        var expirationTime = DateTime.UtcNow.AddMinutes(VerificationCode.ExpirationMinutes);

        var pendingVerification = await _pendingVerificationRepository.GetByEmailAsync(emailString, ct);

        if (pendingVerification != null)
        {
            pendingVerification.PasswordHash = hashedPassword;
            pendingVerification.VerificationCode = verificationCode;
            pendingVerification.ExpiresAt = expirationTime;
            pendingVerification.CreatedAt = DateTime.UtcNow;
            _pendingVerificationRepository.Update(pendingVerification);
        }
        else
        {
            pendingVerification = new PendingVerification
            {
                Id = Guid.NewGuid(),
                Email = emailString,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expirationTime,
                VerificationCode = verificationCode
            };

            _pendingVerificationRepository.Add(pendingVerification);
        }

        await _userRepository.UnitOfWork.SaveChangesAsync(ct);
        await _emailService.SendVerificationEmailAsync(emailString.NormalizedValue, verificationCode);

        return Result<RegisterResponse>.Success(new RegisterResponse("Verification code sent to your email."));
    }
}