using MediatR;

namespace OSN.Application.Features.Auth.Login;

public record LoginCommand(LoginRequest Request) : IRequest<Result<LoginResponse>>;