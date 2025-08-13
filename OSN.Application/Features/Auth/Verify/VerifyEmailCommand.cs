using MediatR;

namespace OSN.Application.Features.Auth.Verify;

[AllowAnonymousCommand]
public record VerifyEmailCommand(VerifyEmailRequest Request) : IRequest<Result<VerifyEmailResponse>>;