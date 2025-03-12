using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class WorkLogsRepository : GenericRepository<WorkLog>, IWorkLogRepository
{
    private readonly AppDbContext _context;
    private DbSet<WorkLog> _dbSet;
    private readonly IMapper _mapper;

    public WorkLogsRepository(AppDbContext context, IMapper mapper) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<WorkLog>();
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public override async Task<PaginatedList<WorkLog>> GetAsync(QueryFilterDto queryFilter)
    {
        List<string> searchParameters = new List<string>();


        IQueryable<WorkLog> query = _dbSet.AsQueryable();

        var totalCount = await query
             .WhereActive()
             .CountAsync(queryFilter, searchParameters);

        var items = await query
            .ApplyQueryFilterAndActive(queryFilter, searchParameters)
            .ToListAsync();

        return new PaginatedList<WorkLog>(items, totalCount);
    }

    /// <inheritdoc/>
    public override async Task<WorkLog?> GetByIdAsync(long id)
    {
        return await _dbSet
            .Where(x => x.Active == true && x.Id == id)
            .Include(x => x.Technician)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public Task<List<WorkLog>> GetByIncidentIdAsync(long incidentId)
    {
        return _dbSet
            .Where(x => x.IncidentId == incidentId)
            .Include(x => x.Technician)
            .ToListAsync();
    }
}

