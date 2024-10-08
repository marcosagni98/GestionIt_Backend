using System.Text.Json.Serialization;

namespace Application.Dtos.CommonDtos.Response;

/// <summary>
/// Details of an error response
/// </summary>
public class ErrorDetails
{
    /// <summary>
    /// timestamp of the error
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(-5)]
    [JsonPropertyName("timeStamp")]
    public DateTime? TimeStamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// status code of the error
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(-4)]
    [JsonPropertyName("statusCode")]
    public int? StatusCode { get; set; }

    /// <summary>
    /// Error type
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(-3)]
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(-2)]
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Path of the error
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(-1)]
    [JsonPropertyName("path")]
    public string? Path { get; set; }
}
