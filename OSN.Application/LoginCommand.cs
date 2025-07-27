using MediatR;

namespace OSN.Application;

public record LoginCommand(LoginRequest Request): IRequest<Result<LoginResponse>>;