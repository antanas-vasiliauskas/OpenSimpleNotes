using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using OSN.Application.Features.Auth.Login;
using OSN.Application.Features.Auth.Register;
using OSN.Application.Features.Auth.GoogleSignIn;
using OSN.Application.Features.Auth.AnonymousLogin;
using OSN.Application.Features.Auth.Verify;
using OSN.Application.Features.Auth.VerifyResend;

namespace OSN.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("AuthPolicy")]
public class AuthController: ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) 
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return Unauthorized(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("verify")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail(VerifyEmailCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("verify-resend")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyResend(VerifyResendCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("google-signin")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleSignIn(GoogleSignInCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("anonymous-login")]
    [AllowAnonymous]
    public async Task<IActionResult> AnonymousLogin(AnonymousLoginCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }
}