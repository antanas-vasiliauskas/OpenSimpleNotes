namespace OSN.Application.Features.Notes.Update;

public record UpdateNoteRequest(
    string? Title,
    string? Content,
    bool? IsPinned
);