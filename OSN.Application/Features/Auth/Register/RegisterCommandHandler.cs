using MediatR;
using Microsoft.EntityFrameworkCore;
using OSN.Domain.Models;
using OSN.Infrastructure;
using OSN.Infrastructure.Services;

namespace OSN.Application.Features.Auth.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly AppDbContext _db;
    private readonly AuthService _authService;
    private readonly PasswordHasher _passwordHasher;

    public RegisterCommandHandler(AppDbContext db, AuthService authService, PasswordHasher passwordHasher)
    {
        _db = db;
        _authService = authService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand command, CancellationToken ct)
    {
        var request = command.Request;

        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower(), ct);
        if (existingUser != null)
        {
            if (existingUser.GoogleSignIn != null)
            {
                return Result<RegisterResponse>.Failure("This email is linked to a Google account. Please sign in with Google instead.");
            }
            return Result<RegisterResponse>.Failure("User with this email already exists.");
        }

        var hashedPassword = _passwordHasher.HashSHA256Password(request.Password);

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = hashedPassword,
            Role = RoleHierarchy.UserRole,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync(ct);

        // Set authentication cookie instead of returning token
        _authService.SetAuthenticationCookie(newUser);

        return Result<RegisterResponse>.Success(new RegisterResponse(newUser.Role));
    }
}