using Application.Dtos.Auth.Requests;
using Application.Dtos.CommonDtos.Response;
using FluentResults;

namespace Application.Interfaces.UseCases.Auth;

/// <summary>
/// Use case interface for registering a new user.
/// </summary>
public interface IRegisterUseCase
{
    /// <summary>
    /// Creates a new user asynchronously.
    /// </summary>
    /// <param name="registerRequestDto">The data of the user.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="Result{CreatedResponseDto}"/> indicating success or failure.</returns>
    Task<Result<CreatedResponseDto>> ExecuteAsync(RegisterRequestDto registerRequestDto);
}
