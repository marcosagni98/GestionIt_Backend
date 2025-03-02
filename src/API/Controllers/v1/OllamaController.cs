using Application.Dtos.CRUD.Messages;
using Application.Dtos.NewFolder;
using Application.Dtos.Ollama;
using Application.Interfaces.Services;
using Domain.Dtos.CommonDtos.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1;

/// <summary>
/// Ollama controller for ollama related queries
/// </summary>
/// <param name="logger">Logger interface</param>
/// <param name="ollamaService">Ollama service </param>
[ApiController]
[Route("api/[controller]")]
public class OllamaController(ILogger<OllamaController> logger, IOllamaService ollamaService) : BaseApiController
{
    private readonly ILogger<OllamaController> _logger = logger;
    private readonly IOllamaService _ollamaService = ollamaService;

    /// <summary>
    /// Endpoint to improve the description of an issue based on the provided title and current description.
    /// This method takes a user's issue title and description, processes them, and returns an improved version of the description.
    /// </summary>
    /// <param name="request">The request containing the title and current description of the issue to be improved.</param>
    /// <returns>Returns a 200 OK response with the improved description, or a 400 Bad Request if the title is missing, or a 500 Internal Server Error if an exception occurs.</returns>
    [Authorize]
    [HttpPost("improve-description")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ImproveDescriptionResponseDto))]
    public async Task<IActionResult> ImproveIssueDescription(
        [FromBody] ImproveDescriptionRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest("El título es obligatorio");

        try
        {
            var improvedDescription = await _ollamaService.GenerateImprovedDescriptionAsync(
                request.Title,
                request.CurrentDescription
            );

            return Ok(improvedDescription);
        }
        catch (Exception ex)
        {
            // Manejo de errores
            return StatusCode(500, new
            {
                Message = "Error al generar descripción",
                Details = ex.Message
            });
        }
    }
}


