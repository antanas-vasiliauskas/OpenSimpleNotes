using MediatR;
using Microsoft.EntityFrameworkCore;
using OSN.Infrastructure;

namespace OSN.Application;

public class GetAllNotesQueryHandler: IRequestHandler<GetAllNotesQuery, Result<List<NoteResponse>>>
{
    private readonly AppDbContext _db; // TODO: replace with repository
    private readonly ICurrentUserService _currentUser;

    public GetAllNotesQueryHandler(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<List<NoteResponse>>> Handle(GetAllNotesQuery query, CancellationToken ct)
    {
        var notes = await _db.Notes
            .Where(n => n.UserId == _currentUser.UserId && !n.IsDeleted)
            .OrderByDescending(n => n.IsPinned)
            .ThenByDescending(n => n.UpdatedAt)
            .Select(n => new NoteResponse(n.Id, n.Title, n.Content, n.IsPinned, n.CreatedAt, n.UpdatedAt))
            .ToListAsync(ct);
        return Result<List<NoteResponse>>.Success(notes);
    }
}