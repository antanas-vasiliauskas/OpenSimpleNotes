namespace OSN.Domain.Models;
public record User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "User", "Admin", "SuperAdmin"
    public bool IsDeleted { get; init; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Note> Notes { get; set; } = [];
}