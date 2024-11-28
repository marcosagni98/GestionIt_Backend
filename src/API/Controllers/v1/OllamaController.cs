using Application.Dtos.NewFolder;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1;

[ApiController]
[Route("api/[controller]")]
public class OllamaController : BaseApiController
{
    private readonly IOllamaService _ollamaService;

    public OllamaController(IOllamaService ollamaService)
    {
        _ollamaService = ollamaService;
    }

    [Authorize]
    [HttpPost("improve-description")]
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

            return Ok(new { ImprovedDescription = improvedDescription });
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


