using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
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

    public async Task<int> GetUserHappinessAsync(DateTime startDate, DateTime endDate)
    {
        var averageRating = await _dbSet
            .Where(f => f.SubmittedAt >= startDate && f.SubmittedAt <= endDate)
            .Select(f => (double?)f.Rating)
            .AverageAsync();

        return averageRating.HasValue ? (int)Math.Round(averageRating.Value) : 0;
    }
}