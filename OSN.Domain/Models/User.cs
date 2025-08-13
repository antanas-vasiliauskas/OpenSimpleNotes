using OSN.Domain.ValueObjects;

namespace OSN.Domain.Models;

public record User
{
    public Guid Id { get; set; }
    public EmailString? Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsDeleted { get; init; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Note> Notes { get; set; } = [];
    public virtual GoogleSignInFields? GoogleSignIn { get; set; }
}