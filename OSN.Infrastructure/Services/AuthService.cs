using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OSN.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OSN.Infrastructure.Services;

public class AuthService
{
    private readonly IConfiguration _config;
    private readonly CookieService _cookieService;

    public AuthService(IConfiguration config, CookieService cookieService) 
    {
        _config = config;
        _cookieService = cookieService;
    }

    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenExpiry = TimeSpan.FromMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"]));
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.Add(tokenExpiry),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public void SetAuthenticationCookie(User user)
    {
        var token = GenerateToken(user);
        var tokenExpiry = TimeSpan.FromMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"]));
        _cookieService.SetAuthCookie(token, tokenExpiry);
    }

    public void RemoveAuthenticationCookie()
    {
        _cookieService.RemoveAuthCookie();
    }

    // Keep this for backward compatibility or testing
    [Obsolete("Use SetAuthenticationCookie instead for secure cookie-based authentication")]
    public string GenearateToken(User user) => GenerateToken(user);
}