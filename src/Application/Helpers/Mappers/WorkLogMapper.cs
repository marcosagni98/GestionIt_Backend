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
        CreateMap<WorkLog, WorkLogDto>()
            .ForMember(dest => dest.TechnicianName, opt => opt.MapFrom(src => src.Technician != null ? src.Technician.Name : string.Empty))
            .ReverseMap();
        
        CreateMap<WorkLogAddRequestDto, WorkLog>().ReverseMap();
        CreateMap<WorkLogUpdateRequestDto, WorkLog>().ReverseMap();
    }
}