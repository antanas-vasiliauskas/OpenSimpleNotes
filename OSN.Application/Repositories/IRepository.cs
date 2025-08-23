namespace OSN.Application.Repositories;

public interface IRepository
{
    IUnitOfWork UnitOfWork { get; }
}