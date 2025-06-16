using OSN.Domain.Daos;
using OSN.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace OSN.Persistence.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly AppDbContext _context;

    public UsersRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDao> CreateAsync(UserDao userDao)
    {
        userDao = _context.CreateProxy<UserDao>(userDao);

        var entityEntry = await _context.AddAsync(userDao);
        await _context.SaveChangesAsync();

        return entityEntry.Entity;
    }


    public async Task<UserDao?> GetByEmailAsync(string email)
    {
        var userDao = _context.ChangeTracker.Entries<UserDao>().FirstOrDefault(x => x.Entity.Email == email && !x.Entity.IsDeleted)?.Entity;

        userDao ??= await _context.Users.FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted);

        if (userDao is null || userDao.IsDeleted)
        {
            return null;
        }

        return userDao;
    }

    public async Task<UserDao> UpdateAsync(UserDao userDao)
    {
        await RemoveAsync(userDao.Id);
        _context.Users.Update(userDao);
        await _context.SaveChangesAsync();

        return userDao;
    }

    public async Task<bool> ExistsWithEmail(string email)
    {
        return await _context.Users.AnyAsync(user => user.Email.ToLower() == email.ToLower() && !user.IsDeleted);
    }

    private async Task RemoveAsync(Guid id)
    {
        var userDao = await _context.Users.FindAsync(id);
        _context.Users.Remove(userDao!);
    }
}