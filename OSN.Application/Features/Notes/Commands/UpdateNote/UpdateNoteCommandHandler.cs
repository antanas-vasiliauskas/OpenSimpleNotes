using MediatR;
using OSN.Application.Features.Notes.Commands.CreateNote;
using OSN.Application.Interfaces.ModelsResolvers;
using OSN.Application.Mappings.ModelsResolvers;
using OSN.Application.Models.Responses.Notes;
using OSN.Domain.Interfaces;
using OSN.Infrastructure.Interfaces;

namespace OSN.Application.Features.Notes.Commands.UpdateNote;

public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, NoteResponse>
{
    private readonly INotesRepository _notesRepository;
    private readonly INoteModelsResolver _noteModelsResolver;
    private readonly IUserContext _userContext;

    public UpdateNoteCommandHandler(
        INotesRepository notesRepository,
        INoteModelsResolver noteModelsResolver,
        IUserContext userContext
    )
    {
        _notesRepository = notesRepository;
        _noteModelsResolver = noteModelsResolver;
        _userContext = userContext;
    }

    public async Task<NoteResponse> Handle(UpdateNoteCommand command, CancellationToken cancellationToken)
    {
        var updateNoteRequest = command.Request;
        var originalDao = await _notesRepository.GetAsync(updateNoteRequest.Id);
        if (originalDao == null)
        {
            throw new KeyNotFoundException($"Note with ID {updateNoteRequest.Id} not found.");
        }

        var updatedDao = originalDao with
        {
            UpdatedAt = DateTime.UtcNow,
            Title = updateNoteRequest.Title,
            Content = updateNoteRequest.Content,
            IsPinned = updateNoteRequest.IsPinned
        };

        await _notesRepository.UpdateAsync(updatedDao);

        var response = _noteModelsResolver.GetResponseFromDao(updatedDao);
        return response;
    }
}