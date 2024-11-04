using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.Users;
using Application.Dtos.CRUD.Users.Request;
using Domain.Enums;
using FluentResults;

namespace Application.Interfaces.Services;

/// <summary>
/// User Service Interface
/// </summary>
public interface IUserService : IBaseService<UserDto, UserAddRequestDto, UserUpdateRequestDto>, IDisposable
{
    /// <summary>
    /// Verifies the specified user ID and returns the corresponding user ID if valid.
    /// </summary>
    /// <param name="userId">The ID of the user to verify.</param>
    /// <returns>
    /// An asynchronous task representing a <see cref="Result{long}"/> containing the verified user ID 
    /// if the verification is successful; otherwise, an error result.
    /// </returns>
    public Task<Result<long>> VerifyUserAsync(string userId);

    /// <summary>
    /// Updates the user type for the specified user ID.
    /// </summary>
    /// <param name="userId">The ID of the user to update.</param>
    /// <param name="userType">The new userType to set for the user.</param>
    /// <returns>
    /// An asynchronous task representing a <see cref="Result{SuccessResponseDto}"/> containing the result of the update operation.
    /// </returns>
    public Task<Result<SuccessResponseDto>> UpdateUserTypeAsync(long userId, UserType userType);

    /// <summary>
    /// Retrieves a list of users with the technician role.
    /// </summary>
    /// <returns>
    /// An asynchronous task representing a <see cref="Result{List{UserDto}}"/> containing the list of technicians.
    /// </returns>
    public Task<Result<List<UserDto>>> GetTechniciansAsync();
}
