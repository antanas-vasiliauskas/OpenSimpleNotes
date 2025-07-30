using MediatR;

namespace OSN.Application.Features.Notes.Get;
public record GetNoteByIdQuery(Guid Id) : IRequest<Result<NoteResponse>>;