using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace OSN.Infrastructure.Services;

public class CookieService
{
    private const string AuthTokenCookieName = "auth-token";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _environment;

    public CookieService(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
    {
        _httpContextAccessor = httpContextAccessor;
        _environment = environment;
    }

    public void SetAuthCookie(string token, TimeSpan? expiry = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // Prevent XSS attacks
            Secure = !_environment.IsDevelopment(), // Use HTTPS in production
            SameSite = SameSiteMode.Strict, // Prevent CSRF attacks
            Expires = DateTime.UtcNow.Add(expiry ?? TimeSpan.FromHours(24)), // Default 24 hours
            Path = "/",
            IsEssential = true
        };

        httpContext.Response.Cookies.Append(AuthTokenCookieName, token, cookieOptions);
    }

    public void RemoveAuthCookie()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !_environment.IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(-1), // Expire immediately
            Path = "/",
            IsEssential = true
        };

        httpContext.Response.Cookies.Append(AuthTokenCookieName, "", cookieOptions);
    }

    public string? GetAuthToken()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        return httpContext.Request.Cookies[AuthTokenCookieName];
    }
}