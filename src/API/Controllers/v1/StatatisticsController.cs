using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.IncidentHistories;
using Application.Dtos.CRUD.IncidentHistories.Request;
using Application.Dtos.Stats;
using Application.Interfaces.Services;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1;

/// <summary>
/// Controller for managing statistics-related operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="StatisticsController"/> class.
/// </remarks>
/// <param name="statisticsService">The statistics service.</param>
[Produces("application/json")]
[Route("api/v1/[controller]")]
public class StatisticsController(IStatisticsService statisticsService) : BaseApiController
{
    private readonly IStatisticsService _statisticsService = statisticsService;



    /// <summary>
    /// Gets the number of incidents and their sevirity.
    /// </summary>
    /// <returns>he count and severity of active incidents</returns>
    [HttpGet("Active-incidents")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActiveIncidentsStatsResponseDto))]
    public async Task<IActionResult> GetActiveIncidentsSevirityCount()
    {
        var result = await _statisticsService.GetActiveIncidentsSevirityCount();
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets the average resolution time of the incidents.
    /// </summary>
    /// <returns>he count and severity of active incidents</returns>
    [HttpGet("Average-incident-resolution-time")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AverageIncidencesResolutionTimeResponseDto))]
    public async Task<IActionResult> GetAverageResolutionTime()
    {
        var result = await _statisticsService.GetAverageResolutionTime();
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpGet("User-happiness")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserHappinessResponseDto))]
    public async Task<IActionResult> GetUserHappiness()
    {
        var result = await _statisticsService.GetUserHappinessAsync();
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }
}
