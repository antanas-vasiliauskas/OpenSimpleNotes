using MediatR;

namespace OSN.Application.Features.Notes.Update;

[AuthorizeCommand(Policy = RoleHierarchy.GuestPolicy)]
public record UpdateNoteCommand(Guid Id, UpdateNoteRequest Request) : IRequest<Result<NoteResponse>>;