using MediatR;
using OSN.Application.Repositories;
using OSN.Application.Services;
using OSN.Domain.ValueObjects;

namespace OSN.Application.Features.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(IUserRepository userRepository, IAuthService authService, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _authService = authService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken ct)
    {
        var emailString = EmailString.Create(command.Email);

        var user = await _userRepository.GetUserByEmailAsync(emailString, ct);

        if (user == null)
        {
            return Result<LoginResponse>.Failure("Invalid email or password."); // User not found.
        }

        var verificationSuccess = _passwordHasher.VerifyPassword(user.PasswordHash, command.Password);

        if (!verificationSuccess)
        {
            return Result<LoginResponse>.Failure("Invalid email or password."); // Invalid password.
        }

        var token = _authService.GenearateJwtToken(user);
        return Result<LoginResponse>.Success(new LoginResponse(token, user.Role));
    }
}