using Application.Dtos.CRUD.WorkLogs.Request;
using Application.Dtos.CRUD.WorkLogs;
using AutoMapper;

using Domain.Entities;

/// <summary>
/// WorkLog mapper
/// </summary>
public class WorkLogMapper : Profile
{
    /// <summary>
    /// Initializes a new instance of <see cref="WorkLogMapper"/>
    /// </summary>
    public WorkLogMapper()
    {
        CreateMap<WorkLog, WorkLogDto>().ReverseMap();
        CreateMap<WorkLogAddRequestDto, WorkLog>().ReverseMap();
        CreateMap<WorkLogUpdateRequestDto, WorkLog>().ReverseMap();
    }
}