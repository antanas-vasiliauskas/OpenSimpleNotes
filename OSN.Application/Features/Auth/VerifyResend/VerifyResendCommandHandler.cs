using MediatR;
using OSN.Application.Repositories;
using OSN.Application.Services;
using OSN.Application.Utils;
using OSN.Domain.ValueObjects;

namespace OSN.Application.Features.Auth.VerifyResend;

public class VerifyResendCommandHandler : IRequestHandler<VerifyResendCommand, Result<VerifyResendResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPendingVerificationRepository _pendingVerificationRepository;
    private readonly IEmailService _emailService;

    public VerifyResendCommandHandler(IUserRepository userRepository, IPendingVerificationRepository pendingVerificationRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _pendingVerificationRepository = pendingVerificationRepository;
        _emailService = emailService;
    }

    public async Task<Result<VerifyResendResponse>> Handle(VerifyResendCommand command, CancellationToken ct)
    {
        var emailString = EmailString.Create(command.Email);

        var existingUser = await _userRepository.GetUserByEmailAsync(emailString, ct);
        if (existingUser != null)
        {
            return Result<VerifyResendResponse>.Failure("User with this email is already verified.");
        }

        var pendingVerification = await _pendingVerificationRepository.GetByEmailAsync(emailString, ct);

        if (pendingVerification == null)
        {
            return Result<VerifyResendResponse>.Failure("No pending verification found for this email. Please register first.");
        }

        var newVerificationCode = VerificationCode.Generate();
        var newExpirationTime = DateTime.UtcNow.AddMinutes(VerificationCode.ExpirationMinutes);

        pendingVerification.VerificationCode = newVerificationCode;
        pendingVerification.ExpiresAt = newExpirationTime;

        _pendingVerificationRepository.Update(pendingVerification);
        await _pendingVerificationRepository.UnitOfWork.SaveChangesAsync(ct);

        try
        {
            await _emailService.SendVerificationEmailAsync(emailString.NormalizedValue, newVerificationCode);
        }
        catch (Exception ex)
        {
            return Result<VerifyResendResponse>.Failure($"Failed to resend verification email: {ex.Message}");
        }

        return Result<VerifyResendResponse>.Success(new VerifyResendResponse("Verification code resent. Please check your email."));
    }
}