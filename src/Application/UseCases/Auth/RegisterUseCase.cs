using Application.Dtos.Auth.Requests;
using Application.Dtos.CommonDtos.Response;
using Application.Helpers.Utils;
using Application.Helpers.Validators.Auth;
using Application.Interfaces.UseCases.Auth;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Auth;

/// <summary>
/// Use case for registering a new user.
/// </summary>
public class RegisterUseCase : IRegisterUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RegisterUseCase> _logger;

    public RegisterUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper, ILogger<RegisterUseCase> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

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

