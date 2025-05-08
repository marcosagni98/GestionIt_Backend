using Application.Dtos.Auth.Requests;

namespace Application.Interfaces.UseCases.Auth;

/// <summary>
/// Use case interface for sending a password recovery email.
/// </summary>
public interface IForgotPasswordUseCase
{
    /// <summary>
    /// Sends a password recovery email asynchronously.
    /// </summary>
    /// <param name="forgotPasswordRequestDto">DTO that contains the user's email.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="Result{SuccessResponseDto}"/> indicating success or failure.</returns>
    Task<Result<SuccessResponseDto>> ExecuteAsync(ForgotPasswordRequestDto forgotPasswordRequestDto);
}
