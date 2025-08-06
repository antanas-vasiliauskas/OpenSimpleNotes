namespace OSN.Domain.Models;

public record GoogleSignInFields
{
    public Guid Id { get; set; }
    public string GoogleId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public DateTime LinkedAt { get; set; }
    
    public Guid UserId { get; set; }
}