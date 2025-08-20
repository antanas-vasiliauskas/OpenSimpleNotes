using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Net;

namespace OSN.Test.Integration.Infrastructure;

public class MockRemoteIpAddressMiddleware
{
    private readonly RequestDelegate _next;

    public MockRemoteIpAddressMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Test-Remote-IP", out var ipAddress))
        {
            if (IPAddress.TryParse(ipAddress.ToString(), out var parsedIp))
            {
                var connectionFeature = context.Features.Get<IHttpConnectionFeature>();
                if (connectionFeature != null)
                {
                    connectionFeature.RemoteIpAddress = parsedIp;
                }
                else
                {
                    var mockConnectionFeature = new MockHttpConnectionFeature(parsedIp);
                    context.Features.Set<IHttpConnectionFeature>(mockConnectionFeature);
                }
            }
        }

        await _next(context);
    }
}