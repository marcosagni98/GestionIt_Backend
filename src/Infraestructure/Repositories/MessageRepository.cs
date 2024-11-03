using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{

    private readonly AppDbContext _dbContext;
    private readonly DbSet<Message> _dbSet;
    private readonly IMapper _mapper;

    public MessageRepository(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
        _dbContext = context;
        _mapper = mapper;
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