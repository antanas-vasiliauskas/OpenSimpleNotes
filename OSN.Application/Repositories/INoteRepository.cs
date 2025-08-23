using OSN.Domain.Models;

namespace OSN.Application.Repositories;

public interface INoteRepository : IRepository
{
    void Add(Note note);
    Task<Note?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<List<Note>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
    void Update(Note note);
}