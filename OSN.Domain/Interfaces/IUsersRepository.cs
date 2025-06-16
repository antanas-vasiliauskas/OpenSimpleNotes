using OSN.Domain.Daos;

namespace OSN.Domain.Interfaces;

public interface IUsersRepository
{
    Task<UserDao> CreateAsync(UserDao userDao);
    Task<UserDao?> GetByEmailAsync(string email);
    Task<UserDao> UpdateAsync(UserDao userDao);
    Task<bool> ExistsWithEmail(string email);
}