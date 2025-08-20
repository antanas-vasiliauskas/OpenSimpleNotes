using MediatR;

namespace OSN.Application.Features.Notes.Create;

[AuthorizeCommand(Policy = RoleHierarchy.GuestPolicy)]
public record CreateNoteCommand(CreateNoteRequest Request) : IRequest<Result<NoteResponse>>;