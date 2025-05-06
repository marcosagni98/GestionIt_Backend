using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities.Common;
using Domain.Interfaces.Repositories;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : Entity
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
        IQueryable<TEntity> query = _dbSet.AsQueryable();

        return await query
            .ToPaginatedListAsync(queryFilter);
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity?> GetByIdAsync(long id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("The id must be higher than 0.", nameof(id));
        }

        return await _dbSet
            .WhereId(id)
            .WhereActive()
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
        var entity = await _dbSet.FindAsync(id) ?? throw new KeyNotFoundException("Entity not found");
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
}
