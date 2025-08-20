using MediatR;

namespace OSN.Application.Features.Notes.Delete;

[AuthorizeCommand(Policy = RoleHierarchy.GuestPolicy)]
public record DeleteNoteCommand(Guid Id) : IRequest<Result<Unit>>;
