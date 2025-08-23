using Microsoft.EntityFrameworkCore;
using OSN.Application.Repositories;
using OSN.Domain.Models;
using OSN.Domain.ValueObjects;

namespace OSN.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public IUnitOfWork UnitOfWork => _db; // just expose save async

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
    }

    public async Task<User?> GetUserByEmailAsync(EmailString email, CancellationToken cancellationToken = default)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
    }

    public void Add(User user)
    {
        _db.Users.Add(user);
    }
}