namespace OSN.Application.Features.Notes;

public record NoteResponse(
    Guid Id,
    string Title,
    string Content,
    bool IsPinned,
    DateTime CreatedAt,
    DateTime UpdatedAt
);