using Microsoft.EntityFrameworkCore;
using OSN.Application.Repositories;
using OSN.Domain.Models;

namespace OSN.Infrastructure.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly AppDbContext _db;
    public IUnitOfWork UnitOfWork => _db; // just expose save async

    public NoteRepository(AppDbContext db)
    {
        _db = db;
    }

    public void Add(Note note)
    {
        _db.Notes.Add(note);
    }

    public async Task<Note?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _db.Notes
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId && !n.IsDeleted, cancellationToken);
    }


    public async Task<List<Note>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _db.Notes
            .Where(n => n.UserId == userId && !n.IsDeleted)
            .OrderByDescending(n => n.IsPinned)
            .ThenByDescending(n => n.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public void Update(Note note)
    {
        _db.Notes.Update(note);
    }
}