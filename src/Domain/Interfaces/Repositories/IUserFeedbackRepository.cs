using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IUserFeedbackRepository : IGenericRepository<UserFeedback>
{
    public Task<int> GetUserHappinessAsync(DateTime startDate, DateTime endDate);
}
