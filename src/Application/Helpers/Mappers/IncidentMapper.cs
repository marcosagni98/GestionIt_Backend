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
        CreateMap<Incident, IncidentDto>().ReverseMap();
        CreateMap<IncidentAddRequestDto, Incident>().ReverseMap();
        CreateMap<IncidentUpdateRequestDto, Incident>().ReverseMap();
        CreateMap<IncidentUpdateStatusRequestDto, Incident>().ReverseMap();
    }
}