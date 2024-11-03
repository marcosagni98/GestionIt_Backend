using Application.Dtos.Stats;
using Application.Interfaces.Services;
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
    /// <param name="id">The ID of the user.</param>
    /// <returns>he count and severity of active incidents</returns>
    [HttpGet("Active-incidents")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActiveIncidentsStatsResponseDto))]
    public async Task<IActionResult> GetActiveIncidentsSevirityCount()
    {
        //Todo: Get the user id from the jwt
        var result = await _statisticsService.GetActiveIncidentsSevirityCount(1);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets the average resolution time of the incidents.
    /// </summary>
    /// <returns>The count and severity of active incidents</returns>
    [HttpGet("Average-incident-resolution-time")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AverageIncidencesResolutionTimeResponseDto))]
    public async Task<IActionResult> GetAverageResolutionTime()
    {
        //Todo: Get the user id from the jwt
        var result = await _statisticsService.GetAverageResolutionTime(1);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets the user happiness statistics.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>The user happiness statistics including the happiness ratio and change ratio from the last month.</returns>
    [HttpGet("User-happiness")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserHappinessResponseDto))]
    public async Task<IActionResult> GetUserHappiness()
    {
        //Todo: Get the user id from the jwt
        var result = await _statisticsService.GetUserHappinessAsync(1);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets the summary of incidences.
    /// </summary>
    /// <returns>The total number of incidences in each type (open, closed, unassinged)</returns>
    [HttpGet("incidences-resume")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IncidencesResumeResponseDto))]
    public async Task<IActionResult> GetIncidencesResumeAsync()
    {
        var result = await _statisticsService.GetIncidencesResumeAsync();
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets the summary of incidences.
    /// </summary>
    /// <returns>The total number of incidences in each type (open, closed, unassinged)</returns>
    [HttpGet("incidences-monthly-resume")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IncidencesMonthlyResumeResponseDto))]
    public async Task<IActionResult> GetIncidencesMonthlyResumeAsync()
    {
        var result = await _statisticsService.GetIncidencesMonthlyResumeAsync();
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get number of incidents by day.
    /// </summary>
    /// <returns>The total number of incidences created each day</returns>
    [HttpGet("incidences-by-day")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IncidencesDailyResumeResponseDto))]
    public async Task<IActionResult> GetIncidencesDayResumeAsync()
    {
        var result = await _statisticsService.GetIncidencesDayResumeAsync();
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }
}
