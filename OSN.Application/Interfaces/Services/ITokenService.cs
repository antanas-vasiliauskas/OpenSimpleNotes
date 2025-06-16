using OSN.Application.Models.Dtos;
using OSN.Application.Models.Responses.Auth;
using OSN.Domain.Daos;

namespace OSN.Application.Interfaces.Services;

public interface ITokenService
{
    LoginResponse CreateToken(UserDao userDao);
    TokenDataDto GetTokenData(string token);
    bool IsTokenValid(string token);
}