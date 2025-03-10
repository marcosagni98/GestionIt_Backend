using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class IncidentRepository : GenericRepository<Incident>, IIncidentRepository
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<Incident> _dbSet;
    private readonly IMapper _mapper;

    public IncidentRepository(AppDbContext context, IMapper mapper) : base(context)
    {
        _dbContext = context;
        _mapper = mapper;
        _dbSet = _dbContext.Set<Incident>();
    }

    /// <inheritdoc/>
    public async Task<List<long>?> GetIdsAsync()
    {
        return await _dbSet.Where(x => x.Active == true).Select(x => x.Id).ToListAsync();
    }

    /// <inheritdoc/>
    public Task<int> CountByPriorityAsync(Priority priority)
    {
        return _dbSet
            .CountAsync(x => x.Priority == priority && x.Active == true && (x.Status != Status.Completed && x.Status != Status.Closed));
    }

    /// <inheritdoc/>
    public Task<int> CountByPriorityAsync(Priority priority, long technicianId)
    {
        return _dbSet
            .CountAsync(x => x.Priority == priority && x.Active == true && (x.Status != Status.Completed && x.Status != Status.Closed) && x.Technician.Id == technicianId);
    }

    /// <inheritdoc/>
    public Task<int> CountByStatusAsync(Status status)
    {
        return _dbSet
            .CountAsync(x => x.Status == status && x.Active == true);
    }

    /// <inheritdoc/>
    public async Task<int> CountAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .CountAsync(f => f.CreatedAt >= startDate && f.CreatedAt <= endDate && f.Active == true);
    }

    /// <inheritdoc/>
    public async Task<int> CountAsync(DateTime startDate, DateTime endDate, long id)
    {
        return await _dbSet
            .CountAsync(f => f.CreatedAt >= startDate && f.CreatedAt <= endDate && f.Active == true && f.Technician.Id == id);
    }

    /// <inheritdoc/>
    public override async Task<PaginatedList<Incident>> GetAsync(QueryFilterDto queryFilter)
    {
        List<string> searchParameters = [
            "Title",
            "Description",
            "User.Name",
            "Technician.Name"
        ];

        GetCorrectQueryFilterOrderBy(queryFilter);

        IQueryable<Incident> query = _dbSet.Where(x => (x.Status != Status.Completed && x.Status != Status.Closed)).Include(i => i.User)
            .Include(i => i.Technician);

        var totalCount = await CountAsync(query, queryFilter, searchParameters);


        query = new QueryFilterBuilder<Incident>(query)
            .ApplyQueryFilterAndActive(queryFilter, searchParameters)
            .Build();

        var items = await query.ToListAsync();

        return new PaginatedList<Incident>(items, totalCount);
    }



    #region GetAsync private functions
    /// <summary>
    /// Transforms sorting property names from Data Transfer Object (DTO) conventions to corresponding entity navigation property names.
    /// This method maps client-side sorting properties to the correct database entity property paths for efficient querying.
    /// </summary>
    /// <param name="queryFilter">The query filter containing sorting parameters to be transformed.</param>
    /// /// <remarks>
    /// Supported mappings include:
    /// - "UserName" -> "User.Name"
    /// - "UserId" -> "User.Id"
    /// - "TechnicianName" -> "Technician.Name"
    /// - "TechnicianId" -> "Technician.Id"
    /// 
    /// If no matching mapping is found, the original OrderBy value is preserved.
    /// </remarks>
    private static void GetCorrectQueryFilterOrderBy(QueryFilterDto queryFilter)
    {
        if (!string.IsNullOrWhiteSpace(queryFilter?.OrderBy))
        {
            queryFilter.OrderBy = queryFilter.OrderBy switch
            {
                "userName" => "User.Name",
                "userId" => "User.Id",
                "technicianName" => "Technician.Name",
                "technicianId" => "Technician.Id",
                _ => queryFilter.OrderBy
            };
        }
    }

    #endregion

    /// <inheritdoc/>
    public override async Task<Incident?> GetByIdAsync(long id)
    {
        return await _dbSet
            .Where(x => x.Active == true && x.Id == id)
            .Include(i => i.User)
            .Include(i => i.Technician)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<PaginatedList<Incident>> GetHistoricAsync(QueryFilterDto queryFilter)
    {
        List<string> searchParameters = [
             "Title",
            "Description",
            "User.Name",
            "Technician.Name"
         ];

        GetCorrectQueryFilterOrderBy(queryFilter);

        var baseQuery = _dbSet.Where(x => (x.Status == Status.Completed || x.Status == Status.Closed) && x.Active == true).Include(i => i.User)
            .Include(i => i.Technician);

        var totalCount = await CountAsync(baseQuery, queryFilter, searchParameters);

        var query = new QueryFilterBuilder<Incident>(baseQuery)
            .ApplyQueryFilterAndActive(queryFilter, searchParameters)
            .Build()
            .Include(i => i.User)
            .Include(i => i.Technician);

        var items = await query.ToListAsync();

        return new PaginatedList<Incident>(items, totalCount);
    }

    /// <inheritdoc/>
    public async Task<List<long>?> GetIdsByUserIdAsync(long userId)
    {
        return await _dbSet
            .Where(x => x.UserId == userId || x.TechnicianId == userId && x.Active == true)
            .Include(i => i.User)
            .Include(i => i.Technician)
            .Select(x => x.Id)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<double> GetAverageResolutionTimeAsync(DateTime startDate, DateTime endDate)
    {
        var incidents = await _dbContext.Incidents
            .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate && i.Active == true)
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

        var resolutionTimes = completedIncidents
            .Select(i => (i.CompletedAt.ChangedAt - i.CreatedAt).TotalMinutes)
            .ToList();

        double averageResolutionTime = resolutionTimes.Count > 0
            ? resolutionTimes.Average()
            : 0;

        return averageResolutionTime;
    }

    /// <inheritdoc/>
    public async Task<double> GetAverageResolutionTimeAsync(DateTime startDate, DateTime endDate, long id)
    {
        var incidents = await _dbContext.Incidents
            .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate && i.Technician.Id == id && i.Active == true)
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

        var resolutionTimes = completedIncidents
            .Select(i => (i.CompletedAt.ChangedAt - i.CreatedAt).TotalMinutes)
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

    /// <inheritdoc/>
    public async Task UpdateIncidentPriorityAsync(long id, Priority newPriority)
    {
        var incident = await _dbSet.FindAsync(id) ?? throw new KeyNotFoundException($"Incident with ID {id} not found.");
        incident.Priority = newPriority;
    }

    /// <inheritdoc/>
    public async Task<PaginatedList<Incident>> GetIncidentsOfUserAsync(QueryFilterDto queryFilter, long userId)
    {
        List<string> searchParameters = [
            "Title",
            "Description",
            "User.Name",
            "Technician.Name"
        ];

        GetCorrectQueryFilterOrderBy(queryFilter);

        var baseQuery = _dbSet
            .Where(x => (x.UserId == userId || x.TechnicianId == userId) && x.Active == true && (x.Status != Status.Completed && x.Status != Status.Closed))
            .Include(i => i.User)
            .Include(i => i.Technician);

        var totalCount = await CountAsync(baseQuery, queryFilter, searchParameters);

        var query = new QueryFilterBuilder<Incident>(baseQuery)
            .ApplyQueryFilterAndActive(queryFilter, searchParameters)
            .Build()
            .Include(i => i.User)
            .Include(i => i.Technician);

        var items = await query.ToListAsync();

        return new PaginatedList<Incident>(items, totalCount);
    }

    /// <inheritdoc/>
    public async Task<PaginatedList<Incident>> GetByPriorityAsync(QueryFilterDto queryFilter, Priority priority)
    {
        List<string> searchParameters = [
            "Title",
            "Description",
            "User.Name",
            "Technician.Name"
        ];

        GetCorrectQueryFilterOrderBy(queryFilter);

        var baseQuery = _dbSet.Where(x => (x.Priority == priority && (x.Status != Status.Completed && x.Status != Status.Closed)))
            .Include(i => i.User)
            .Include(i => i.Technician);

        var totalCount = await CountAsync(baseQuery, queryFilter, searchParameters);

        var query = new QueryFilterBuilder<Incident>(baseQuery)
            .ApplyQueryFilterAndActive(queryFilter, searchParameters)
            .Build();

        var items = await query.ToListAsync();

        return new PaginatedList<Incident>(items, totalCount);
    }

    /// <inheritdoc/>
    public async Task<PaginatedList<Incident>> GetIncidentsByPriorityUserAsync(QueryFilterDto queryFilter, Priority priority, long userId)
    {
        List<string> searchParameters = [
            "Title",
            "Description",
            "User.Name",
            "Technician.Name"
        ];

        GetCorrectQueryFilterOrderBy(queryFilter);

        var baseQuery = _dbSet.Where(x => (x.Priority == priority && (x.Status != Status.Completed && x.Status != Status.Closed)))
            .Include(i => i.User)
            .Include(i => i.Technician); ;

        var totalCount = await CountAsync(baseQuery, queryFilter, searchParameters);

        var query = new QueryFilterBuilder<Incident>(baseQuery)
            .ApplyQueryFilterAndActive(queryFilter, searchParameters)
            .Build()
            .Include(i => i.User)
            .Include(i => i.Technician);

        var items = await query.ToListAsync();

        return new PaginatedList<Incident>(items, totalCount);
    }

    /// <inheritdoc/>
    public async Task<List<(string Date, int Count)>> GetIncidentCountByDayAsync(int year)
    {
        var dailyIncidences = await _dbSet
        .Where(i => i.CreatedAt.Year == year)
        .GroupBy(i => new { i.CreatedAt.Month, i.CreatedAt.Day })
        .Where(g => g.Count() > 0)
        .Select(g => new
        {
            Date = new DateTime(year, g.Key.Month, g.Key.Day).ToString("yyyy-MM-dd"),
            Count = g.Count()
        })
        .ToListAsync();

        return dailyIncidences
            .Select(x => (x.Date, x.Count))
            .ToList();
    }


}

