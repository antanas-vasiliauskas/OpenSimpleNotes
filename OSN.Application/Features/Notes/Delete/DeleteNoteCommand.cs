using MediatR;

namespace OSN.Application.Features.Notes.Delete;
public record DeleteNoteCommand(Guid Id) : IRequest<Result<Unit>>;
