using MediatR;

namespace OSN.Application.Features.Notes.Update;
public record UpdateNoteCommand(Guid Id, string? Title, string? Content, bool? IsPinned) 
    : IRequest<Result<NoteResponse>>;