using Domain.Entities;

namespace Domain.Interfaces;

public interface IWorkLogsRepository : IBaseRepository<WorkLog>, IDisposable
{
}