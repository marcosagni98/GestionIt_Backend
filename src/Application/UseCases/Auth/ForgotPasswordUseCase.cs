using Application.Dtos.Auth.Requests;
using Application.Dtos.CommonDtos.Response;
using Application.Helpers.Validators.Auth;
using Application.Interfaces.UseCases.Auth;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Utils;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Auth;

/// <summary>
/// Use case for sending a password recovery email.
/// </summary>
public class ForgotPasswordUseCase : IForgotPasswordUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwt _jwt;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ForgotPasswordUseCase> _logger;

    public ForgotPasswordUseCase(IUserRepository userRepository, IJwt jwt, IEmailSender emailSender, ILogger<ForgotPasswordUseCase> logger)
    {
        _userRepository = userRepository;
        _jwt = jwt;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<Result<SuccessResponseDto>> ExecuteAsync(ForgotPasswordRequestDto forgotPasswordRequestDto)
    {
        var validator = new ForgotPasswordRequestDtoValidator();
        var validationResult = await validator.ValidateAsync(forgotPasswordRequestDto);
        if (!validationResult.IsValid)
        {
            string error = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogError(error);
            return Result.Fail<SuccessResponseDto>(error);
        }

        if (!await _userRepository.EmailExistsAsync(forgotPasswordRequestDto.Email))
        {
            _logger.LogError("User {Email} does not exist.", forgotPasswordRequestDto.Email);
            return Result.Fail<SuccessResponseDto>("Email does not exist.");
        }

        User? user = await _userRepository.GetUserByEmailAsync(forgotPasswordRequestDto.Email);
        if (user == null)
        {
            _logger.LogError("User {Email} does not exist.", forgotPasswordRequestDto.Email);
            return Result.Fail<SuccessResponseDto>("Email does not exist.");
        }

        var token = _jwt.GenerateJwtToken(user).Token;
        try
        {
            await _emailSender.SendRecoverPasswordAsync(user.Email, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send recovery email to {Email}", user.Email);
            return Result.Fail<SuccessResponseDto>("Failed to send recovery email. Please try again later.");
        }

        return Result.Ok(new SuccessResponseDto());
    }
}

