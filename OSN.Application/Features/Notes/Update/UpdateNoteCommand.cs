using MediatR;

namespace OSN.Application.Features.Notes.Update;

public record UpdateNoteCommand(Guid Id, UpdateNoteRequest Request) : IRequest<Result<NoteResponse>>;