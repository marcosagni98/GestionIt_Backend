using Application.Dtos.CRUD.Users;
using Application.Dtos.CRUD.Users.Request;
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
    /// An asynchronous task representing a <see cref="{Result{long}}"/> containing the verified user ID 
    /// if the verification is successful; otherwise, an error result.
    /// </returns>
    public Task<Result<long>> VerifyUserAsync(string userId);
}
