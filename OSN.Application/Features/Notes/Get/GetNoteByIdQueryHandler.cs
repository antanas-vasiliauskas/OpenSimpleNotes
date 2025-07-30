using MediatR;
using Microsoft.EntityFrameworkCore;
using OSN.Infrastructure;

namespace OSN.Application.Features.Notes.Get;

public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, Result<NoteResponse>>
{
    private readonly AppDbContext _db; // TODO: replace with repository
    private readonly ICurrentUserService _currentUser;

    public GetNoteByIdQueryHandler(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<NoteResponse>> Handle(GetNoteByIdQuery query, CancellationToken ct)
    {
        var note = await _db.Notes
            .Where(n => n.Id == query.Id && n.UserId == _currentUser.UserId && !n.IsDeleted)
            .Select(n => new NoteResponse(n.Id, n.Title, n.Content, n.IsPinned, n.CreatedAt, n.UpdatedAt))
            .FirstOrDefaultAsync(ct) ?? throw new NotFoundException("Note not found.");

        return Result<NoteResponse>.Success(note);
    }
}