using MediatR;

namespace OSN.Application.Features.Auth.AnonymousLogin;
public record AnonymousLoginCommand(Guid? GuestId) : IRequest<Result<AnonymousLoginResponse>>;