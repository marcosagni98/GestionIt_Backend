using Domain.Enums;

namespace Application.Dtos.CRUD.Users.Request;

public class UserAddRequestDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserType UserType { get; set; }
}
