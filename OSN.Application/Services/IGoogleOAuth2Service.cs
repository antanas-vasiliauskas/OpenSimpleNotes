using OSN.Application.Dto;

namespace OSN.Application.Services;

public interface IGoogleOAuth2Service
{
    Task<string> GetGoogleIdTokenAsync(string authorizationCode, string redirectUri);
    Task<GoogleIdTokenDto> ValidateIdTokenAsync(string idToken);
}