using Application.Dtos.Auth.Requests;
using Application.Helpers.Validators.Auth;
using Application.Interfaces.UseCases.Auth;

namespace Application.UseCases.Auth;

/// <summary>
/// Use case for recovering a user's password.
/// </summary>
public class RecoverPasswordUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<RecoverPasswordUseCase> logger) : IRecoverPasswordUseCase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<RecoverPasswordUseCase> _logger = logger;

    /// <inheritdoc/>
    public async Task<Result<SuccessResponseDto>> ExecuteAsync(ResetPasswordRequestDto resetPasswordRequestDto)
    {
        var validator = new ResetPasswordRequestDtoValidator();
        var validationResult = await validator.ValidateAsync(resetPasswordRequestDto);
        if (!validationResult.IsValid)
        {
            string error = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogError(error);
            return Result.Fail<SuccessResponseDto>(error);
        }

        if (!await _userRepository.EmailExistsAsync(resetPasswordRequestDto.Email))
        {
            _logger.LogError("User {Email} does not exist.", resetPasswordRequestDto.Email);
            return Result.Fail<SuccessResponseDto>("Email does not exist.");
        }

        User? user = await _userRepository.GetUserByEmailAsync(resetPasswordRequestDto.Email);
        user.Password = PasswordHasher.HashPassword(resetPasswordRequestDto.Password);
        _userRepository.Update(user);
        await _unitOfWork.SaveAsync();

        return Result.Ok(new SuccessResponseDto());
    }
}

