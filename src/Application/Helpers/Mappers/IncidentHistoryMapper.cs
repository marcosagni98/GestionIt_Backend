using AutoMapper;
using Domain.Entities;
using Application.Dtos.CRUD.IncidentHistories;
using Application.Dtos.CRUD.IncidentHistories.Request;

namespace Application.Helpers.Mappers;

/// <summary>
/// IncidentHistory mapper
/// </summary>
public class IncidentHistoryMapper : Profile
{
    /// <summary>
    /// Initializes a new instance of <see cref="IncidentHistoryMapper"/>
    /// </summary>
    public IncidentHistoryMapper()
    {
        CreateMap<IncidentHistory, IncidentHistoryDto>().ReverseMap();
        CreateMap<IncidentHistoryAddRequestDto, IncidentHistory>().ReverseMap();
        CreateMap<IncidentHistoryUpdateRequestDto, IncidentHistory>().ReverseMap();
    }
}
