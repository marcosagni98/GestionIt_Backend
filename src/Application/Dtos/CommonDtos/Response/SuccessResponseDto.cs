using Microsoft.AspNetCore.Http;

namespace Application.Dtos.CommonDtos.Response;

/// <summary>
/// Response DTO for successful requests
/// </summary>
public class SuccessResponseDto
{
    /// <summary>
    /// Status code of the response
    /// </summary>
    public int StatusCode { get; set; } = StatusCodes.Status200OK;

    /// <summary>
    /// Success message of the response
    /// </summary>
    public string Message { get; set; } = "Success";
}
