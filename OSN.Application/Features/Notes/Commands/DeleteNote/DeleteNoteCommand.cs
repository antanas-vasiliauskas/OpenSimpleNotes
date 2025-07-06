using MediatR;
using OSN.Application.Models.Responses.Notes;

namespace OSN.Application.Features.Notes.Commands.CreateNote;

public record DeleteNoteCommand(Guid Id) : IRequest<NoteResponse>;