using Application.Dtos.CRUD.UserFeedbacks;
using Application.Dtos.CRUD.UserFeedbacks.Request;

namespace Application.Interfaces.Services;

/// <summary>
/// UserFeedback Service Interface
/// </summary>
public interface IUserFeedbackService : IBaseService<UserFeedbackDto, UserFeedbackAddRequestDto, UserFeedbackUpdateRequestDto>, IDisposable
{
}
