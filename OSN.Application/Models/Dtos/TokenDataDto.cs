using OSN.Domain.Enums;

namespace OSN.Application.Models.Dtos;

public record TokenDataDto(string UserId, UserRole UserRole)
{
    public string UserId { get; init; } = UserId;
    public UserRole UserRole { get; init; } = UserRole;
}