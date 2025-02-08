using Application.Dtos.Auth.Requests;
using Application.Dtos.Auth.Response;
using FluentResults;
using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CommonDtos;

namespace Application.Interfaces.Services;

/// <summary>
/// Auth Service Interface
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Logs in the user asynchronously
    /// </summary>
    /// <param name="loginRequestDto">The data of the user</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="Result{LoginResponseDto}"/> indicating success or failure.</returns>
    public Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequestDto);

    /// <summary>
    /// Creates a new user asynchronously
    /// </summary>
    /// <param name="registerRequestDto">The data of the user</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="Result{SuccessResponseDto}"/> indicating success or failure.</returns>
    public Task<Result<CreatedResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto);

    /// <summary>
    /// Sends a password recovery email asynchronously
    /// </summary>
    /// <param name="forgotPasswordRequestDto">dto that contains the user</param>
    /// <returns>A <see cref="SuccessResponseDto"/></returns>
    public Task<Result<SuccessResponseDto>> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto);

    /// <summary>
    /// Updates the user's password asynchronously
    /// </summary>
    /// <param name="resetPasswordRequestDto">dto that contains the new password</param>
    /// <returns>A <see cref="SuccessResponseDto"/></returns>
    public Task<Result<SuccessResponseDto>> RecoverPasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto);
}