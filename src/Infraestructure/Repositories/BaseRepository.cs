using AutoMapper;
using Domain.Entities.Common;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Infraestructure.Helpers;
using Domain.Dtos.CommonDtos.Response;
using Domain.Dtos.CommonDtos.Request;
using Domain;
namespace Infraestructure.Repositories;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;
    private readonly IMapper _mapper;

    public BaseRepository(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public virtual async Task<Result<Unit>> AddAsync(TEntity entity)
    {
        try
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return Result<Unit>.SuccessUnit();
        }
        catch (Exception ex)
        {
            return Result<Unit>.Failure($"Error adding the entity: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public virtual async Task<Result<PaginatedList<TEntity>>> GetAsync(QueryFilterDto queryFilter)
    {
        try
        {
            List<string> searchParameters = new List<string>();

            var countResult = await CountAsync(queryFilter, searchParameters);

            if (!countResult.IsSuccess)
            {
                return Result<PaginatedList<TEntity>>.Failure(countResult.Error!);
            }

            var totalCount = countResult.Value;

            IQueryable<TEntity> query = _dbSet.AsQueryable();

            query = new QueryFilterBuilder<TEntity>(_dbSet)
                .ApplyQueryFilterAndActive(queryFilter, searchParameters)
                .Build();

            var items = await query.ToListAsync();

            return Result<PaginatedList<TEntity>>.Success(new PaginatedList<TEntity>(items, totalCount));
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<TEntity>>.Failure($"Error retrieving entities: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public virtual async Task<Result<TEntity>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _dbSet
                .Where(x => (x as Entity).Active == true && (x as Entity).Id == id)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return Result<TEntity>.Failure("Entity not found");
            }

            return Result<TEntity>.Success(entity);
        }
        catch (Exception ex)
        {
            return Result<TEntity>.Failure($"Error retrieving entity: {ex.Message}");
        }
    }


    /// <inheritdoc/>
    public virtual async Task<Result<Unit>> UpdateAsync(TEntity entity)
    {
        try
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();

            return Result<Unit>.SuccessUnit();
        }
        catch (Exception ex)
        {
            // En caso de error, retornamos un resultado de fallo con el mensaje de error
            return Result<Unit>.Failure($"Error updating entity: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public virtual async Task<Result<Unit>> DeleteAsync(long id)
    {
        try
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return Result<Unit>.Failure("Entity not found");
            }

            if (entity is Entity entityToDeactivate)
            {
                entityToDeactivate.Deactivate();
            }

            await _dbContext.SaveChangesAsync();
            return Result<Unit>.SuccessUnit();
        }
        catch (Exception ex)
        {
            return Result<Unit>.Failure($"Error deleting the entity: {ex.Message}");
        }
    }


    /// <inheritdoc/>
    public virtual async Task<Result<bool>> ExistsAsync(int id)
    {
        try
        {
            var exists = await _dbSet
                .Where(x => (x as Entity).Active == true && (x as Entity).Id == id)
                .AnyAsync();

            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error checking if entity exists: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public virtual async Task<Result<int>> CountAsync(QueryFilterDto queryFilter, List<string>? searchParameters)
    {
        try
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();

            query = new QueryFilterBuilder<TEntity>(_dbSet)
                .Where(searchParameters, queryFilter.Search)
                .Build();

            var count = await query.CountAsync();
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Error counting entities: {ex.Message}");
        }
    }
}
