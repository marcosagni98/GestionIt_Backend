using Application.Dtos.Ollama;

namespace Application.Interfaces.Services;

public interface IOllamaService
{

    public Task<ImproveDescriptionResponseDto> GenerateImprovedDescriptionAsync(string title, string? currentDescription);
}
