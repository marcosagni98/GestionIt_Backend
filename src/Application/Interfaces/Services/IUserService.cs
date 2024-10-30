using Application.Dtos.Auth.Requests;
using Application.Dtos.Auth.Response;
using Application.Dtos.CRUD.Users;
using Application.Dtos.CRUD.Users.Request;
using FluentResults;

namespace Application.Interfaces.Services;

/// <summary>
/// User Service Interface
/// </summary>
public interface IUserService : IBaseService<UserDto, UserAddRequestDto, UserUpdateRequestDto>, IDisposable
{

}
