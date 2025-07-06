using AutoMapper;
using OSN.Application.Interfaces.ModelsResolvers;
using OSN.Application.Models.Requests.Notes;
using OSN.Application.Models.Responses.Notes;
using OSN.Domain.Daos;

namespace OSN.Application.Mappings.ModelsResolvers;

public class NoteModelsResolver : INoteModelsResolver
{
    private readonly IMapper _mapper;

    public NoteModelsResolver(IMapper mapper)
    {
        _mapper = mapper;
    }
    public NoteDao GetDaoFromRequest(CreateNoteRequest createNoteRequest, Guid userId)
    {
        return _mapper.Map<NoteDao>(createNoteRequest) with { UserId = userId};
    }

    public NoteResponse GetResponseFromDao(NoteDao noteDao)
    {
        return _mapper.Map<NoteResponse>(noteDao) with
        {
            Id = noteDao.Id,
            CreatedAt = noteDao.CreatedAt,
            UpdatedAt = noteDao.UpdatedAt,
            Content = noteDao.Content,
            Title = noteDao.Title,
            IsPinned = noteDao.IsPinned,
        };
    }

    public IEnumerable<NoteResponse> GetResponseFromDao(IEnumerable<NoteDao> noteDaos)
    {
        return noteDaos.Select(noteDao => GetResponseFromDao(noteDao));
    }
}