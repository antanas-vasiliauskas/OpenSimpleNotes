using MediatR;

namespace OSN.Application.Features.Auth.AnonymousLogin;

[AllowAnonymousCommand]
public record AnonymousLoginCommand(AnonymousLoginRequest Request) : IRequest<Result<AnonymousLoginResponse>>;