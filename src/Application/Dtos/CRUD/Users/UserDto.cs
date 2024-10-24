using Domain.Enums;

namespace Application.Dtos.CRUD.Users;

/// <summary>
/// a user
/// </summary>
public class UserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserType UserType { get; set; }
}
