
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class IncidentRepository : GenericRepository<Incident>, IIncidentRepository
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<Incident> _dbSet;
    private readonly IMapper _mapper;

    public IncidentRepository(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
        _dbContext = context;
        _mapper = mapper;
        _dbSet = _dbContext.Set<Incident>();
    }

    /// <inheritdoc/>
    public async Task<List<Incident>?> GetByUserIdAsync(long userId)
    {
        return await _dbSet
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }
}

