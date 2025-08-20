using MediatR;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Security.Claims;

namespace OSN.Application;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private IHttpContextAccessor _httpContentAccessor;
    public AuthorizationBehavior(IHttpContextAccessor httpContentAccessor)
    {
        _httpContentAccessor = httpContentAccessor;
    }

    public async Task<TResponse> Handle(TRequest command, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        Console.WriteLine($"AuthorizationBehavior: {command.GetType().Name}");
        if (command.GetType().GetCustomAttribute<AllowAnonymousCommandAttribute>() != null)
        {
            return await next();
        }

        var userRole = _httpContentAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole == null) throw new UnauthorizedAccessException("No token or policy value found.");

        var authorizeAttribute = command.GetType().GetCustomAttribute<AuthorizeCommandAttribute>();

        if (authorizeAttribute != null && !string.IsNullOrEmpty(authorizeAttribute.Policy))
        {
            if (!RoleHierarchy.HasPermission(userRole, authorizeAttribute.Policy))
            {
                throw new UnauthorizedAccessException("Forbidden.");
            }
        }
        else if (!RoleHierarchy.HasPermission(userRole, RoleHierarchy.DefaultPolicy))
        {
            throw new UnauthorizedAccessException("Invalid policy.");
        }

        return await next();
    }
}