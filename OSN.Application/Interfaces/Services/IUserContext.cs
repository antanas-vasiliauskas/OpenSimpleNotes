namespace OSN.Infrastructure.Interfaces;
public interface IUserContext
{
    Guid UserId { get; }
    string UserRole { get; }
}
