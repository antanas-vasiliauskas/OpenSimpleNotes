using MediatR;

using Microsoft.EntityFrameworkCore;
using OSN.Infrastructure;
using OSN.Infrastructure.Services;

namespace OSN.Application;

public class LoginCommandHandler: IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    // Obviously later add IUserRepository and IAuthService, so that no Infrastructure reference.
    // Then remove infrastrucutre from dependencies and add Application to dependencies on infrastructure.
    private readonly AppDbContext _db;
    private readonly AuthService _authService;
    private readonly PasswordHasher _passwordHasher;

    public LoginCommandHandler(AppDbContext db, AuthService authService, PasswordHasher passwordHasher)
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
            return Result<LoginResponse>.Failure("Invalid email."); // being precise whether problem is with username or password is a slight security risk
        }

        var verificationSuccess = _passwordHasher.VerifySHA256Password(user.PasswordHash, request.Password);

        if(!verificationSuccess)
        {
            return Result<LoginResponse>.Failure("Invalid password.");
        }

        var token = _authService.GenearateToken(user);

        return Result<LoginResponse>.Success(new LoginResponse(token, user.Role));
    }
}