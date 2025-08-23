using OSN.Domain.Models;
using OSN.Domain.ValueObjects;

namespace OSN.Application.Repositories;

public interface IUserRepository: IRepository
{
    Task<User?> GetUserByIdAsync(Guid guestId, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(EmailString email, CancellationToken cancellationToken = default);
    void Add(User user);
}