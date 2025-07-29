using MediatR;
using Microsoft.EntityFrameworkCore;
using OSN.Infrastructure;

namespace OSN.Application;

public class DeleteNoteCommandHandler: IRequestHandler<DeleteNoteCommand, Result<Unit>>
{
    private readonly AppDbContext _db; // TODO: replace with repository
    private readonly ICurrentUserService _currentUser;

    public DeleteNoteCommandHandler(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<Unit>> Handle(DeleteNoteCommand command, CancellationToken ct)
    {
        var note = await _db.Notes
            .FirstOrDefaultAsync(n => n.Id == command.Id && n.UserId == _currentUser.UserId && !n.IsDeleted, ct)
            ?? throw new NotFoundException("Note not found.");

        note.IsDeleted = true;
        note.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct); // Entities retrieved from db directly doesn't require _db.Notes.Update call.
        return Result<Unit>.Success(Unit.Value);
    }
}

