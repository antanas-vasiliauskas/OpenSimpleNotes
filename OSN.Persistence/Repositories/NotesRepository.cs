using Microsoft.EntityFrameworkCore;
using OSN.Domain.Daos;
using OSN.Domain.Filters.Note;
using OSN.Domain.Interfaces;

namespace OSN.Persistence.Repositories;
public class NotesRepository : INotesRepository
{
    private readonly AppDbContext _context;

    public NotesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<NoteDao> CreateAsync(NoteDao noteDao)
    {
        noteDao = _context.CreateProxy<NoteDao>(noteDao);

        var entityEntry = await _context.Notes.AddAsync(noteDao);
        await _context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async Task<IEnumerable<NoteDao>> GetAllAsync()
    {
        return await _context.Notes
            .Where(n => !n.IsDeleted)
            .Include(n => n.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<NoteDao>> GetAllAsync(GetAllNotesFilter filter)
    {
        return await _context.Notes
            .Where(n => !n.IsDeleted)
            .Where(n => n.UserId == filter.UserId)
            .Include(n => n.User)
            .ToListAsync();
    }

    public async Task<NoteDao?> GetAsync(Guid id)
    {
        var orderDao = await _context.Notes
            .Where(x => x.IsDeleted == false)
            .Include(n => n.User)
            .FirstOrDefaultAsync(x => x.Id == id);

        return orderDao;

    }

    public async Task<NoteDao?> UpdateAsync(NoteDao noteDao)
    {
        var existingNoteDao = await _context.Notes
            .Where(o => !o.IsDeleted && o.Id == noteDao.Id)
            .FirstOrDefaultAsync();

        if (existingNoteDao is null) return null;

        _context.Entry(existingNoteDao).CurrentValues.SetValues(noteDao);
        await _context.SaveChangesAsync();

        return existingNoteDao;
    }
}