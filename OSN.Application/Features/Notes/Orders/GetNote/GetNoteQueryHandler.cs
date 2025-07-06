using MediatR;
using OSN.Application.Features.Notes.Commands.CreateNote;
using OSN.Application.Interfaces.ModelsResolvers;
using OSN.Application.Models.Responses.Notes;
using OSN.Domain.Interfaces;
using OSN.Infrastructure.Interfaces;

namespace OSN.Application.Features.Notes.Orders.GetNote;

public class GetNoteQueryHandler : IRequestHandler<GetNoteQuery, NoteResponse>
{
    private readonly INotesRepository _notesRepository;
    private readonly INoteModelsResolver _noteModelsResolver;
    private readonly IUserContext _userContext;

    public GetNoteQueryHandler(
        INotesRepository notesRepository,
        INoteModelsResolver noteModelsResolver,
        IUserContext userContext
    )
    {
        _notesRepository = notesRepository;
        _noteModelsResolver = noteModelsResolver;
        _userContext = userContext;
    }

    public async Task<NoteResponse> Handle(GetNoteQuery query, CancellationToken cancellationToken)
    {
        var originalDao = await _notesRepository.GetAsync(query.Id);
        if (originalDao == null)
        {
            throw new KeyNotFoundException($"Note with ID {query.Id} not found.");
        }
        var response = _noteModelsResolver.GetResponseFromDao(originalDao);
        return response;
    }
}