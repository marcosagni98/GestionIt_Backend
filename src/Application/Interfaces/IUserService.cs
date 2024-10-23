using Application.Dtos.CRUD.Users;
using Application.Dtos.CRUD.Users.Request;
using Application.Dtos.CRUD.Users.Response;

namespace Application.Interfaces;

/// <summary>
/// User Service Interface
/// </summary>
public interface IUserService : IBaseService<UserDto, UserResponseDto, UserAddRequestDto, UserUpdateRequestDto>, IDisposable
{
}
