using MediatR;
using OSN.Application.Interfaces.ModelsResolvers;
using OSN.Application.Models.Responses.Notes;
using OSN.Domain.Interfaces;
using OSN.Infrastructure.Interfaces;

namespace OSN.Application.Features.Notes.Commands.CreateNote;

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, NoteResponse>
{
    private readonly INotesRepository _notesRepository;
    private readonly INoteModelsResolver _noteModelsResolver;
    private readonly IUserContext _userContext;

    public CreateNoteCommandHandler(INotesRepository notesRepository, INoteModelsResolver noteModelsResolver, IUserContext userContext)
    {
        _notesRepository = notesRepository ?? throw new ArgumentNullException(nameof(notesRepository));
        _noteModelsResolver = noteModelsResolver ?? throw new ArgumentNullException(nameof(noteModelsResolver));
        _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }

    public async Task<NoteResponse> Handle(CreateNoteCommand command, CancellationToken cancellationToken)
    {
        var noteRequest = command.Request;
        var userId = _userContext.UserId;

        var noteDao = _noteModelsResolver.GetDaoFromRequest(noteRequest, userId);

        noteDao = await _notesRepository.CreateAsync(noteDao);

        return _noteModelsResolver.GetResponseFromDao(noteDao);
    }
}