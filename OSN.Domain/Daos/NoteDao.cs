namespace OSN.Domain.Daos;

public record NoteDao
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public bool IsPinned { get; init; }
    public bool IsDeleted { get; init; }


    public virtual UserDao User { get; init; } = null!;
}