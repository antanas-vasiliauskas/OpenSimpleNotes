namespace OSN.Domain.Models;

public record PendingVerification
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string VerificationCode { get; set; } = string.Empty;
}