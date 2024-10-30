using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.Messages;
using Application.Dtos.CRUD.Messages.Request;
using Application.Interfaces.Services;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1;

/// <summary>
/// Controller for managing message-related operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MessageController"/> class.
/// </remarks>
/// <param name="messageService">The message service.</param>
[Produces("application/json")]
[Route("api/v1/[controller]")]
public class MessageController(IMessageService messageService) : BaseApiController
{
    private readonly IMessageService _messageService = messageService;

    /// <summary>
    /// Adds a new message.
    /// </summary>
    /// <param name="addRequestDto">The data for the new message.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResponseDto))]
    public async Task<IActionResult> AddAsync([FromBody] MessageAddRequestDto addRequestDto)
    {
        var result = await _messageService.AddAsync(addRequestDto);
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a list of messages.
    /// </summary>
    /// <returns>A list of messages.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<MessageDto>))]
    public async Task<IActionResult> GetAsync([FromQuery] QueryFilterDto queryFilter)
    {
        var result = await _messageService.GetAsync(queryFilter);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a message by ID.
    /// </summary>
    /// <param name="id">The ID of the message to retrieve.</param>
    /// <returns>The requested message.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var result = await _messageService.GetByIdAsync(id);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Created(string.Empty, result.Value);
    }

    /// <summary>
    /// Updates an existing message.
    /// </summary>
    /// <param name="id">The ID of the message to update.</param>
    /// <param name="updateRequestDto">The updated data for the message.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] MessageUpdateRequestDto updateRequestDto)
    {
        var result = await _messageService.UpdateAsync(id, updateRequestDto);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Deletes a message by ID.
    /// </summary>
    /// <param name="id">The ID of the message to delete.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var result = await _messageService.DeleteAsync(id);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }
}
