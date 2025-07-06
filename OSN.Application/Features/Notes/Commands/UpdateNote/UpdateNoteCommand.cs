using MediatR;
using OSN.Application.Models.Requests.Notes;
using OSN.Application.Models.Responses.Notes;

namespace OSN.Application.Features.Notes.Commands.CreateNote;

public record UpdateNoteCommand(UpdateNoteRequest Request) : IRequest<NoteResponse>;