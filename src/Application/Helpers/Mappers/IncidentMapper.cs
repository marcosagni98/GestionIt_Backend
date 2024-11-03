using Application.Dtos.CRUD.Incidents.Request;
using Application.Dtos.CRUD.Incidents;
using AutoMapper;

using Domain.Entities;

/// <summary>
/// Incident mapper
/// </summary>
public class IncidentMapper : Profile
{
    /// <summary>
    /// Initializes a new instance of <see cref="IncidentMapper"/>
    /// </summary>
    public IncidentMapper()
    {
        CreateMap<Incident, IncidentDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : string.Empty))
            .ForMember(dest => dest.TechnicianName, opt => opt.MapFrom(src => src.Technician != null ? src.Technician.Name : string.Empty))
            .ReverseMap();
        CreateMap<IncidentAddRequestDto, Incident>().ReverseMap();
        CreateMap<IncidentUpdateRequestDto, Incident>().ReverseMap();
        CreateMap<IncidentUpdateStatusRequestDto, Incident>().ReverseMap();
    }
}