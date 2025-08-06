using MediatR;

namespace OSN.Application.Features.Auth.Register;

[AllowAnonymousCommand]
public record RegisterCommand(RegisterRequest Request) : IRequest<Result<RegisterResponse>>;