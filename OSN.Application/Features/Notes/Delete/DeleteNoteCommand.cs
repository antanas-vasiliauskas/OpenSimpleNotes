using MediatR;

namespace OSN.Application;

public record DeleteNoteCommand(Guid Id): IRequest<Result<Unit>>;
