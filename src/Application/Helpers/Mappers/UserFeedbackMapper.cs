using Application.Dtos.CRUD.UserFeedbacks.Request;
using Application.Dtos.CRUD.UserFeedbacks;
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
        CreateMap<UserFeedback, UserFeedbackDto>().ReverseMap();
        CreateMap<UserFeedbackAddRequestDto, UserFeedback>().ReverseMap();
        CreateMap<UserFeedbackUpdateRequestDto, UserFeedback>().ReverseMap();
    }
}