using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.CommonDtos;

/// <summary>
/// Response DTO for successful create requests
/// </summary>
public class CreatedResponseDto
{
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
    public string Message { get; set; } = "Success";
}