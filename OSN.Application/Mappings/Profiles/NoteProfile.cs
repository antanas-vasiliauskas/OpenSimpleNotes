using AutoMapper;
using OSN.Application.Features.Notes.Commands.CreateNote;
using OSN.Application.Models.Requests.Notes;
using OSN.Application.Models.Responses.Notes;
using OSN.Domain.Daos;
using OSN.Domain.Filters.Note;

namespace OSN.Application.Mappings.Profiles;

public class NoteProfile: Profile
{
    public NoteProfile()
    {
        CreateMap<GetNotesQuery, GetAllNotesFilter>();
        CreateMap<NoteDao, NoteResponse>(MemberList.Source);
        CreateMap<CreateNoteRequest, NoteDao>(MemberList.Source);
        CreateMap<UpdateNoteRequest, NoteDao>(MemberList.Source)
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember is not null));
    }
}