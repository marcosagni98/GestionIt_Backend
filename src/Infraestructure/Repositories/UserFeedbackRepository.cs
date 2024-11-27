using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infraestructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class UserFeedbackRepository : GenericRepository<UserFeedback>, IUserFeedbackRepository
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<UserFeedback> _dbSet;
    private readonly IMapper _mapper;

    public UserFeedbackRepository(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
        _dbContext = context;
        _mapper = mapper;
        _dbSet = _dbContext.Set<UserFeedback>();
    }

    /// <inheritdoc/>
    public override async Task<PaginatedList<UserFeedback>> GetAsync(QueryFilterDto queryFilter)
    {
        List<string> searchParameters = ["User.Name", "Feedback"];

        GetCorrectQueryFilterOrderBy(queryFilter);

        var totalCount = await CountAsync(queryFilter, searchParameters);

        IQueryable<UserFeedback> query = _dbSet
            .Include(x => x.User);

        query = new QueryFilterBuilder<UserFeedback>(query)
            .ApplyQueryFilterAndActive(queryFilter, searchParameters)
            .Build()
            .Include(x => x.User);

        var items = await query.ToListAsync();

        return new PaginatedList<UserFeedback>(items, totalCount);
    }

    #region GetAsync private functions
    /// <summary>
    /// Transforms sorting property names from Data Transfer Object (DTO) conventions to corresponding entity navigation property names.
    /// This method maps client-side sorting properties to the correct database entity property paths for efficient querying.
    /// </summary>
    /// <param name="queryFilter">The query filter containing sorting parameters to be transformed.</param>
    private static void GetCorrectQueryFilterOrderBy(QueryFilterDto queryFilter)
    {
        if (!string.IsNullOrWhiteSpace(queryFilter?.OrderBy))
        {
            queryFilter.OrderBy = queryFilter.OrderBy switch
            {
                "userName" => "User.Name",
                _ => queryFilter.OrderBy
            };
        }
    }

    #endregion

    /// <inheritdoc/>
    public override async Task<UserFeedback?> GetByIdAsync(long id)
    {
        return await _dbSet
            .Where(x => x.Active == true && x.Id == id)
            .Include(x => x.User)
            .FirstOrDefaultAsync();
    }

    public async Task<UserFeedback?> GetByIncidentIdAsync(long incidentId)
    {
        return await _dbSet
        .Where(x => x.IncidentId == incidentId)
        .OrderBy(x => x.SubmittedAt)
        .LastOrDefaultAsync();
    }

    public async Task<int> GetUserHappinessAsync(DateTime startDate, DateTime endDate)
    {
        var averageRating = await _dbSet
            .Where(f => f.SubmittedAt >= startDate && f.SubmittedAt <= endDate && f.Active == true)
            .Select(f => (double?)f.Rating)
            .AverageAsync();

        return averageRating.HasValue ? (int)Math.Round(averageRating.Value) : 0;
    }

    public async Task<int> GetUserHappinessAsync(DateTime startDate, DateTime endDate, long id)
    {
        var averageRating = await _dbSet
           .Include(i => i.Incident)
           .Where(f => f.SubmittedAt >= startDate && f.SubmittedAt <= endDate && f.Incident!.TechnicianId == id && f.Active == true)
           .Select(f => (double?)f.Rating)
           .AverageAsync();

        return averageRating.HasValue ? (int)Math.Round(averageRating.Value) : 0;
    }
}