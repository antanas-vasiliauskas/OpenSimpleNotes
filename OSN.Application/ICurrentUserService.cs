namespace OSN.Application;

public interface ICurrentUserService
{
    Guid UserId { get; }
}