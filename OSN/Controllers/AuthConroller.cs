using MediatR;
using Microsoft.AspNetCore.Mvc;
using OSN.Application.Features.Auth.Commands.Login;
using OSN.Application.Models.Requests.Auth;
using OSN.Application.Models.Responses.Auth;
using OSN.Configuration.Attributes;
using OSN.Swagger.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace OSN.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _contextAccessor;

    public AuthController(IMediator mediator, IHttpContextAccessor contextAccessor)
    {
        _mediator = mediator;
        _contextAccessor = contextAccessor;
    }


    [HttpPost($"login")]
    [ExcludeHeader]
    [SwaggerRequestType(typeof(LoginRequest))]
    [SwaggerResponse(StatusCodes.Status200OK, "Token successfully generated.", typeof(LoginResponse))]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request);

        var response = await _mediator.Send(command);

        if (response is LoginResponse loginResponse)
        {
            var cookieOptions = new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = loginResponse.Expires };

            Response.Cookies.Append("auth-token", loginResponse.Token, cookieOptions);
        }

        return Ok(response);
    }
}
