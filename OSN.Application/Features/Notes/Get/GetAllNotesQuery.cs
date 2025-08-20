using MediatR;

namespace OSN.Application.Features.Notes.Get;

[AuthorizeCommand(Policy = RoleHierarchy.GuestPolicy)]
public record GetAllNotesQuery : IRequest<Result<List<NoteResponse>>>;