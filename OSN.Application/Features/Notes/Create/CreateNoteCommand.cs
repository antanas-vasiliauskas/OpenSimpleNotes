using MediatR;

namespace OSN.Application.Features.Notes.Create;
public record CreateNoteCommand(CreateNoteRequest Request) : IRequest<Result<NoteResponse>>;