using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Entities.Common;
using Domain.Interfaces.Repositories;
using Infraestructure.Helpers;
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
    public override async Task<PaginatedList<WorkLog>> GetAsync(QueryFilterDto queryFilter)
    {
        List<string> searchParameters = new List<string>();

        var totalCount = await CountAsync(queryFilter, searchParameters);

        IQueryable<WorkLog> query = _dbSet.AsQueryable();

        query = new QueryFilterBuilder<WorkLog>(_dbSet)
            .ApplyQueryFilterAndActive(queryFilter, searchParameters)
            .Build()
            .Include(x => x.Technician);

        var items = await query.ToListAsync();

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

