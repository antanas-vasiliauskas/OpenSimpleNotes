using MediatR;

namespace OSN.Application.Features.Auth.Login;
public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;