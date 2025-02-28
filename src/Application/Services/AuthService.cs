using Application.Dtos.Auth.Requests;
using Application.Dtos.Auth.Response;
using Application.Dtos.CommonDtos;
using Application.Dtos.CommonDtos.Response;
using Application.Interfaces.Services;
using Application.Interfaces.Utils;
using Application.Validators.Auth;
using Application.Utils;
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
public class AuthService : IAuthService
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
            return Result.Fail<LoginResponseDto>(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var hashedPassword = PasswordHasher.HashPassword(loginRequestDto.Password);
        if (!await _userRepository.LoginAsync(loginRequestDto.Email, hashedPassword))
        {
            return Result.Fail<LoginResponseDto>("Email or password incorrect.");
        }
        
        User? user = await _userRepository.GetUserByEmailAsync(loginRequestDto.Email);
        if (user == null)
        {
            return Result.Fail<LoginResponseDto>("User does not exists.");
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
                return Result.Fail<CreatedResponseDto>("Email already exists.");
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
            return Result.Fail<SuccessResponseDto>(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        if (!await _userRepository.EmailExistsAsync(forgotPasswordRequestDto.Email))
        {
            return Result.Fail<SuccessResponseDto>("Email does not exists.");
        }
        User? user = await _userRepository.GetUserByEmailAsync(forgotPasswordRequestDto.Email);
        if (user == null)
        {
            return Result.Fail<SuccessResponseDto>("User does not exists.");
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
            return Result.Fail<SuccessResponseDto>(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        if (!await _userRepository.EmailExistsAsync(resetPasswordRequestDto.Email))
        {
            return Result.Fail<SuccessResponseDto>("User not found.");
        }
        User? user = await _userRepository.GetUserByEmailAsync(resetPasswordRequestDto.Email);
        user.Password = PasswordHasher.HashPassword(resetPasswordRequestDto.Password);
        _userRepository.Update(user);
        await _unitOfWork.SaveAsync();

        return Result.Ok(new SuccessResponseDto());
    }
}
