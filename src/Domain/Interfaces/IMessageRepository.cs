using Domain.Entities;

namespace Domain.Interfaces;

public interface IMessageRepository : IBaseRepository<Message>, IDisposable
{
}
