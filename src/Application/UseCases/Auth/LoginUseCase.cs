using Application.Dtos.Auth.Requests;
using Application.Dtos.Auth.Response;
using Application.Helpers.Utils;
using Application.Helpers.Validators.Auth;
using Application.Interfaces.UseCases.Auth;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Utils;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Auth;

/// <summary>
/// Use case for logging in a user.
/// </summary>
public class LoginUseCase : ILoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwt _jwt;
    private readonly ILogger<LoginUseCase> _logger;

    public LoginUseCase(IUserRepository userRepository, IJwt jwt, ILogger<LoginUseCase> logger)
    {
        _userRepository = userRepository;
        _jwt = jwt;
        _logger = logger;
    }

    public async Task<Result<LoginResponseDto>> ExecuteAsync(LoginRequestDto loginRequestDto)
    {
        var validator = new LoginRequestDtoValidator();
        var validationResult = await validator.ValidateAsync(loginRequestDto);
        if (!validationResult.IsValid)
        {
            string error = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogError(error);
            return Result.Fail<LoginResponseDto>(error);
        }

        var hashedPassword = PasswordHasher.HashPassword(loginRequestDto.Password);
        if (!await _userRepository.LoginAsync(loginRequestDto.Email, hashedPassword))
        {
            string error = "Email or password incorrect.";
            _logger.LogError(error);
            return Result.Fail<LoginResponseDto>(error);
        }

        User? user = await _userRepository.GetUserByEmailAsync(loginRequestDto.Email);
        if (user == null)
        {
            _logger.LogError("User {Email} does not exist.", loginRequestDto.Email);
            return Result.Fail<LoginResponseDto>("Email or password incorrect.");
        }

        return Result.Ok(_jwt.GenerateJwtToken(user));
    }
}