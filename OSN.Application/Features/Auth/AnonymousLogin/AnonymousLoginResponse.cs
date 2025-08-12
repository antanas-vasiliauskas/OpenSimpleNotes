namespace OSN.Application.Features.Auth.AnonymousLogin;
public record AnonymousLoginResponse(string Token, string Role, Guid GuestId, bool isNewUser);