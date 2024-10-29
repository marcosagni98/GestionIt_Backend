using Domain.Enums;

namespace Application.Dtos.CRUD.Users.Request;

/// <summary>
/// User add request dto
/// </summary>
public class UserAddRequestDto
{
    /// <summary>
    /// Name of the user
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Email of the user
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Password of the user
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// User type
    /// </summary>
    public UserType UserType { get; set; } = UserType.Basic;
}
