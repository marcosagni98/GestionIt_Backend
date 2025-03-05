using Application.Dtos.CRUD.UserFeedbacks;
using Application.Dtos.CRUD.UserFeedbacks.Request;
using AutoMapper;
using Domain.Entities;

/// <summary>
/// UserFeedback mapper
/// </summary>
public class UserFeedbackMapper : Profile
{
    /// <summary>
    /// Initializes a new instance of <see cref="UserFeedbackMapper"/>
    /// </summary>
    public UserFeedbackMapper()
    {
        CreateMap<UserFeedback, UserFeedbackDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : string.Empty))
            .ReverseMap();
        CreateMap<UserFeedbackAddRequestDto, UserFeedback>().ReverseMap();
        CreateMap<UserFeedbackUpdateRequestDto, UserFeedback>().ReverseMap();
    }
}