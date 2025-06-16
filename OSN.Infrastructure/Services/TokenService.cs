using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OSN.Application.Interfaces.ModelsResolvers;
using OSN.Application.Interfaces.Services;
using OSN.Application.Models.Dtos;
using OSN.Application.Models.Responses.Auth;
using OSN.Application.TokenConstants;
using OSN.Domain.Daos;
using OSN.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OSN.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IAuthModelsResolver _authModelsResolver;

    public TokenService(IConfiguration configuration, IAuthModelsResolver authModelsResolver)
    {
        _configuration = configuration;
        _authModelsResolver = authModelsResolver;
    }

    public LoginResponse CreateToken(UserDao userDao)
    {
        var expires = DateTime.UtcNow.AddHours(TokenConstants.TokenDurationInHours);
        var tokenDataDto = new TokenDataDto(userDao.Id.ToString(), userDao.Role);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(TokenConstants.UserIdClaim, tokenDataDto.UserId),
                new Claim(TokenConstants.UserRoleClaim, tokenDataDto.UserRole.ToString()),
            ]),
            Expires = expires,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return _authModelsResolver.GetResponse(tokenHandler.WriteToken(token), expires, tokenDataDto);
    }

    public TokenDataDto GetTokenData(string token)
    {
        var claims = ValidateToken(token, out var validatedToken);

        var userRole = claims.Claims.First(x => x.Type == TokenConstants.UserRoleClaim).Value;
        var userId = claims.Claims.First(x => x.Type == TokenConstants.UserIdClaim).Value;

        return new(userId, Enum.Parse<UserRole>(userRole));
    }

    public bool IsTokenValid(string token)
    {
        try
        {
            ValidateToken(token, out var validatedToken);

            return validatedToken.ValidTo > DateTime.UtcNow;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private ClaimsPrincipal ValidateToken(string token, out SecurityToken validatedToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();

        return tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
    }

    private TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters()
        {
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!))
        };
    }
}