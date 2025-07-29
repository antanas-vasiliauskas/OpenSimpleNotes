namespace OSN.Application.Features.Notes.Create;

public record CreateNoteRequest(
    string Title,
    string Content
);