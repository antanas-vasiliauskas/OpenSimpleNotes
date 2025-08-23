using MediatR;

namespace OSN.Application.Features.Notes.Create;
public record CreateNoteCommand(string Title, string Content) : IRequest<Result<NoteResponse>>;