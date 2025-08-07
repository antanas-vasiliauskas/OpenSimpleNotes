using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace OSN.Infrastructure.Services;

public class GoogleOAuth2Service
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GoogleOAuth2Service(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string?> ExchangeAuthorizationCodeAsync(string authorizationCode, string redirectUri)
    {
        try
        {
            var clientId = _configuration["Authentication:Google:ClientId"];
            var clientSecret = _configuration["Authentication:Google:ClientSecret"];

            // Exchange authorization code for tokens
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

            return tokenData?.IdToken;
        }
        catch (Exception)
        {
            // Re-throw with original exception for debugging
            throw;
        }
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