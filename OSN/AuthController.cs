using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OSN.Application.Features.Auth.Login;
using OSN.Application.Features.Auth.Register;
using OSN.Application.Features.Auth.GoogleSignIn;
using OSN.Infrastructure.Services;

namespace OSN;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly IMediator _mediator;
    private readonly AuthService _authService;

    public AuthController(IMediator mediator, AuthService authService) 
    {
        _mediator = mediator;
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var command = new LoginCommand(request);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return Unauthorized(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(request);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("google-signin")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleSignIn(GoogleSignInRequest request)
    {
        var command = new GoogleSignInCommand(request);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        _authService.RemoveAuthenticationCookie();
        return Ok(new { message = "Logged out successfully" });
    }
}