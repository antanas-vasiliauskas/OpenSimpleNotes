using MediatR;

namespace OSN.Application.Features.Notes.GetAll;
public record GetAllNotesQuery : IRequest<Result<List<NoteResponse>>>;