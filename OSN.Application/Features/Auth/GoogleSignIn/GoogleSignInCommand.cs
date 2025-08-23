using MediatR;

namespace OSN.Application.Features.Auth.GoogleSignIn;
public record GoogleSignInCommand(string AuthorizationCode, string RedirectUri) : IRequest<Result<GoogleSignInResponse>>;