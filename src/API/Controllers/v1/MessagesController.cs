﻿using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.Messages;
using Application.Dtos.CRUD.Messages.Request;
using Application.Interfaces.Services;
using Domain.Dtos.CommonDtos.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1;

/// <summary>
/// Controller for managing message-related operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MessageController"/> class.
/// </remarks>
/// <param name="logger">Logger interface</param>
/// <param name="messageService">The message service.</param>
[Produces("application/json")]
[Route("api/v1/[controller]")]
public class MessageController(ILogger<MessageController> logger, IMessageService messageService) : BaseApiController
{
    private readonly ILogger<MessageController> _logger = logger;
    private readonly IMessageService _messageService = messageService;

    /// <summary>
    /// Gets a list of messages by incident id.
    /// </summary>
    /// <param name="incidentId">The ID of the incident.</param></param>
    /// <returns>A list of messages.</returns>
    [Authorize]
    [HttpGet("{incidentId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<MessageDto>))]
    public async Task<IActionResult> GetByIncidentIdAsync(long incidentId)
    {
        var result = await _messageService.GetByIncidentIdAsync(incidentId);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }


    /// <summary>
    /// Gets a list of messages by incident id.
    /// </summary>
    /// <param name="incidentId">The ID of the incident.</param></param>
    /// <returns>A list of messages.</returns>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    public async Task<IActionResult> AddASYNC(MessageAddRequestDto incidentId)
    {
        var result = await _messageService.AddAsync(incidentId);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }
}
