using Microsoft.AspNetCore.Http.Features;
using System.Net;

namespace OSN.Test.Integration.Infrastructure;

public class MockHttpConnectionFeature : IHttpConnectionFeature
{
    public MockHttpConnectionFeature(IPAddress remoteIpAddress)
    {
        RemoteIpAddress = remoteIpAddress;
        LocalIpAddress = IPAddress.Loopback;
        RemotePort = 80;
        LocalPort = 5000;
        ConnectionId = Guid.NewGuid().ToString();
    }

    public string ConnectionId { get; set; }
    public IPAddress? RemoteIpAddress { get; set; }
    public int RemotePort { get; set; }
    public IPAddress? LocalIpAddress { get; set; }
    public int LocalPort { get; set; }
}