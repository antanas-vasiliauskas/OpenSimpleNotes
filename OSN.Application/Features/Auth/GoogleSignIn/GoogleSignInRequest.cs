namespace OSN.Application.Features.Auth.GoogleSignIn;

public record GoogleSignInRequest(string AuthorizationCode, string RedirectUri);