using Microsoft.AspNetCore.Authentication;

namespace OSN.Infrastructure.Authentication;

public class JwtCookieAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "JwtCookie";
    public string CookieName { get; set; } = "auth-token";
}