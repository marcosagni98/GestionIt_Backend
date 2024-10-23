using Application.Dtos.CRUD.Incidents;
using Application.Dtos.CRUD.Incidents.Response;
using Application.Dtos.CRUD.Incidents.Request;

namespace Application.Interfaces;

public interface IIncidentService : IBaseService<IncidentsDto, IncidentResponseDto, IncidentAddRequestDto, IncidentUpdateRequestDto>, IDisposable
{

}
