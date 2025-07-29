using MediatR;

namespace OSN.Application;
public record GetNoteByIdQuery(Guid Id): IRequest<Result<NoteResponse>>;