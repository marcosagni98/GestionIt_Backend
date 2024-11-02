using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.IncidentHistories;
using Application.Dtos.CRUD.IncidentHistories.Request;
using Application.Interfaces.Services;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1;

/// <summary>
/// Controller for managing incidenthistory-related operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="IncidentHistoryController"/> class.
/// </remarks>
/// <param name="incidentHistoryService">The incidenthistory service.</param>
[Produces("application/json")]
[Route("api/v1/[controller]")]
public class IncidentHistoryController(IIncidentHistoryService incidentHistoryService) : BaseApiController
{
    private readonly IIncidentHistoryService _incidentHistoryService = incidentHistoryService;

    /// <summary>
    /// Adds a new incidenthistory.
    /// </summary>
    /// <param name="addRequestDto">The data for the new incidenthistory.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResponseDto))]
    public async Task<IActionResult> AddAsync([FromBody] IncidentHistoryAddRequestDto addRequestDto)
    {
        var result = await _incidentHistoryService.AddAsync(addRequestDto);
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }

        return Created(string.Empty, result.Value);
    }

    /// <summary>
    /// Gets a list of incidenthistorys.
    /// </summary>
    /// <returns>A list of incidenthistorys.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<IncidentHistoryDto>))]
    public async Task<IActionResult> GetAsync([FromQuery] QueryFilterDto queryFilter)
    {
        var result = await _incidentHistoryService.GetAsync(queryFilter);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a incidenthistory by ID.
    /// </summary>
    /// <param name="id">The ID of the incidenthistory to retrieve.</param>
    /// <returns>The requested incidenthistory.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IncidentHistoryDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var result = await _incidentHistoryService.GetByIdAsync(id);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }
}
