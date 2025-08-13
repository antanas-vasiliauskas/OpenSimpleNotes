using MediatR;

namespace OSN.Application.Features.Auth.VerifyResend;

[AllowAnonymousCommand]
public record VerifyResendCommand(VerifyResendRequest Request) : IRequest<Result<VerifyResendResponse>>;