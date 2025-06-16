using MediatR;
using Microsoft.AspNetCore.Http;
using OSN.Application.Interfaces.Markers;
using OSN.Application.Interfaces.Services;
using OSN.Application.TokenConstants;

namespace OSN.Infrastructure.Utilities.Behaviours;

public class AuthenticationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ITokenService _tokenService;
    private readonly IHttpContextAccessor _contextAccessor;

    public AuthenticationBehavior(ITokenService tokenService, IHttpContextAccessor contextAccessor)
    {
        _tokenService = tokenService;
        _contextAccessor = contextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var httpRequest = _contextAccessor.HttpContext?.Request;
        if (httpRequest is null)
            throw new Exception("Missing HTTP context");

        if (typeof(TRequest).GetInterfaces().Contains(typeof(ISkipAuthentication)))
        {
            return await next();
        }

        if (!httpRequest.Headers.TryGetValue(TokenConstants.TokenHeader, out var token) || token.Count == 0)
        {
            throw new Exception("Token not provided");
        }


        var tokenString = token[0]!;
        if (!tokenString.StartsWith(TokenConstants.TokenPrefix)
            || !_tokenService.IsTokenValid(tokenString[TokenConstants.TokenPrefix.Length..]))
        {
            throw new Exception("Invalid token provided");
        }


        StoreTokenData(tokenString[TokenConstants.TokenPrefix.Length..]);
        return await next();
    }

    private void StoreTokenData(string token)
    {
        var tokenData = _tokenService.GetTokenData(token);

        var context = _contextAccessor.HttpContext;
        if (context == null)
            throw new Exception("HTTP context is unavailable");

        context.Items[TokenConstants.UserIdClaim] = tokenData.UserId;
        context.Items[TokenConstants.UserRoleClaim] = tokenData.UserRole;
    }
}