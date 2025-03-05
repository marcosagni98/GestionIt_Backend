using Application.Dtos.CommonDtos.Response;
using log4net;
using Microsoft.AspNetCore.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace API.Helpers;

/// <summary>
/// Handles exceptions and returns appropriate response
/// </summary>
public class ExceptionHandler : IExceptionHandler
{
    private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Handles exceptions and returns appropriate response
    /// </summary>
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception ex, CancellationToken ct)
    {
        _log.Error($"{ex.Message} for more information {ex.StackTrace}");
        context.Response.StatusCode = GetStatusCode(ex);

        if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
        {
            await WriteErrorDetailsAsync(context, ex, ct);
        }
        else
        {
            await WriteErrorDetailsInternalAsync(context, ex, ct);
        }
        return true;
    }

    /// <summary>
    /// Returns the status code of the exception
    /// </summary>
    /// <param name="ex">Exception</param>
    /// <returns>Status code number</returns>
    private int GetStatusCode(Exception ex)
    {
        return ex switch
        {
            ArgumentNullException or ArgumentOutOfRangeException or ArgumentException or ValidationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            NotImplementedException => StatusCodes.Status501NotImplemented,
            _ => StatusCodes.Status500InternalServerError,
        };
    }

    /// <summary>
    /// Writes the error details to the response if is not a internal server error
    /// </summary>
    private async Task WriteErrorDetailsAsync(HttpContext context, Exception ex, CancellationToken ct)
    {
        await context.Response.WriteAsJsonAsync(new ErrorDetails
        {
            TimeStamp = DateTime.UtcNow,
            StatusCode = context.Response.StatusCode,
            Error = ex.GetType().Name,
            Message = ex.Message,
            Path = $"{context.Request.Method} {context.Request.Path}"
        }, cancellationToken: ct);
    }

    /// <summary>
    /// Writes the error details to the response if is a internal server error
    /// </summary>
    private async Task WriteErrorDetailsInternalAsync(HttpContext context, Exception ex, CancellationToken ct)
    {
        await context.Response.WriteAsJsonAsync(new ErrorDetails
        {
            TimeStamp = DateTime.UtcNow,
            StatusCode = context.Response.StatusCode,
            Error = ex.GetType().Name,
            Message = "Internal server error",
            Path = $"{context.Request.Method} {context.Request.Path}"
        }, cancellationToken: ct);
    }
}
