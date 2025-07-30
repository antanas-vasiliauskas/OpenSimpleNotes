using MediatR;

namespace OSN.Application.Features.Notes.Get;

public record GetAllNotesQuery : IRequest<Result<List<NoteResponse>>>;