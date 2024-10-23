using AutoMapper;
using Domain.Interfaces;
using Infraestructure.Repositories;


namespace Infraestructure;

public class UnitOfWork(AppDbContext dbContext, IMapper mapper) : IUnitOfWork
{
    private readonly AppDbContext _dbContext = dbContext;
    private IMapper _mapper = mapper;

    private IIncidentHistoryRepository _incidentHistoryRepository;
    private IIncidentRepository _incidentRepository;
    private IMessageRepository _messageRepository;
    private IUserRepository _userRepository;
    private IUserFeedbackRepository _userFeedbackRepository;
    private IWorkLogRepository _workLogsRepository;

    public IIncidentHistoryRepository IncidentHistoryRepository
    {
        get
        {
            _incidentHistoryRepository ??= new IncidentHistoryRepository(_dbContext, _mapper);
            return _incidentHistoryRepository;
        }
    }
    public IIncidentRepository IncidentRepository
    {
        get
        {
            _incidentRepository ??= new IncidentRepository(_dbContext, _mapper);
            return _incidentRepository;
        }
    }
    public IMessageRepository MessageRepository
    {
        get
        {
            _messageRepository ??= new MessageRepository(_dbContext, _mapper);
            return _messageRepository;
        }
    }
    public IUserRepository UserRepository
    {
        get
        {
            _userRepository ??= new UserRepository(_dbContext, _mapper);
            return _userRepository;
        }
    }
    public IUserFeedbackRepository UserFeedbackRepository
    {
        get
        {
            _userFeedbackRepository ??= new UserFeedbackRepository(_dbContext, _mapper);
            return _userFeedbackRepository;
        }
    }
    public IWorkLogRepository WorkLogRepository 
    { 
        get
        {
            _workLogsRepository ??= new WorkLogsRepository(_dbContext, _mapper);
            return _workLogsRepository;
        }
    }

    #region Dispose
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
