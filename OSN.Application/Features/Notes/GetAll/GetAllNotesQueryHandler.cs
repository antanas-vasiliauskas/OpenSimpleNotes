using MediatR;
using OSN.Application.Repositories;
using OSN.Application.Services;

namespace OSN.Application.Features.Notes.GetAll;

public class GetAllNotesQueryHandler : IRequestHandler<GetAllNotesQuery, Result<List<NoteResponse>>>
{
    private readonly INoteRepository _noteRepository;
    private readonly ICurrentUserService _currentUser;

    public GetAllNotesQueryHandler(INoteRepository noteRepository, ICurrentUserService currentUser)
    {
        _noteRepository = noteRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<List<NoteResponse>>> Handle(GetAllNotesQuery query, CancellationToken ct)
    {
        var notes = await _noteRepository.GetAllAsync(_currentUser.UserId, ct);
        var noteResponses = notes.Select(note => 
            new NoteResponse(note.Id, note.Title, note.Content, note.IsPinned, note.CreatedAt, note.UpdatedAt)
        ).ToList();
        return Result<List<NoteResponse>>.Success(noteResponses);
    }
}