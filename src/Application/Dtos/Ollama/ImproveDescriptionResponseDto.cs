namespace Application.Dtos.Ollama;

/// <summary>
/// DTO used to represent the response containing the improved description.
/// </summary>
public class ImproveDescriptionResponseDto
{
    /// <summary>
    /// The improved description generated based on the provided title and current description.
    /// </summary>
    public string ImproveDescription { get; set; }
}