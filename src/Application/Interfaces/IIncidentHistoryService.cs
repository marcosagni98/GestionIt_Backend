using Application.Dtos.CRUD.IncidentHistories;
using Application.Dtos.CRUD.IncidentHistories.Response;
using Application.Dtos.CRUD.IncidentHistories.Request;

namespace Application.Interfaces;

public interface IIncidentHistoryService : IBaseService<IncidentHistoryDto, IncidentHistoryResponseDto, IncidentHistoryAddRequestDto, IncidentHistoryUpdateRequestDto>, IDisposable
{
}
