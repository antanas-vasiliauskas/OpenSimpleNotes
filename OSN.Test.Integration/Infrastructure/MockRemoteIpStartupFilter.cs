using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace OSN.Test.Integration.Infrastructure;

public class MockRemoteIpStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.UseMiddleware<MockRemoteIpAddressMiddleware>();
            next(app);
        };
    }
}