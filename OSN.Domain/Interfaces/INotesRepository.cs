using OSN.Domain.Daos;
using OSN.Domain.Filters.Note;

namespace OSN.Domain.Interfaces;

public interface INotesRepository
{
    Task<NoteDao> CreateAsync(NoteDao noteDao);
    Task<NoteDao?> UpdateAsync(NoteDao noteDao);
    Task<IEnumerable<NoteDao>> GetAllAsync();
    Task<IEnumerable<NoteDao>> GetAllAsync(GetAllNotesFilter filter);
    Task<NoteDao?> GetAsync(Guid id);

    // delete is done through soft delete
}