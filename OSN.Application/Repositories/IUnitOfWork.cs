namespace OSN.Application.Repositories;

// All repositories using same DbContext should be handled by DbContext scoped lifetime.
public interface IUnitOfWork : IDisposable
{
    Task <int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
