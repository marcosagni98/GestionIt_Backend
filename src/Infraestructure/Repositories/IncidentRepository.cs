
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
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
    public Task<int> CountByPriority(Priority priority)
    {
        return _dbSet
            .CountAsync(x => x.Priority == priority && x.Active == true && (x.Status != Status.Completed || x.Status != Status.Closed));
    }

    /// <inheritdoc/>
    public Task<int> CountByStatus(Status status)
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
        // Obtener todas las incidencias creadas entre las fechas especificadas
        var incidents = await _dbContext.Incidents
            .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate)
            .Include(i => i.IncidentHistories) // Asegúrate de incluir las historias de la incidencia
            .ToListAsync();

        // Filtrar las incidencias que tienen un historial de estado 'Completed'
        var completedIncidents = incidents
            .Where(i => i.IncidentHistories.Any(ih => ih.Status == Status.Completed))
            .Select(i => new
            {
                CreatedAt = i.CreatedAt,
                CompletedAt = i.IncidentHistories
                                .Where(ih => ih.Status == Status.Completed)
                                .OrderBy(ih => ih.ChangedAt)
                                .FirstOrDefault() // Obtiene la primera fecha en que se completó
            })
            .Where(i => i.CompletedAt != null) // Filtrar para asegurar que tenemos un 'CompletedAt'
            .ToList();

        // Calcular el tiempo de resolución
        var resolutionTimes = completedIncidents
            .Select(i => (i.CompletedAt.ChangedAt - i.CreatedAt).TotalHours) // o TotalMinutes según lo que necesites
            .ToList();

        // Calcular el promedio
        double averageResolutionTime = resolutionTimes.Count > 0
            ? resolutionTimes.Average()
            : 0;

        return averageResolutionTime; // Devuelve el promedio en horas (o minutos)
    }
}

