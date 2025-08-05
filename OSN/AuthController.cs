using MediatR;
using Microsoft.AspNetCore.Mvc;
using OSN.Application.Features.Auth.Login;

namespace OSN;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) 
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {`
        var command = new LoginCommand(request);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return Unauthorized(new { message = result.Error });
        return Ok(result.Value);
    }
}