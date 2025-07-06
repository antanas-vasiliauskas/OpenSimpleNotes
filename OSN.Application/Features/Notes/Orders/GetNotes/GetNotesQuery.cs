using MediatR;
using OSN.Application.Models.Responses.Notes;

namespace OSN.Application.Features.Notes.Commands.CreateNote;

public record GetNotesQuery(Guid UserId) : IRequest<IEnumerable<NoteResponse>>;