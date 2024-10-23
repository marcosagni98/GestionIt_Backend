using Application.Dtos.CRUD.UserFeedbacks;
using Application.Dtos.CRUD.UserFeedbacks.Request;
using Application.Dtos.CRUD.UserFeedbacks.Response;

namespace Application.Interfaces;

public interface IUserFeedbackService : IBaseService<UserFeedbackDto, UserFeedbackResponseDto, UserFeedbackAddRequestDto, UserFeedbackUpdateRequestDto>, IDisposable
{
}
