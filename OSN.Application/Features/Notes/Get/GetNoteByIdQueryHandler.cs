using MediatR;
using OSN.Application.Repositories;
using OSN.Application.Services;

namespace OSN.Application.Features.Notes.Get;

public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, Result<NoteResponse>>
{
    private readonly INoteRepository _noteRepository;
    private readonly ICurrentUserService _currentUser;

    public GetNoteByIdQueryHandler(INoteRepository noteRepository, ICurrentUserService currentUser)
    {
        _noteRepository = noteRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<NoteResponse>> Handle(GetNoteByIdQuery query, CancellationToken ct)
    {
        var note = await _noteRepository.GetByIdAsync(query.Id, _currentUser.UserId, ct);
        if (note == null)
        {
            return Result<NoteResponse>.Failure("Note not found.");
        }

        return Result<NoteResponse>.Success(
            new NoteResponse(note.Id, note.Title, note.Content, note.IsPinned, note.CreatedAt, note.UpdatedAt)
        );
    }
}