namespace Application.Dtos.Auth.Response;

/// <summary>
/// Login response
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// Initializes a new instance of <see cref="LoginResponseDto"/>
    /// </summary>
    /// <param name="token">Jwt token</param>
    /// <param name="expirationUtc">Expiration time utc</param>
    public LoginResponseDto(string token, DateTime expirationUtc)
    {
        Token = token;
        ExpirationUtc = expirationUtc;
    }

    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Expiration date
    /// </summary>
    public DateTime ExpirationUtc { get; set; } = DateTime.UtcNow;

    /*/// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;*/
}
