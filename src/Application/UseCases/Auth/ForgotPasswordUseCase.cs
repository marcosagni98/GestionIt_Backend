using Application.Dtos.Auth.Requests;
using Application.Helpers.Validators.Auth;
using Application.Interfaces.UseCases.Auth;

namespace Application.UseCases.Auth;

/// <summary>
/// Use case for sending a password recovery email.
/// </summary>
public class ForgotPasswordUseCase(IUserRepository userRepository, IJwt jwt, IEmailSender emailSender, ILogger<ForgotPasswordUseCase> logger) : IForgotPasswordUseCase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwt _jwt = jwt;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly ILogger<ForgotPasswordUseCase> _logger = logger;

    /// <inheritdoc/>
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

