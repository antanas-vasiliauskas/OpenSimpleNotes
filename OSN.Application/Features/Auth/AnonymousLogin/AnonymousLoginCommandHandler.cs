using MediatR;
using Microsoft.EntityFrameworkCore;
using OSN.Domain.Models;
using OSN.Infrastructure;
using OSN.Infrastructure.Services;

namespace OSN.Application.Features.Auth.AnonymousLogin;

public class AnonymousLoginCommandHandler : IRequestHandler<AnonymousLoginCommand, Result<AnonymousLoginResponse>>
{
    private readonly AppDbContext _db;
    private readonly AuthService _authService;

    public AnonymousLoginCommandHandler(AppDbContext db, AuthService authService)
    {
        _db = db;
        _authService = authService;
    }

    public async Task<Result<AnonymousLoginResponse>> Handle(AnonymousLoginCommand command, CancellationToken ct)
    {
        var request = command.Request;
        User? user;

        if (request.GuestId != null)
        {
            user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.GuestId.Value && u.Email == null && !u.IsDeleted, ct);
            
            if (user == null)
            {
                return Result<AnonymousLoginResponse>.Failure("Invalid guest ID.");
            }
        }
        else
        {   
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = null,
                PasswordHash = string.Empty,
                Role = RoleHierarchy.GuestRole,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
        }

        var token = _authService.GenearateToken(user);

        bool isNewUser = request.GuestId == null;
        return Result<AnonymousLoginResponse>.Success(new AnonymousLoginResponse(token, user.Role, user.Id, isNewUser));
    }
}