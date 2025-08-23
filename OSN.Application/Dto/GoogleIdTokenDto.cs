namespace OSN.Application.Dto;

public record GoogleIdTokenDto(
    string GoogleId,
    string Email,
    string FirstName,
    string LastName,
    string ProfilePictureUrl
);