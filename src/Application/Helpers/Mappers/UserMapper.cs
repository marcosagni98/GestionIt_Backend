using AutoMapper;
using Domain.Entities;
using Application.Dtos.CRUD.Users.Request;
using Application.Dtos.CRUD.Users;

namespace Application.Helpers.Mappers;

/// <summary>
/// User mapper
/// </summary>
public class UserMapper : Profile
{
    /// <summary>
    /// Initializes a new instance of <see cref="UserMapper"/>
    /// </summary>
    public UserMapper()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<UserAddRequestDto, User>().ReverseMap();
        CreateMap<UserUpdateRequestDto, User>().ReverseMap();
    }
}
