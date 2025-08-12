namespace OSN.Application.Features.Auth.GoogleSignIn;

public record GoogleSignInResponse(string Token, string Role, bool IsNewUser);