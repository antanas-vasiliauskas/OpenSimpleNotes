using MediatR;
using OSN.Application.Repositories;
using OSN.Application.Services;

namespace OSN.Application.Features.Notes.Delete;

public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand, Result<Unit>>
{
    private readonly INoteRepository _noteRepository;
    private readonly ICurrentUserService _currentUser;

    public DeleteNoteCommandHandler(INoteRepository noteRepository, ICurrentUserService currentUser)
    {
        _noteRepository = noteRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<Unit>> Handle(DeleteNoteCommand command, CancellationToken ct)
    {
        // Enforce authorization in data layer by passing current user ID
        var note = await _noteRepository.GetByIdAsync(command.Id, _currentUser.UserId, ct);

        if(note == null)
        {
            return Result<Unit>.Failure("Note not found.");
        }

        note.IsDeleted = true;
        note.UpdatedAt = DateTime.UtcNow;

        _noteRepository.Update(note);
        await _noteRepository.UnitOfWork.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}

