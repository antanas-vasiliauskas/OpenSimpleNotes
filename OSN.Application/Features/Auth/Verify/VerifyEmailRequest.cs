namespace OSN.Application.Features.Auth.Verify;

public record VerifyEmailRequest(string Email, string VerificationCode);