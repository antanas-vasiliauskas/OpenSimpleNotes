using OSN.Application.Interfaces.ModelsResolvers;
using OSN.Application.Models.Dtos;
using OSN.Application.Models.Responses.Auth;

namespace OSN.Application.Mappings.ModelsResolvers;

public class AuthModelsResolver : IAuthModelsResolver
{
    public LoginResponse GetResponse(string token, DateTime expires, TokenDataDto tokenDataDto)
    {
        return new()
        {
            Token = token,
            Expires = expires,
            UserId = Guid.Parse(tokenDataDto.UserId),
            Role = tokenDataDto.UserRole
        };
    }
}