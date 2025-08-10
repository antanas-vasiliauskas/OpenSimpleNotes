namespace OSN.Application.Features.Auth.GoogleSignIn;

public record GoogleSignInResponse(string role, bool IsNewUser);