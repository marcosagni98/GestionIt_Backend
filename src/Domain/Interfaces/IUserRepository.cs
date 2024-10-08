using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository : IDisposable, IBaseRepository<User>
{
}