using Application.Dtos.Auth.Requests;
using Application.Dtos.CommonDtos.Response;
using FluentResults;

namespace Application.Interfaces.UseCases.Auth;

/// <summary>
/// Use case interface for recovering a user's password.
/// </summary>
public interface IRecoverPasswordUseCase
{
    /// <summary>
    /// Updates the user's password asynchronously.
    /// </summary>
    /// <param name="resetPasswordRequestDto">DTO that contains the new password.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="Result{SuccessResponseDto}"/> indicating success or failure.</returns>
    Task<Result<SuccessResponseDto>> ExecuteAsync(ResetPasswordRequestDto resetPasswordRequestDto);
}
