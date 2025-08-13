using MediatR;
using OSN.Domain.Models;
using OSN.Infrastructure;

namespace OSN.Application.Features.Notes.Create;

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, Result<NoteResponse>>
{
    private readonly AppDbContext _db; // TODO: replace with repository
    private readonly ICurrentUserService _currentUser;

    public CreateNoteCommandHandler(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<NoteResponse>> Handle(CreateNoteCommand command, CancellationToken ct)
    {
        var note = new Note
        {
            Title = command.Request.Title,
            Content = command.Request.Content,
            IsPinned = false,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserId = _currentUser.UserId
        };

        _db.Notes.Add(note);
        await _db.SaveChangesAsync(ct);

        return Result<NoteResponse>.Success(new NoteResponse(note.Id, note.Title, note.Content, note.IsPinned, note.CreatedAt, note.UpdatedAt));
    }
}