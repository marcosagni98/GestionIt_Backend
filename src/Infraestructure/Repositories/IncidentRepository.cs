
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using FluentResults;
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
    public Task<int> CountByPriorityAsync(Priority priority)
    {
        return _dbSet
            .CountAsync(x => x.Priority == priority && x.Active == true && (x.Status != Status.Completed || x.Status != Status.Closed));
    }

    /// <inheritdoc/>
    public Task<int> CountByStatusAsync(Status status)
    {
        return _dbSet
            .CountAsync(x => x.Status == status && x.Active == true && (x.Status != Status.Completed || x.Status != Status.Closed));
    }

    /// <inheritdoc/>
    public async Task<int> CountAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .CountAsync(f => f.CreatedAt >= startDate && f.CreatedAt <= endDate && f.Active == true);
    }

    /// <inheritdoc/>
    public async Task<List<Incident>?> GetByUserIdAsync(long userId)
    {
        return await _dbSet
            .Where(x => x.UserId == userId && x.Active == true)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<double> GetAverageResolutionTimeAsync(DateTime startDate, DateTime endDate)
    {
        var incidents = await _dbContext.Incidents
            .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate)
            .Include(i => i.IncidentHistories)
            .ToListAsync();


        var completedIncidents = incidents
            .Where(i => i.IncidentHistories.Any(ih => ih.Status == Status.Completed))
            .Select(i => new
            {
                CreatedAt = i.CreatedAt,
                CompletedAt = i.IncidentHistories
                                .Where(ih => ih.Status == Status.Completed)
                                .OrderBy(ih => ih.ChangedAt)
                                .FirstOrDefault()
            })
            .Where(i => i.CompletedAt != null) 
            .ToList();

        // Calcular el tiempo de resolución
        var resolutionTimes = completedIncidents
            .Select(i => (i.CompletedAt.ChangedAt - i.CreatedAt).TotalHours)
            .ToList();

        double averageResolutionTime = resolutionTimes.Count > 0
            ? resolutionTimes.Average()
            : 0;

        return averageResolutionTime;
    }

    /// <inheritdoc/>
    public async Task UpdateIncidentStatusAsync(long id, Status newStatus)
    {
        var incident = await _dbSet.FindAsync(id) ?? throw new KeyNotFoundException($"Incident with ID {id} not found.");
        incident.Status = newStatus;
    }
}

