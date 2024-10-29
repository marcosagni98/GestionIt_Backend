using Domain.Enums;

namespace Application.Dtos.CRUD.Users;

/// <summary>
/// a user
/// </summary>
public class UserDto
{
    /// <summary>
    /// Id of the user
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Name of the user
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email of the user
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User type
    /// </summary>
    public UserType UserType { get; set; } = UserType.Basic;
}
