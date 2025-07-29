namespace OSN.Application;

public record UpdateRequest(
    string Title,
    string Content,
    bool IsPinned
);