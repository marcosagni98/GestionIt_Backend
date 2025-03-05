using Application.Dtos.CRUD.IncidentHistories;
using AutoMapper;
using Domain.Entities;

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
        CreateMap<IncidentHistory, IncidentHistoryDto>()
            .ForMember(dest => dest.ChangeByUserName, opt => opt.MapFrom(src => src.ChangedByUser != null ? src.ChangedByUser.Name : string.Empty))
            .ReverseMap();
    }
}
