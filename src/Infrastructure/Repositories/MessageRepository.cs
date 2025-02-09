using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{

    private readonly AppDbContext _dbContext;
    private readonly DbSet<Message> _dbSet;

    public MessageRepository(AppDbContext context) : base(context)
    {
        _dbContext = context;
        _dbSet = _dbContext.Set<Message>();
    }

    /// <inheritdoc/>
    public async Task<List<Message>> GetByIncidentIdAsync(long incidentId)
    {
        return await _dbSet
            .Where(x => x.IncidentId == incidentId)
            .Include(x => x.Sender)
            .ToListAsync();
    }
}