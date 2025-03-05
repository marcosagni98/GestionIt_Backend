using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.Users;
using Application.Interfaces.Services;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1;

/// <summary>
/// Controller for managing user-related operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserController"/> class.
/// </remarks>
/// <param name="userService">The user service.</param>
[Produces("application/json")]
[Route("api/v1/[controller]")]
public class UserController(ILogger<UserController> logger, IUserService userService) : BaseApiController
{
    private readonly ILogger<UserController> _logger = logger;
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Gets a list of users.
    /// </summary>
    /// <returns>A list of users.</returns>
    [Authorize(Roles = "2")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<UserDto>))]
    public async Task<IActionResult> GetAsync([FromQuery] QueryFilterDto queryFilter)
    {
        var result = await _userService.GetAsync(queryFilter);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a list of users technicians.
    /// </summary>
    /// <returns>A list of users technicians.</returns>
    [Authorize(Roles = "2")]
    [HttpGet("technicians")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserDto>))]
    public async Task<IActionResult> GetTechniciansAsync()
    {
        var result = await _userService.GetTechniciansAsync();
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Updates an existing user userType.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="userType">The new usertype.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [Authorize(Roles = "2")]
    [HttpPut("update-user-type/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserTypeAsync(long id, [FromBody] UserType userType)
    {
        var result = await _userService.UpdateUserTypeAsync(id, userType);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Deletes a user by ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [Authorize(Roles = "2")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var result = await _userService.DeleteAsync(id);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }
}
