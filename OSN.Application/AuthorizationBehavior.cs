using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Security.Claims;

namespace OSN.Application;

public class AuthorizationBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>
{
    private IHttpContextAccessor _httpContentAccessor;
    public AuthorizationBehavior(IHttpContextAccessor httpContentAccessor)
    {
        _httpContentAccessor = httpContentAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (request.GetType().GetCustomAttribute<AllowAnonymousAttribute>() != null) // [AllowAnonymous]
        {
            return await next();
        }

        var userRole = _httpContentAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole == null) throw new UnauthorizedAccessException("No token or role value found.");

        var authorizeAttribute = request.GetType().GetCustomAttribute<AuthorizeAttribute>();

        if (authorizeAttribute != null && !string.IsNullOrEmpty(authorizeAttribute.Roles)) // [Authorize(Roles = "Admin")]
        {
            var requiredRoles = authorizeAttribute.Roles.Split(',');
            if (requiredRoles.Any(requiredRole => RoleHierarchy.HasPermission(userRole, requiredRole))){
                throw new UnauthorizedAccessException("Forbidden.");
            }
        }
        else if (!RoleHierarchy.HasPermission(userRole, RoleHierarchy.DefaultRole)) // Empty or [Authorize] - check for default role (User)
        {
            throw new UnauthorizedAccessException("Invalid role.");
        }


        return await next();
    }
}