using Application.Dtos.CRUD.Messages.Request;
using Application.Dtos.CRUD.Messages;
using AutoMapper;

using Domain.Entities;

/// <summary>
/// Message mapper
/// </summary>
public class MessageMapper : Profile
{
    /// <summary>
    /// Initializes a new instance of <see cref="MessageMapper"/>
    /// </summary>
    public MessageMapper()
    {
        CreateMap<Message, MessageDto>().ReverseMap();
        CreateMap<MessageAddRequestDto, Message>().ReverseMap();
        CreateMap<MessageUpdateRequestDto, Message>().ReverseMap();
    }
}