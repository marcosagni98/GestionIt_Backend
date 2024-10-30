using Application.Dtos.CRUD.IncidentHistories;
using Application.Dtos.CRUD.IncidentHistories.Request;

namespace Application.Interfaces.Services;

/// <summary>
/// IncidentHistory Service Interface
/// </summary>
public interface IIncidentHistoryService : IBaseService<IncidentHistoryDto, IncidentHistoryAddRequestDto, IncidentHistoryUpdateRequestDto>, IDisposable
{
}
