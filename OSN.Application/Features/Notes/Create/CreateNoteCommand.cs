using MediatR;

namespace OSN.Application;
public record CreateNoteCommand(CreateNoteRequest Request): IRequest<Result<NoteResponse>>;