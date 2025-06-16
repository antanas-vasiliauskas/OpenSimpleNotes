using MediatR;
using OSN.Application.Interfaces.Services;
using OSN.Application.Models.Responses.Auth;
using OSN.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace OSN.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IUsersRepository usersRepository, ITokenService tokenService)
    {
        _usersRepository = usersRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var loginRequest = command.Request;

        var user = await _usersRepository.GetByEmailAsync(loginRequest.Email);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid credentials");

        
        var hashedPassword = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(loginRequest.Password))
        );

        if (user.Password != hashedPassword)
            throw new UnauthorizedAccessException("Invalid credentials");

        return _tokenService.CreateToken(user);
    }
}