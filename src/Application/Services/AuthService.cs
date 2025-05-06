using Application.Dtos.Auth.Requests;
using Application.Dtos.Auth.Response;
using Application.Dtos.CommonDtos;
using Application.Dtos.CommonDtos.Response;
using Application.Helpers.Utils;
using Application.Helpers.Validators.Auth;
using Application.Interfaces.Services;
using Application.Interfaces.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Utils;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Auth related operations
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IJwt _jwt;
    private readonly IEmailSender _emailSender;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="unitOfWork">The unit of work for database operations.</param>
    /// <param name="mapper">The mapper for object mapping.</param>
    /// <param name="jwt">The jwt service.</param>
    /// <param name="emailSender">The email sender.</param>
    /// <param name="userRepository">The user repository.</param>
    public AuthService(ILogger<AuthService> logger, IUnitOfWork unitOfWork, IMapper mapper, IJwt jwt, IEmailSender emailSender, IUserRepository userRepository)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jwt = jwt;
        _emailSender = emailSender;
        _userRepository = userRepository;
    }

    /// <inheritdoc/>
    public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequestDto)
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
            _logger.LogError("User {Email} does not exists.", loginRequestDto.Email);
            return Result.Fail<LoginResponseDto>("Email or password incorrect.");
        }
        return Result.Ok(_jwt.GenerateJwtToken(user));
    }

    /// <inheritdoc/>
    public async Task<Result<CreatedResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto)
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
            var result = await _userRepository.EmailExistsAsync(user.Email);
            if (result)
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

    /// <inheritdoc/>
    public async Task<Result<SuccessResponseDto>> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto)
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
            _logger.LogError("User {Email} does not exists.", forgotPasswordRequestDto.Email);
            return Result.Fail<SuccessResponseDto>("Email or password incorrect.");
        }
        User? user = await _userRepository.GetUserByEmailAsync(forgotPasswordRequestDto.Email);
        if (user == null)
        {
            _logger.LogError("User {Email} does not exists.", forgotPasswordRequestDto.Email);
            return Result.Fail<SuccessResponseDto>("Email or password incorrect.");
        }
        var token = _jwt.GenerateJwtToken(user).Token;
        await _emailSender.SendRecoverPasswordAsync(user.Email, token);
        return Result.Ok(new SuccessResponseDto());
    }

    /// <inheritdoc/>
    public async Task<Result<SuccessResponseDto>> RecoverPasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto)
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
            _logger.LogError("User {Email} does not exists.", resetPasswordRequestDto.Email);
            return Result.Fail<SuccessResponseDto>("Email or password incorrect.");
        }
        User? user = await _userRepository.GetUserByEmailAsync(resetPasswordRequestDto.Email);
        user.Password = PasswordHasher.HashPassword(resetPasswordRequestDto.Password);
        _userRepository.Update(user);
        await _unitOfWork.SaveAsync();

        return Result.Ok(new SuccessResponseDto());
    }
}
