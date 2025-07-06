using OSN.Application.Models.Requests.Notes;
using OSN.Application.Models.Responses.Notes;
using OSN.Domain.Daos;

namespace OSN.Application.Interfaces.ModelsResolvers;

public interface INoteModelsResolver
{
    NoteDao GetDaoFromRequest(CreateNoteRequest createNoteRequest, Guid userId);
    NoteResponse GetResponseFromDao(NoteDao noteDao);
    IEnumerable<NoteResponse> GetResponseFromDao(IEnumerable<NoteDao> noteDaos);
}