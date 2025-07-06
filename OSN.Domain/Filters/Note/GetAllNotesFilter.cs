namespace OSN.Domain.Filters.Note;
public record GetAllNotesFilter
{
    //public string? Status { get; init; }
    public Guid? UserId { get; init; }
}