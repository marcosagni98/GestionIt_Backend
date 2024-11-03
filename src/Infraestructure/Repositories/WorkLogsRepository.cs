using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class WorkLogsRepository : GenericRepository<WorkLog>, IWorkLogRepository
{
    private readonly AppDbContext _context;
    private DbSet<WorkLog> _dbSet;
    private readonly IMapper _mapper;

    public WorkLogsRepository(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<WorkLog>();
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public Task<List<WorkLog>> GetByIncidentIdAsync(long incidentId)
    {
        return _dbSet.Where(x => x.IncidentId == incidentId).ToListAsync();
    }
}

