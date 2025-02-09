namespace Domain.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task SaveAsync();
}