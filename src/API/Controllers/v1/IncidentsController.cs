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

        //TODO: Añadir obtner el usertype id del jwt
        var result = await _incidentService.GetAsync(queryFilter, 1);
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
    /// Updates the status of an incident.
    /// </summary>
    /// <param name="id">The ID of the incident to update.</param>
    /// <param name="updateStatusRequestDto">The updated data for the incident.</param>
    /// <returns>A response indicating the result of the operation.</returns>
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
    /// Updates the tecnitian assigned to a incident.
    /// </summary>
    /// <param name="id">The ID of the incident to update.</param>
    /// <param name="updateTechnitianRequestDto">The data to be updated.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [HttpPut("set-tecnitian/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTechnitian(long id, [FromBody] IncidentUpdateTechnitianRequestDto updateTechnitianRequestDto)
    {
        var result = await _incidentService.UpdateTechnitianAsync(id, updateTechnitianRequestDto);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

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
