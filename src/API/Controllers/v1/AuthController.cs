using Application.Dtos.Auth.Requests;
using Application.Dtos.Auth.Response;
using Application.Dtos.CommonDtos;
using Application.Dtos.CommonDtos.Response;
using Application.Interfaces.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1;

/// <summary>
/// Controller for managing user auth-related operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthController"/> class.
/// </remarks>
/// <param name="logger">Logger interface</param>
/// <param name="loginUseCase">The login use case.</param>
/// <param name="registerUseCase">The register use case.</param>
/// <param name="forgotPasswordUseCase">The forgot password use case.</param>
/// <param name="recoverPasswordUseCase">The recover password use case.</param>
[Produces("application/json")]
[Route("api/v1/[controller]")]
public class AuthController(
    ILogger<AuthController> logger,
    ILoginUseCase loginUseCase,
    IRegisterUseCase registerUseCase,
    IForgotPasswordUseCase forgotPasswordUseCase,
    IRecoverPasswordUseCase recoverPasswordUseCase
) : BaseApiController
{
    private readonly ILogger<AuthController> _logger = logger;
    private readonly ILoginUseCase _loginUseCase = loginUseCase;
    private readonly IRegisterUseCase _registerUseCase = registerUseCase;
    private readonly IForgotPasswordUseCase _forgotPasswordUseCase = forgotPasswordUseCase;
    private readonly IRecoverPasswordUseCase _recoverPasswordUseCase = recoverPasswordUseCase;

    /// <summary>
    /// Logs in the user and returns a jwt.
    /// </summary>
    /// <param name="loginDto">The data for the new login.</param>
    /// <returns>.</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto loginDto)
    {
        var loginResult = await _loginUseCase.ExecuteAsync(loginDto);
        if (!loginResult.IsSuccess)
        {
            return BadRequest(loginResult.Errors);
        }
        return Ok(loginResult.Value);
    }

    /// <summary>
    /// Adds a new user.
    /// </summary>
    /// <param name="registerRequestDto">The data for the new user.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreatedResponseDto))]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto registerRequestDto)
    {
        var result = await _registerUseCase.ExecuteAsync(registerRequestDto);
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }

        return Created(string.Empty, result.Value);
    }

    /// <summary>
    /// Recover password
    /// </summary>
    /// <param name="forgotPasswordRequestDto">The data to be able to recover a password of the user.</param>
    /// <returns>a <see cref="SuccessResponseDto"/> indicating if it was able to recover password.</returns>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    public async Task<IActionResult> RecoverPasswordAsync([FromBody] ForgotPasswordRequestDto forgotPasswordRequestDto)
    {
        var result = await _forgotPasswordUseCase.ExecuteAsync(forgotPasswordRequestDto);
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Resets the password
    /// </summary>
    /// <param name="resetPasswordRequestDto">The data to be able to recover a password of the user.</param>
    /// <returns>a <see cref="SuccessResponseDto"/> indicating if it was able to recover password.</returns>
    [Authorize]
    [HttpPut("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
    {
        var result = await _recoverPasswordUseCase.ExecuteAsync(resetPasswordRequestDto);
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }
}