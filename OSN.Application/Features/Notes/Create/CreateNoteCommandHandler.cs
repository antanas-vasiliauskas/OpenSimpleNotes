using MediatR;
using OSN.Application.Repositories;
using OSN.Application.Services;
using OSN.Domain.Models;

namespace OSN.Application.Features.Notes.Create;

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, Result<NoteResponse>>
{
    private readonly INoteRepository _noteRepository;
    private readonly ICurrentUserService _currentUser;

    public CreateNoteCommandHandler(INoteRepository noteRepository, ICurrentUserService currentUser)
    {
        _noteRepository = noteRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<NoteResponse>> Handle(CreateNoteCommand command, CancellationToken ct)
    {
        var note = new Note
        {
            Title = command.Title,
            Content = command.Content,
            IsPinned = false,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserId = _currentUser.UserId
        };

        _noteRepository.Add(note);
        await _noteRepository.UnitOfWork.SaveChangesAsync(ct);

        return Result<NoteResponse>.Success(new NoteResponse(note.Id, note.Title, note.Content, note.IsPinned, note.CreatedAt, note.UpdatedAt));
    }
}