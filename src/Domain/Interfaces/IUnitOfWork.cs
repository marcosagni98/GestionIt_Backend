namespace Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IIncidentHistoryRepository IncidentHistoryRepository { get; }
    IIncidentRepository IncidentRepository { get; }
    IMessageRepository MessageRepository { get; }
    IUserRepository UserRepository { get; }
    IUserFeedbackRepository UserFeedbackRepository { get; }
    IWorkLogRepository WorkLogRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
