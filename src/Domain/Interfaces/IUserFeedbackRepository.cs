using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserFeedbackRepository : IBaseRepository<UserFeedback>, IDisposable
{
}
