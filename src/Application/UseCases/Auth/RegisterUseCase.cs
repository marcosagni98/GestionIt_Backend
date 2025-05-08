
using Application.Dtos.Auth.Requests;
using Application.Helpers.Validators.Auth;
using Application.Interfaces.UseCases.Auth;
using AutoMapper;
using Domain.Enums;

namespace Application.UseCases.Auth;

/// <summary>
/// Use case for registering a new user.
/// </summary>
public class RegisterUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper, ILogger<RegisterUseCase> logger) : IRegisterUseCase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<RegisterUseCase> _logger = logger;

    /// <inheritdoc/>
    public async Task<Result<CreatedResponseDto>> ExecuteAsync(RegisterRequestDto registerRequestDto)
    {
        var validator = new RegisterRequestDtoValidator();
        var validationResult = await validator.ValidateAsync(registerRequestDto);
        if (!validationResult.IsValid)
        {
            return Result.Fail<CreatedResponseDto>(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var user = _mapper.Map<User>(registerRequestDto);
        if (await _userRepository.CountAsync() == 0)
        {
            user.UserType = UserType.Admin;
        }
        else
        {
            if (await _userRepository.EmailExistsAsync(user.Email))
            {
                _logger.LogError("Email {Email} already exists.", user.Email);
                return Result.Fail<CreatedResponseDto>("Email already exists");
            }
        }

        user.Password = PasswordHasher.HashPassword(registerRequestDto.Password);
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveAsync();

        return Result.Ok(new CreatedResponseDto(user.Id));
    }
}

