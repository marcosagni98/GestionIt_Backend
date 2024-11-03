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
        List<string> searchParameters = new List<string>();

        var totalCount = await CountAsync(queryFilter, searchParameters);

        IQueryable<UserFeedback> query = _dbSet.AsQueryable();

        query = new QueryFilterBuilder<UserFeedback>(_dbSet)
            .ApplyQueryFilterAndActive(queryFilter, searchParameters)
            .Build()
            .Include(x => x.User);

        var items = await query.ToListAsync();

        return new PaginatedList<UserFeedback>(items, totalCount);
    }

    /// <inheritdoc/>
    public override async Task<UserFeedback?> GetByIdAsync(long id)
    {
        return await _dbSet
            .Where(x => x.Active == true && x.Id == id)
            .Include(x => x.User)
            .FirstOrDefaultAsync();
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
           .Where(f => f.SubmittedAt >= startDate && f.SubmittedAt <= endDate && f.UserId == id && f.Active == true)
           .Select(f => (double?)f.Rating)
           .AverageAsync();

        return averageRating.HasValue ? (int)Math.Round(averageRating.Value) : 0;
    }
}