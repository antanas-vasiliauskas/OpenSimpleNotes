using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OSN.Domain.Models;
using OSN.Infrastructure;
using OSN.Infrastructure.Services;
using System.Runtime.InteropServices;

namespace OSN.Application;

public class LoginCommandHandler: IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    // Obviously later add IUserRepository and IAuthService, so that no Infrastructure reference.
    // Then remove infrastrucutre from dependencies and add Application to dependencies on infrastructure.
    private readonly AppDbContext _db;
    private readonly AuthService _authService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public LoginCommandHandler(AppDbContext db, AuthService authService, IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _authService = authService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken ct)
    {
        var request = command.Request;
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower(), ct);

        if(user == null)
        {
            return Result<LoginResponse>.Failure("Invalid username or password");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if(verificationResult != PasswordVerificationResult.Success)
        {
            return Result<LoginResponse>.Failure("Invalid username or password");
        }

        var token = _authService.GenearateToken(user);

        return Result<LoginResponse>.Success(new LoginResponse(token, user.Role));
    }
}