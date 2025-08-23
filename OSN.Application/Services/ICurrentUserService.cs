namespace OSN.Application.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }
}