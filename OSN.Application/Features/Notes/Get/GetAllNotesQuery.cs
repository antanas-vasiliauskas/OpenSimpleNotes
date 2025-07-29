using MediatR;

namespace OSN.Application;

public record GetAllNotesQuery: IRequest<Result<List<NoteResponse>>>;