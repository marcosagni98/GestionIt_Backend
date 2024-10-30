namespace Application.Dtos.Auth.Requests;

/// <summary>
/// Reset password request dto
/// </summary>
public class ResetPasswordRequestDto
{
    /// <summary>
    /// The user email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// New password
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
