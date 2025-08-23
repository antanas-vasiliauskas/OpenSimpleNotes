using MediatR;

namespace OSN.Application.Features.Auth.Verify;
public record VerifyEmailCommand(string Email, string VerificationCode) : IRequest<Result<VerifyEmailResponse>>;