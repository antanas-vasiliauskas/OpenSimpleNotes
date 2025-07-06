using MediatR;
using OSN.Application.Features.Notes.Commands.CreateNote;
using OSN.Application.Interfaces.ModelsResolvers;
using OSN.Application.Models.Responses.Notes;
using OSN.Domain.Interfaces;
using OSN.Infrastructure.Interfaces;

namespace OSN.Application.Features.Notes.Commands.DeleteNote;

public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand, NoteResponse>
{
    private readonly INotesRepository _notesRepository;
    private readonly INoteModelsResolver _noteModelsResolver;
    private readonly IUserContext _userContext;

    public DeleteNoteCommandHandler(
        INotesRepository notesRepository,
        INoteModelsResolver noteModelsResolver,
        IUserContext userContext
    )
    {
        _notesRepository = notesRepository;
        _noteModelsResolver = noteModelsResolver;
        _userContext = userContext;
    }

    public async Task<NoteResponse> Handle(DeleteNoteCommand command, CancellationToken cancellationToken)
    {
        var originalDao = await _notesRepository.GetAsync(command.Id);
        if (originalDao == null)
        {
            throw new KeyNotFoundException($"Note with ID {command.Id} not found.");
        }

        var deletedDao = originalDao with
        {
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = true
        };

        await _notesRepository.UpdateAsync(deletedDao);

        var response = _noteModelsResolver.GetResponseFromDao(deletedDao);
        return response;
    }
}