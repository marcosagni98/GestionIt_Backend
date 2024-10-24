using AutoMapper;
using Domain.Entities;
using Application.Dtos.CRUD.Users.Response;
using Application.Dtos.CRUD.Users.Request;
using Application.Dtos.CRUD.Users;

namespace Application.Helpers.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<UserAddRequestDto, User>().ReverseMap();
        CreateMap<UserUpdateRequestDto, User>().ReverseMap();
        CreateMap<UserResponseDto, User>().ReverseMap();
    }
}
