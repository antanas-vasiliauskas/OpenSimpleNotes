using OSN.Application.Models.Dtos;
using OSN.Application.Models.Responses.Auth;

namespace OSN.Application.Interfaces.ModelsResolvers;
public interface IAuthModelsResolver
{
    LoginResponse GetResponse(string token, DateTime expires, TokenDataDto tokenDataDto);
}