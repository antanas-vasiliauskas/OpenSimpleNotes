namespace OSN.Application.Models.Requests.Notes;

public record UpdateNoteRequest
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    public bool IsPinned { get; init; } = false;
}