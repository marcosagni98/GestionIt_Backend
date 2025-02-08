using Application.Dtos.CRUD.UserFeedbacks;
using Application.Dtos.CRUD.UserFeedbacks.Request;
using FluentResults;

namespace Application.Interfaces.Services;

/// <summary>
/// Retrieves user feedback by incident ID.
/// </summary>
public interface IUserFeedbackService : IBaseService<UserFeedbackDto, UserFeedbackAddRequestDto, UserFeedbackUpdateRequestDto>
{
    /// <summary>
    /// Retrieves user feedback by incident ID.
    /// </summary>
    /// <param name="incident">The ID of the incident.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user feedback DTO.</returns>
    public Task<Result<UserFeedbackDto>> GetByIncidentIdAsync(long incident);
}
