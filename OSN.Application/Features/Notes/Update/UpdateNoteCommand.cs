using MediatR;

namespace OSN.Application;

public record UpdateNoteCommand(Guid Id, UpdateRequest Request): IRequest<Result<NoteResponse>>;