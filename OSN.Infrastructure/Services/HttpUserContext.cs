using Microsoft.AspNetCore.Http;
using OSN.Application.TokenConstants;
using OSN.Infrastructure.Interfaces;

namespace OSN.Infrastructure.Services;

public class HttpUserContext : IUserContext
{
    private readonly IHttpContextAccessor _contextAccessor;

    public HttpUserContext(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var value = _contextAccessor.HttpContext?.Items[TokenConstants.UserIdClaim];
            if (value is null) throw new UnauthorizedAccessException("User ID not found in context");

            return Guid.Parse(value.ToString()!);
        }
    }

    public string UserRole
    {
        get
        {
            var value = _contextAccessor.HttpContext?.Items[TokenConstants.UserRoleClaim];
            if (value is null) throw new UnauthorizedAccessException("User role not found in context");

            return value.ToString()!;
        }
    }
}
