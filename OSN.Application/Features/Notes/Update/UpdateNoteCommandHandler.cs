using MediatR;
using Microsoft.EntityFrameworkCore;
using OSN.Infrastructure;

namespace OSN.Application.Features.Notes.Update;

public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, Result<NoteResponse>>
{
    private readonly AppDbContext _db; // TODO: replace with repository
    private readonly ICurrentUserService _currentUser;

    public UpdateNoteCommandHandler(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<NoteResponse>> Handle(UpdateNoteCommand command, CancellationToken ct)
    {
        var note = await _db.Notes
            .FirstOrDefaultAsync(n => n.Id == command.Id && n.UserId == _currentUser.UserId && !n.IsDeleted, ct)
            ?? throw new NotFoundException("Note not found.");

        note.Title = command.Request.Title ?? note.Title;
        note.Content = command.Request.Content ?? note.Content;
        note.IsPinned = command.Request.IsPinned ?? note.IsPinned;
        note.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        return Result<NoteResponse>.Success(new NoteResponse(note.Id, note.Title, note.Content, note.IsPinned, note.CreatedAt, note.UpdatedAt));
    }
}
