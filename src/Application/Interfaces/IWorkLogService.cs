using Application.Dtos.CRUD.WorkLogs;
using Application.Dtos.CRUD.WorkLogs.Request;


namespace Application.Interfaces;

/// <summary>
/// WorkLog Service Interface
/// </summary>
public interface IWorkLogService : IBaseService<WorkLogDto, WorkLogAddRequestDto, WorkLogUpdateRequestDto>, IDisposable
{
}
