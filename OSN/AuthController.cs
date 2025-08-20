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

namespace OSN;

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

    [HttpPost("verify")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request)
    {
        var command = new VerifyEmailCommand(request);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("verify-resend")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyResend(VerifyResendRequest request)
    {
        var command = new VerifyResendCommand(request);
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

    [HttpPost("anonymous-login")]
    [AllowAnonymous]
    public async Task<IActionResult> AnonymousLogin(AnonymousLoginRequest request)
    {
        var command = new AnonymousLoginCommand(request);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }
}