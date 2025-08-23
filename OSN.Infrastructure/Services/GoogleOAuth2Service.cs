using Microsoft.Extensions.Configuration;
using System.Text.Json;
using OSN.Application.Services;
using Google.Apis.Auth;
using OSN.Application.Dto;

namespace OSN.Infrastructure.Services;

public class GoogleOAuth2Service : IGoogleOAuth2Service
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GoogleOAuth2Service(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GetGoogleIdTokenAsync(string authorizationCode, string redirectUri)
    {
        var clientId = _configuration["Authentication:Google:ClientId"]!;
        var clientSecret = _configuration["Authentication:Google:ClientSecret"]!;

        var tokenRequest = new Dictionary<string, string>
        {
            ["code"] = authorizationCode,
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["redirect_uri"] = redirectUri,
            ["grant_type"] = "authorization_code"
        };

        var tokenResponse = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(tokenRequest));

        if (!tokenResponse.IsSuccessStatusCode)
        {
            var errorContent = await tokenResponse.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Token exchange failed: {tokenResponse.StatusCode}, {errorContent}");
        }

        var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
        var tokenData = JsonSerializer.Deserialize<GoogleTokenResponse>(tokenContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

        if(tokenData == null || string.IsNullOrEmpty(tokenData.IdToken))
        {
            throw new InvalidOperationException("Token data is missing from the response.");
        }

        return tokenData!.IdToken;
    }

    public async Task<GoogleIdTokenDto> ValidateIdTokenAsync(string idToken)
    {
        var clientId = _configuration["Authentication:Google:ClientId"];
        var payload =  await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { clientId }
        });

        if(payload == null)
        {
            throw new InvalidJwtException("Invalid ID token.");
        }
        return new GoogleIdTokenDto(
            payload.Subject, // Google user ID
            payload.Email,
            payload.GivenName, // First name 
            payload.FamilyName, // Last name
            payload.Picture // Profile picture URL
        );
    }

    private class GoogleTokenResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? IdToken { get; set; }
        public string? TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string? Scope { get; set; }
    }
}