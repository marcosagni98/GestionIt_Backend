using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Entities.Common;
using Domain.Interfaces;
using Infraestructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class IncidentHistoryRepository : IIncidentHistoryRepository
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<IncidentHistory> _dbSet;
    private readonly IMapper _mapper;

    public IncidentHistoryRepository(AppDbContext context, IMapper mapper) 
    {
        _dbContext = context;
        _mapper = mapper;
        _dbSet = _dbContext.Set<IncidentHistory>();
    }

    /// <inheritdoc/>
    public virtual async Task AddAsync(IncidentHistory entity)
    {
        await _dbSet.AddAsync(entity);
    }

    /// <inheritdoc/>
    public async Task<PaginatedList<IncidentHistory>> GetAsync(QueryFilterDto queryFilter)
    {
        List<string> searchParameters = new List<string>();

        var totalCount = await CountAsync(queryFilter, searchParameters);

        IQueryable<IncidentHistory> query = _dbSet.AsQueryable();

        query = new QueryFilterBuilder<IncidentHistory>(_dbSet)
            .ApplyQueryFilter(queryFilter, searchParameters)
            .Build();

        var items = await query.ToListAsync();

        return new PaginatedList<IncidentHistory>(items, totalCount);
    }

    /// <inheritdoc/>
    public async Task<IncidentHistory?> GetByIdAsync(long id)
    {
        return await _dbSet
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(IncidentHistory entity)
    {
        _dbSet.Update(entity);
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(long id)
    {
        return await _dbSet
            .Where(x => x.Id == id)
            .AnyAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<int> CountAsync(QueryFilterDto queryFilter, List<string>? searchParameters)
    {
        IQueryable<IncidentHistory> query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(queryFilter.Search))
        {
            query = new QueryFilterBuilder<IncidentHistory>(_dbSet)
            .Where(searchParameters, queryFilter.Search)
            .Build();
        }

        return await query.CountAsync();
    }
}
