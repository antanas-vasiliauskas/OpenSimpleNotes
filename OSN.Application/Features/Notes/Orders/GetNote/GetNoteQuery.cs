using MediatR;
using OSN.Application.Models.Responses.Notes;

namespace OSN.Application.Features.Notes.Commands.CreateNote;

public record GetNoteQuery(Guid Id) : IRequest<NoteResponse>;