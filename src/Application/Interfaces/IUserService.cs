using Application.Dtos.CRUD.Users;
using Application.Dtos.CRUD.Users.Request;

namespace Application.Interfaces;

/// <summary>
/// User Service Interface
/// </summary>
public interface IUserService : IBaseService<UserDto, UserAddRequestDto, UserUpdateRequestDto>, IDisposable
{
}
