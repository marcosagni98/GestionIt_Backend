namespace Application.Dtos.CRUD.Users;

/// <summary>
/// a user
/// </summary>
public class UserDto
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
    public string? UserType { get; set; }
}
