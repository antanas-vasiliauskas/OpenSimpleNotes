using System.Net;

namespace OSN.Test.Integration.Infrastructure;

public static class IpAddressHelper
{
    private static readonly Random _random = new();
    
    public static IPAddress GenerateRandomIpAddress()
    {
        var bytes = new byte[4];
        _random.NextBytes(bytes);
        bytes[0] = 192;
        bytes[1] = 168;
        bytes[2] = (byte)_random.Next(1, 255);
        bytes[3] = (byte)_random.Next(1, 255);
        return new IPAddress(bytes);
    }

    public static HttpClient CreateClientWithRandomIp(this TestWebApplicationFactory factory)
    {
        var client = factory.CreateClient();
        var randomIp = GenerateRandomIpAddress();
        client.DefaultRequestHeaders.Add("X-Test-Remote-IP", randomIp.ToString());
        return client;
    }
}