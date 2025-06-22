using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class MessageRepository(AppDbContext context) : GenericRepository<Message>(context), IMessageRepository
{
    private readonly DbSet<Message> _dbSet = context.Set<Message>();

    /// <inheritdoc/>
    public async Task<List<Message>> GetByIncidentIdAsync(long incidentId)
    {
        return await _dbSet
            .Where(x => x.IncidentId == incidentId)
            .Include(x => x.Sender)
            .ToListAsync();
    }
}