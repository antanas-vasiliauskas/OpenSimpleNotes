using MediatR;

namespace OSN.Application.Features.Auth.GoogleSignIn;

[AllowAnonymousCommand]
public record GoogleSignInCommand(GoogleSignInRequest Request) : IRequest<Result<GoogleSignInResponse>>;