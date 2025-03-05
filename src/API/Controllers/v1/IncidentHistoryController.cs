using Application.Dtos.CRUD.IncidentHistories;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1;

/// <summary>
/// Controller for managing incidenthistory-related operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="IncidentHistoryController"/> class.
/// </remarks>
/// <param name="logger"></param>
/// <param name="incidentHistoryService">The incidenthistory service.</param>
[Produces("application/json")]
[Route("api/v1/[controller]")]
public class IncidentHistoryController(ILogger<IncidentHistoryController> logger, IIncidentHistoryService incidentHistoryService) : BaseApiController
{
    private readonly ILogger<IncidentHistoryController> _logger = logger;
    private readonly IIncidentHistoryService _incidentHistoryService = incidentHistoryService;

    /// <summary>
    /// Gets a incidenthistory by ID.
    /// </summary>
    /// <param name="incidentId">The ID of the incident of the incidents stories to retrieve.</param>
    /// <returns>The requested incidenthistory.</returns>
    [Authorize]
    [HttpGet("{incidentId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IncidentHistoryDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIncidentIdAsync(long incidentId)
    {
        var result = await _incidentHistoryService.GetByIncidentIdAsync(incidentId);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }
}
