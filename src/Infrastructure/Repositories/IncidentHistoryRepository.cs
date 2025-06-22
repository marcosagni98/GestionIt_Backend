using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class IncidentHistoryRepository(AppDbContext context) : IIncidentHistoryRepository
{
    private readonly DbSet<IncidentHistory> _dbSet = context.Set<IncidentHistory>();

    /// <inheritdoc/>
    public async Task AddAsync(IncidentHistory entity)
    {
        await _dbSet.AddAsync(entity);
    }

    /// <inheritdoc/>
    public async Task<PaginatedList<IncidentHistory>> GetAsync(QueryFilterDto queryFilter)
    {
        List<string> searchParameters = ["ChangedByUser.Name", "ResolutionDetails"];

        var query = _dbSet.AsQueryable();

        return await query
            .ToPaginatedListNotActiveAsync(queryFilter, searchParameters);
    }

    /// <inheritdoc/>
    public async Task<IncidentHistory?> GetByIdAsync(long id)
    {
        return await _dbSet
            .WhereId(id)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public Task<List<IncidentHistory>> GetByIncidentIdAsync(long incidentId)
    {
        return _dbSet.Where(x => x.IncidentId == incidentId)
            .Include(x => x.ChangedByUser)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public void Update(IncidentHistory entity)
    {
        _dbSet.Update(entity);
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(long id)
    {
        return await _dbSet
            .WhereId(id)
            .AnyAsync();
    }

    /// <inheritdoc/>
    public async Task<int> CountAsync(QueryFilterDto queryFilter, List<string>? searchParameters)
    {
        var query = _dbSet.AsQueryable();

        return await query
            .WhereFilter(searchParameters, queryFilter.Search)
            .CountAsync();
    }
}
