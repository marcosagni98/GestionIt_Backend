using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.Incidents;
using Application.Dtos.CRUD.Incidents.Request;
using Application.Interfaces.Services;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers.v1;

/// <summary>
/// Controller for managing incident-related operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="IncidentController"/> class.
/// </remarks>
/// <param name="incidentService">The incident service.</param>
[Produces("application/json")]
[Route("api/v1/[controller]")]
public class IncidentController(ILogger<IncidentController>  logger, IIncidentService incidentService) : BaseApiController
{
    private readonly ILogger<IncidentController> _logger = logger;
    private readonly IIncidentService _incidentService = incidentService;

    /// <summary>
    /// Adds a new incident.
    /// </summary>
    /// <param name="addRequestDto">The data for the new incident.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResponseDto))]
    public async Task<IActionResult> AddAsync([FromBody] IncidentAddRequestDto addRequestDto)
    {
        if (!long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long userId))
        {
            return Unauthorized();
        }
        addRequestDto.UserId = userId;
        var result = await _incidentService.AddAsync(addRequestDto);
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }

        return Created(string.Empty, result.Value);
    }

    /// <summary>
    /// Gets a list of incidents based on the provided query filter.
    /// </summary>
    /// <param name="queryFilter">The filtering, sorting, and pagination parameters.</param>
    /// <returns>A paginated list of incidents.</returns>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<IncidentDto>))]
    public async Task<IActionResult> GetAsync([FromQuery] QueryFilterDto queryFilter)
    {
        if (!long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long userId))
        {
            return Unauthorized();
        }
        var result = await _incidentService.GetAsync(queryFilter, userId);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a list of incidents.
    /// </summary>
    /// <returns>A list of incidents.</returns>
    [Authorize]
    [HttpGet("Historic")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<IncidentDto>))]
    public async Task<IActionResult> GetHistoricAsync([FromQuery] QueryFilterDto queryFilter)
    {
        var result = await _incidentService.GetHistoricAsync(queryFilter);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a incident by ID.
    /// </summary>
    /// <param name="id">The ID of the incident to retrieve.</param>
    /// <returns>The requested incident.</returns>
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IncidentDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var result = await _incidentService.GetByIdAsync(id);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a list of incidents filtered by priority.
    /// </summary>
    /// <param name="priorityId">The ID of the priority to filter incidents by.</param>
    /// <param name="queryFilter">The filtering, sorting, and pagination parameters.</param>
    /// <returns>A paginated list of incidents filtered by the specified priority.</returns>
    [Authorize]
    [HttpGet("by-priority/{priorityId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<IncidentDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPriorityAsync(Priority priorityId, [FromQuery] QueryFilterDto queryFilter)
    {
        if (!long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long userId))
        {
            return Unauthorized();
        }
        var result = await _incidentService.GetByPriorityAsync(queryFilter, priorityId, userId);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Updates the status of an incident.
    /// </summary>
    /// <param name="id">The ID of the incident to update.</param>
    /// <param name="updateStatusRequestDto">The updated data for the incident.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [Authorize(Roles = "0, 1, 2")]
    [HttpPut("update-status/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStatusAsync(long id, [FromBody] IncidentUpdateStatusRequestDto updateStatusRequestDto)
    {
        if (updateStatusRequestDto == null)
        {
            return BadRequest("The updateStatusRequestDto field is required.");
        }
        var result = await _incidentService.UpdateStatusAsync(id, updateStatusRequestDto);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Updates the priority of an incident.
    /// </summary>
    /// <param name="id">The ID of the incident to update.</param>
    /// <param name="updateStatusRequestDto">The updated data for the incident.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [Authorize]
    [HttpPut("update-priority/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePriorityAsync(long id, [FromBody] IncidentUpdatePriorityRequestDto updatePriorityRequestDto)
    {
        var result = await _incidentService.UpdatePriorityAsync(id, updatePriorityRequestDto);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Updates the technician assigned to a incident.
    /// </summary>
    /// <param name="id">The ID of the incident to update.</param>
    /// <param name="updateTechnicianRequestDto">The data to be updated.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [Authorize]
    [HttpPut("set-technician/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTechnician(long id, [FromBody] IncidentUpdateTechnicianRequestDto updateTechnicianRequestDto)
    {
        var result = await _incidentService.UpdateTechnicianAsync(id, updateTechnicianRequestDto);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpPut("update-title-description/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTitleAndDescription(long id, [FromBody] IncidentUpdateTitleDescriptionRequestDto updateTitleDescriptionRequestDto)
    {
        var result = await _incidentService.UpdateTitleAndDescription(id, updateTitleDescriptionRequestDto);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Deletes a incident by ID.
    /// </summary>
    /// <param name="id">The ID of the incident to delete.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var result = await _incidentService.DeleteAsync(id);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }
}
