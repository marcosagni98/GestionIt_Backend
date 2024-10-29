using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.UserFeedbacks;
using Application.Dtos.CRUD.UserFeedbacks.Request;
using Application.Interfaces;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1;

/// <summary>
/// Controller for managing userfeedback-related operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserFeedbackController"/> class.
/// </remarks>
/// <param name="userfeedbackService">The userfeedback service.</param>
[Produces("application/json")]
[Route("api/v1/[controller]")]
public class UserFeedbackController(IUserFeedbackService userfeedbackService) : BaseApiController
{
    private readonly IUserFeedbackService _userfeedbackService = userfeedbackService;

    /// <summary>
    /// Adds a new userfeedback.
    /// </summary>
    /// <param name="addRequestDto">The data for the new userfeedback.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResponseDto))]
    public async Task<IActionResult> AddAsync([FromBody] UserFeedbackAddRequestDto addRequestDto)
    {
        var result = await _userfeedbackService.AddAsync(addRequestDto);
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a list of userfeedbacks.
    /// </summary>
    /// <returns>A list of userfeedbacks.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<UserFeedbackDto>))]
    public async Task<IActionResult> GetAsync([FromQuery] QueryFilterDto queryFilter)
    {
        var result = await _userfeedbackService.GetAsync(queryFilter);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a userfeedback by ID.
    /// </summary>
    /// <param name="id">The ID of the userfeedback to retrieve.</param>
    /// <returns>The requested userfeedback.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserFeedbackDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var result = await _userfeedbackService.GetByIdAsync(id);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Updates an existing userfeedback.
    /// </summary>
    /// <param name="id">The ID of the userfeedback to update.</param>
    /// <param name="updateRequestDto">The updated data for the userfeedback.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] UserFeedbackUpdateRequestDto updateRequestDto)
    {
        var result = await _userfeedbackService.UpdateAsync(id, updateRequestDto);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Deletes a userfeedback by ID.
    /// </summary>
    /// <param name="id">The ID of the userfeedback to delete.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var result = await _userfeedbackService.DeleteAsync(id);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }
}
