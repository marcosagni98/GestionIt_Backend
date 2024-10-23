﻿using AutoMapper;
using Domain.Entities.Common;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Infraestructure.Helpers;
using Domain.Dtos.CommonDtos.Request;
using Domain;
using Domain.Dtos.CommonDtos.Response;

namespace Infraestructure.Repositories;

public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;
    private readonly IMapper _mapper;

    public GenericRepository(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public virtual async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
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
    public virtual async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Where(x => (x as Entity).Active == true && (x as Entity).Id == id)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public virtual async Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
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

        await _dbContext.SaveChangesAsync();
    }
     
    /// <inheritdoc/>
    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await _dbSet
            .Where(x => (x as Entity).Active == true && (x as Entity).Id == id)
            .AnyAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<int> CountAsync(QueryFilterDto queryFilter, List<string>? searchParameters)
    {
        IQueryable<TEntity> query = _dbSet.AsQueryable();

        if(!string.IsNullOrEmpty(queryFilter.Search))
        {
            query = new QueryFilterBuilder<TEntity>(_dbSet)
            .Where(searchParameters, queryFilter.Search)
            .Build();
        }
        
        return await query.CountAsync();
    }
}