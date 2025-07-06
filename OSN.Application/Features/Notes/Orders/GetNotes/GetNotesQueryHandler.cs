using MediatR;
using OSN.Application.Features.Notes.Commands.CreateNote;
using OSN.Application.Interfaces.ModelsResolvers;
using OSN.Application.Models.Responses.Notes;
using OSN.Domain.Filters.Note;
using OSN.Domain.Interfaces;
using OSN.Infrastructure.Interfaces;

namespace OSN.Application.Features.Notes.Orders.GetNotes;

public class GetNotesQueryHandler : IRequestHandler<GetNotesQuery, IEnumerable<NoteResponse>>
{
    private readonly INotesRepository _notesRepository;
    private readonly INoteModelsResolver _noteModelsResolver;
    private readonly IUserContext _userContext;

    public GetNotesQueryHandler(
        INotesRepository notesRepository,
        INoteModelsResolver noteModelsResolver,
        IUserContext userContext
    )
    {
        _notesRepository = notesRepository;
        _noteModelsResolver = noteModelsResolver;
        _userContext = userContext;
    }

    public async Task<IEnumerable<NoteResponse>> Handle(GetNotesQuery query, CancellationToken cancellationToken)
    {
        var filter = new GetAllNotesFilter()
        {
            UserId = query.UserId,
        };
        var noteDaos = await _notesRepository.GetAllAsync(filter);
        var response = _noteModelsResolver.GetResponseFromDao(noteDaos);
        return response;
    }
}