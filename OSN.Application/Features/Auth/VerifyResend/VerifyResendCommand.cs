using MediatR;

namespace OSN.Application.Features.Auth.VerifyResend;
public record VerifyResendCommand(string Email) : IRequest<Result<VerifyResendResponse>>;