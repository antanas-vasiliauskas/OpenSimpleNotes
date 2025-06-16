using OSN.Domain.Enums;

namespace OSN.Application.Models.Responses.Auth;

public record LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public DateTime Expires { get; init; }
    public UserRole Role { get; init; }
    public Guid UserId { get; init; }
}