using Application.Dtos.CRUD.WorkLogs;
using Application.Dtos.CRUD.WorkLogs.Response;
using Application.Dtos.CRUD.WorkLogs.Request;


namespace Application.Interfaces;

public interface IWorkLogService : IBaseService<WorkLogDto, WorkLogResponseDto, WorkLogAddRequestDto, WorkLogUpdateRequestDto>, IDisposable
{
}
