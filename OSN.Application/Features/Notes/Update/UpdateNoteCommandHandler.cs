using MediatR;
using OSN.Application.Repositories;
using OSN.Application.Services;

namespace OSN.Application.Features.Notes.Update;

public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, Result<NoteResponse>>
{
    private readonly INoteRepository _noteRepository;
    private readonly ICurrentUserService _currentUser;

    public UpdateNoteCommandHandler(INoteRepository noteRepository, ICurrentUserService currentUser)
    {
        _noteRepository = noteRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<NoteResponse>> Handle(UpdateNoteCommand command, CancellationToken ct)
    {
        var note = await _noteRepository.GetByIdAsync(command.Id, _currentUser.UserId, ct);

        if (note == null)
        {
            return Result<NoteResponse>.Failure("Note not found.");
        }

        note.Title = command.Title ?? note.Title;
        note.Content = command.Content ?? note.Content;
        note.IsPinned = command.IsPinned ?? note.IsPinned;
        note.UpdatedAt = DateTime.UtcNow;

        _noteRepository.Update(note);
        await _noteRepository.UnitOfWork.SaveChangesAsync(ct);

        return Result<NoteResponse>.Success(new NoteResponse(note.Id, note.Title, note.Content, note.IsPinned, note.CreatedAt, note.UpdatedAt));
    }
}
