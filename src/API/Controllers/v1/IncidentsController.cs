using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.Incidents;
using Application.Dtos.CRUD.Incidents.Request;
using Application.Interfaces.Services;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Microsoft.AspNetCore.Mvc;

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
public class IncidentController(IIncidentService incidentService) : BaseApiController
{
    private readonly IIncidentService _incidentService = incidentService;

    /// <summary>
    /// Adds a new incident.
    /// </summary>
    /// <param name="addRequestDto">The data for the new incident.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResponseDto))]
    public async Task<IActionResult> AddAsync([FromBody] IncidentAddRequestDto addRequestDto)
    {
        var result = await _incidentService.AddAsync(addRequestDto);
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }

        return Created(string.Empty, result.Value);
    }

    /// <summary>
    /// Gets a list of incidents.
    /// </summary>
    /// <returns>A list of incidents.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<IncidentDto>))]
    public async Task<IActionResult> GetAsync([FromQuery] QueryFilterDto queryFilter)
    {
        var result = await _incidentService.GetAsync(queryFilter);
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
    /// Updates an existing incident.
    /// </summary>
    /// <param name="id">The ID of the incident to update.</param>
    /// <param name="updateRequestDto">The updated data for the incident.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] IncidentUpdateRequestDto updateRequestDto)
    {
        var result = await _incidentService.UpdateAsync(id, updateRequestDto);
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
