using Application.Dtos.Auth.Requests;
using Application.Dtos.Auth.Response;

namespace Application.Interfaces.UseCases.Auth;

/// <summary>
/// Use case interface for logging in a user.
/// </summary>
public interface ILoginUseCase
{
    /// <summary>
    /// Logs in the user asynchronously.
    /// </summary>
    /// <param name="loginRequestDto">The data of the user.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="Result{LoginResponseDto}"/> indicating success or failure.</returns>
    Task<Result<LoginResponseDto>> ExecuteAsync(LoginRequestDto loginRequestDto);
}
