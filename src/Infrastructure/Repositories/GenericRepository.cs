using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities.Common;
using Domain.Interfaces.Repositories;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
    }

    /// <inheritdoc/>
    public virtual async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    /// <inheritdoc/>
    public virtual async Task<PaginatedList<TEntity>> GetAsync(QueryFilterDto queryFilter)
    {
        List<string> searchParameters = new List<string>();

        var totalCount = await CountAsync(queryFilter, searchParameters);

        IQueryable<TEntity> query = _dbSet.AsQueryable();

        query = new QueryFilterBuilder<TEntity>(_dbSet)
            .ApplyQueryFilterAndActive(queryFilter, searchParameters)
            .Build();

        var items = await query.ToListAsync();

        return new PaginatedList<TEntity>(items, totalCount);
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity?> GetByIdAsync(long id)
    {
        return await _dbSet
            .Where(x => (x as Entity)!.Active == true && (x as Entity)!.Id == id)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    /// <inheritdoc/>
    public virtual async Task DeleteAsync(long id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) throw new KeyNotFoundException("Entity not found");

        if (entity is Entity entityToDeactivate)
        {
            entityToDeactivate.Deactivate();
        }
    }

    /// <inheritdoc/>
    public virtual async Task<bool> ExistsAsync(long id)
    {
        return await _dbSet
            .Where(x => (x as Entity)!.Active == true && (x as Entity)!.Id == id)
            .AnyAsync();
    }

    /// <inheritdoc/>
    public async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    /// <inheritdoc/>
    public async Task<int> CountAsync(IQueryable<TEntity>? query, QueryFilterDto? queryFilter, List<string>? searchParameters)
    {
        if (query == null)
        {
            query = _dbSet.AsQueryable();
        }
        if (queryFilter != null && !string.IsNullOrEmpty(queryFilter.Search))
        {
            query = new QueryFilterBuilder<TEntity>(query)
                .Where(searchParameters, queryFilter.Search)
                .Build();
        }

        query = new QueryFilterBuilder<TEntity>(query)
            .WhereActive()
            .Build();

        return await query.CountAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<int> CountAsync(QueryFilterDto? queryFilter, List<string>? searchParameters)
    {
        IQueryable<TEntity> query = _dbSet.AsQueryable();

        if (queryFilter != null && !string.IsNullOrEmpty(queryFilter.Search))
        {
            query = new QueryFilterBuilder<TEntity>(_dbSet)
            .Where(searchParameters, queryFilter.Search)
            .Build();
        }

        query = new QueryFilterBuilder<TEntity>(_dbSet)
        .WhereActive()
        .Build();

        return await query.CountAsync();
    }
}
