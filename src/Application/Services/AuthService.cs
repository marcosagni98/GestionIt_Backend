using Application.Dtos.Auth.Requests;
using Application.Dtos.Auth.Response;
using Application.Dtos.CommonDtos;
using Application.Dtos.CommonDtos.Response;
using Application.Interfaces.Services;
using Application.Interfaces.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Utils;
using FluentResults;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for database operations.</param>
    /// <param name="mapper">The mapper for object mapping.</param>
    /// <param name="jwt">The jwt service.</param>
    /// <param name="emailSender">The email sender.</param>
    public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IJwt jwt, IEmailSender emailSender)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jwt = jwt;
        _emailSender = emailSender;
    }

    #region Dispose
    private bool disposed = false;

    /// <summary>
    /// Releases the unmanaged resources used by the service and optionally releases
    /// the managed resources if disposing is true.
    /// </summary>
    /// <param name="disposing">Indicates whether the method was called directly
    /// or from a finalizer. If true, the method has been called directly
    /// and managed resources should be disposed. If false, it was called by the
    /// runtime from inside the finalizer and only unmanaged resources should be disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
        }
        disposed = true;
    }

    /// <summary>
    /// Releases all resources used by the service.
    /// This method is called by consumers of the service when they are done
    /// using it to free resources promptly.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion

    /// <inheritdoc/>
    public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequestDto)
    {
        if (!await _unitOfWork.UserRepository.LoginAsync(loginRequestDto.Email, loginRequestDto.Password))
        {
            return Result.Fail<LoginResponseDto>("Email or password incorrect.");
        }
        User? user = await _unitOfWork.UserRepository.GetUserByEmailAsync(loginRequestDto.Email);
        if (user == null)
        {
            return Result.Fail<LoginResponseDto>("User does not exists.");
        }
        return Result.Ok(_jwt.GenerateJwtToken(user));
    }

    /// <inheritdoc/>
    public async Task<Result<CreatedResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto)
    {
        var user = _mapper.Map<User>(registerRequestDto);
        if (await _unitOfWork.UserRepository.CountAsync() == 0)
        {
            user.UserType = UserType.Admin;
        }
        else
        {
            var result = await _unitOfWork.UserRepository.EmailExistsAsync(user.Email);
            if (result)
            {
                return Result.Fail<CreatedResponseDto>("Email already exists.");
            }
        }
        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(new CreatedResponseDto(user.Id));
    }

    /// <inheritdoc/>
    public async Task<Result<SuccessResponseDto>> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto)
    {
        if(!await _unitOfWork.UserRepository.EmailExistsAsync(forgotPasswordRequestDto.Email))
        {
            return Result.Fail<SuccessResponseDto>("Email does not exists.");
        }
        User? user = await _unitOfWork.UserRepository.GetUserByEmailAsync(forgotPasswordRequestDto.Email);
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
        if(!await _unitOfWork.UserRepository.EmailExistsAsync(resetPasswordRequestDto.Email))
        {
            return Result.Fail<SuccessResponseDto>("User not found.");
        }
        User? user = await _unitOfWork.UserRepository.GetUserByEmailAsync(resetPasswordRequestDto.Email);
        user!.Password = resetPasswordRequestDto.Password;
        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(new SuccessResponseDto());
    }
}
