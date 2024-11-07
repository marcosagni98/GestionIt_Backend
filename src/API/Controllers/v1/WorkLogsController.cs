using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.WorkLogs;
using Application.Dtos.CRUD.WorkLogs.Request;
using Application.Interfaces.Services;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers.v1;

/// <summary>
/// Controller for managing worklog-related operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WorkLogController"/> class.
/// </remarks>
/// <param name="worklogService">The worklog service.</param>
[Produces("application/json")]
[Route("api/v1/[controller]")]
public class WorkLogController(IWorkLogService worklogService) : BaseApiController
{
    private readonly IWorkLogService _worklogService = worklogService;

    /// <summary>
    /// Adds a new worklog.
    /// </summary>
    /// <param name="addRequestDto">The data for the new worklog.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResponseDto))]
    public async Task<IActionResult> AddAsync([FromBody] WorkLogAddRequestDto addRequestDto)
    {
        if (!long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long userId))
        {
            return Unauthorized();
        }
        addRequestDto.TechnicianId = userId;
        var result = await _worklogService.AddAsync(addRequestDto);
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }

        return Created(string.Empty, result.Value);
    }

    /// <summary>
    /// Gets a worklog by incidentId.
    /// </summary>
    /// <param name="incidentId">The ID of the incident id related to worklog to retrieve.</param>
    /// <returns>The requested worklogs.</returns>
    [Authorize]
    [HttpGet("Incident/{incidentId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<WorkLogDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIncidentIdAsync(long incidentId)
    {
        var result = await _worklogService.GetByIncidentIdAsync(incidentId);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }
}
