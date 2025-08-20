using OSN.Test.Integration.Infrastructure;
using System.Net;
using System.Text;

namespace OSN.Test.Integration.RateLimiting;

public class RateLimitingTests : IClassFixture<TestWebApplicationFactory>
{
    private const int GlobalRateLimit = 500;
    private const int AuthRateLimit = 50;
    private const int GlobalRateOverlimit = (int)(GlobalRateLimit * 1.2);
    private const int AuthRateOverlimit = (int)(AuthRateLimit * 1.2);
    private const string GlobalEndpoint = "/";
    private const string AuthEndpoint = "/api/auth/login";
    private readonly TestWebApplicationFactory _factory;

    public RateLimitingTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData(GlobalEndpoint, GlobalRateOverlimit)]
    [InlineData(AuthEndpoint, AuthRateOverlimit)]
    public async Task RateLimit_ExceedsLimit_ReturnsRateLimited(string endpoint, int overlimit)
    {
        var client = _factory.CreateClientWithRandomIp();
        var responses = new List<HttpResponseMessage>();
        
        for (int i = 0; i < overlimit; i++)
        {
            var response = endpoint.Contains("auth") 
                ? await client.PostAsync(endpoint, new StringContent("{}", Encoding.UTF8, "application/json"))
                : await client.GetAsync(endpoint);
            responses.Add(response);
        }

        var rateLimitedCount = responses.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);
        rateLimitedCount.Should().BeGreaterThan(0);

        foreach (var response in responses) response.Dispose();
        client.Dispose();
    }

    [Theory]
    [InlineData(GlobalEndpoint, GlobalRateLimit)]
    [InlineData(AuthEndpoint, AuthRateLimit)]
    public async Task RateLimit_BelowLimit_ShouldNotRateLimit(string endpoint, int limit)
    {
        var client = _factory.CreateClientWithRandomIp();
        var responses = new List<HttpResponseMessage>();
        
        for (int i = 0; i < limit; i++)
        {
            var response = endpoint.Contains("auth") 
                ? await client.PostAsync(endpoint, new StringContent("{}", Encoding.UTF8, "application/json"))
                : await client.GetAsync(endpoint);
            responses.Add(response);
        }

        var rateLimitedCount = responses.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);
        rateLimitedCount.Should().Be(0);

        foreach (var response in responses) response.Dispose();
        client.Dispose();
    }

    [Fact]
    public async Task AuthPolicy_StricterThan_GlobalPolicy()
    {
        var authClient = _factory.CreateClientWithRandomIp();
        var globalClient = _factory.CreateClientWithRandomIp();

        var authResponses = new List<HttpResponseMessage>();
        var globalResponses = new List<HttpResponseMessage>();

        for (int i = 0; i < AuthRateOverlimit; i++)
        {
            var authResponse = await authClient.PostAsync(AuthEndpoint, 
                new StringContent("{}", Encoding.UTF8, "application/json"));
            authResponses.Add(authResponse);
        }

        for (int i = 0; i < AuthRateOverlimit; i++)
        {
            var globalResponse = await globalClient.GetAsync(GlobalEndpoint);
            globalResponses.Add(globalResponse);
        }

        var authRateLimited = authResponses.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);
        var globalRateLimited = globalResponses.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);

        authRateLimited.Should().BeGreaterThanOrEqualTo(globalRateLimited);
        authRateLimited.Should().BeGreaterThan(0);

        foreach (var response in authResponses.Concat(globalResponses)) response.Dispose();
        authClient.Dispose();
        globalClient.Dispose();
    }

    [Theory]
    [InlineData(GlobalEndpoint, GlobalRateOverlimit)]
    [InlineData(AuthEndpoint, AuthRateOverlimit)]
    public async Task RateLimitResponse_HasCorrectFormat(string endpoint, int overlimit)
    {
        var client = _factory.CreateClientWithRandomIp();
        HttpResponseMessage? rateLimitedResponse = null;

        for (int i = 0; i < overlimit; i++)
        {
            var response = endpoint.Contains("auth") 
                ? await client.PostAsync(endpoint, new StringContent("{}", Encoding.UTF8, "application/json"))
                : await client.GetAsync(endpoint);
                
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                rateLimitedResponse = response;
                break;
            }
            response.Dispose();
        }

        rateLimitedResponse.Should().NotBeNull();
        rateLimitedResponse!.Headers.Should().ContainKey("Retry-After");
        
        var responseContent = await rateLimitedResponse.Content.ReadAsStringAsync();
        responseContent.Should().Be("Too many requests. Please try again later.");

        rateLimitedResponse.Dispose();
        client.Dispose();
    }

    [Theory]
    [InlineData(GlobalEndpoint, GlobalRateOverlimit / 2)]
    [InlineData(AuthEndpoint, AuthRateOverlimit / 2)]
    public async Task DifferentIpAddresses_ShouldHaveSeparateRateLimits(string endpoint, int requestCount)
    {
        var client1 = _factory.CreateClientWithRandomIp();
        var client2 = _factory.CreateClientWithRandomIp();

        var responses1 = new List<HttpResponseMessage>();
        var responses2 = new List<HttpResponseMessage>();

        for (int i = 0; i < requestCount; i++)
        {
            var response1 = endpoint.Contains("auth") 
                ? await client1.PostAsync(endpoint, new StringContent("{}", Encoding.UTF8, "application/json"))
                : await client1.GetAsync(endpoint);
            responses1.Add(response1);
        }

        for (int i = 0; i < requestCount; i++)
        {
            var response2 = endpoint.Contains("auth") 
                ? await client2.PostAsync(endpoint, new StringContent("{}", Encoding.UTF8, "application/json"))
                : await client2.GetAsync(endpoint);
            responses2.Add(response2);
        }

        var rateLimited1 = responses1.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);
        var rateLimited2 = responses2.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);

        rateLimited1.Should().Be(0);
        rateLimited2.Should().Be(0);

        foreach (var response in responses1.Concat(responses2)) response.Dispose();
        client1.Dispose();
        client2.Dispose();
    }
}