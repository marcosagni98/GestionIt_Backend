using Microsoft.AspNetCore.Http;

namespace Application.Dtos.CommonDtos;

/// <summary>
/// Response DTO for successful create requests
/// </summary>
public class CreatedResponseDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreatedResponseDto"/> class.
    /// </summary>
    /// <param name="id">id of the new entity created</param>
    public CreatedResponseDto(long id) => Id = id;

    /// <summary>
    /// Id of the created entity
    /// </summary>
    public long Id { get; set; } = 0;

    /// <summary>
    /// Status code of the response
    /// </summary>
    public int StatusCode { get; set; } = StatusCodes.Status201Created;

    /// <summary>
    /// Success message of the response
    /// </summary>
    public string Message { get; set; } = "Created succesfully";
}