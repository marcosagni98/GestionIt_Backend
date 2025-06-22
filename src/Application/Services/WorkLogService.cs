using Application.Dtos.CRUD.WorkLogs;
using Application.Dtos.CRUD.WorkLogs.Request;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;

namespace Application.Services;

/// <summary>
/// Service for managing work log-related operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WorkLogService"/> class.
/// </remarks>
/// <param name="logger">Logger interface</param>
/// <param name="unitOfWork">The unit of work for database operations.</param>
/// <param name="mapper">The mapper for object mapping.</param>
/// <param name="workLogRepository">WorkLog repository</param>
/// <param name="incidentRepository">Incident repository</param>
public sealed class WorkLogService(ILogger<WorkLogService> logger, IUnitOfWork unitOfWork, IMapper mapper, IWorkLogRepository workLogRepository, IIncidentRepository incidentRepository) : IWorkLogService
{
    private readonly ILogger<WorkLogService> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IWorkLogRepository _workLogRepository = workLogRepository;
    private readonly IIncidentRepository _incidentRepository = incidentRepository;

    /// <inheritdoc/>
    public async Task<Result<CreatedResponseDto>> AddAsync(WorkLogAddRequestDto addRequestDto)
    {
        var workLog = _mapper.Map<WorkLog>(addRequestDto);
        await _workLogRepository.AddAsync(workLog);
        await _unitOfWork.SaveAsync();

        return Result.Ok(new CreatedResponseDto(workLog.Id));
    }

    /// <inheritdoc/>
    public async Task<Result<SuccessResponseDto>> DeleteAsync(long id)
    {
        var exists = await _workLogRepository.ExistsAsync(id);
        if (!exists)
        {
            _logger.LogError("Work log with id {WorkLogId} not found", id);
            return Result.Fail<SuccessResponseDto>($"Worklog not found");
        }

        await _workLogRepository.SoftDelete(id);
        await _unitOfWork.SaveAsync();

        return Result.Ok(new SuccessResponseDto { Message = "Work log deleted successfully." });
    }

    /// <inheritdoc/>
    public async Task<Result<PaginatedList<WorkLogDto>>> GetAsync(QueryFilterDto queryFilter)
    {
        var paginatedList = await _workLogRepository.GetAsync(queryFilter);

        if (paginatedList == null || paginatedList.Items == null)
        {
            string error = "Error retrieving work logs.";
            _logger.LogError(error);
            return Result.Fail<PaginatedList<WorkLogDto>>(error);
        }

        var workLogDtos = _mapper.Map<List<WorkLogDto>>(paginatedList.Items);

        return Result.Ok(new PaginatedList<WorkLogDto>(workLogDtos, paginatedList.TotalCount));
    }

    /// <inheritdoc/>
    public async Task<Result<WorkLogDto>> GetByIdAsync(long id)
    {
        var workLog = await _workLogRepository.GetByIdAsync(id);
        if (workLog == null)
        {
            _logger.LogError("Work log with id {WorkLogId} not found", id);
            return Result.Fail<WorkLogDto>($"Worklog not found");
        }

        var response = _mapper.Map<WorkLogDto>(workLog);
        return Result.Ok(response);
    }

    /// <inheritdoc/>
    public async Task<Result<SuccessResponseDto>> UpdateAsync(long id, WorkLogUpdateRequestDto updateRequestDto)
    {
        var workLog = await _workLogRepository.GetByIdAsync(id);
        if (workLog == null)
        {
            _logger.LogError("Work log with id {WorkLogId} not found", id);
            return Result.Fail<SuccessResponseDto>($"Worklog  not found");
        }

        _mapper.Map(updateRequestDto, workLog);
        _workLogRepository.Update(workLog);
        await _unitOfWork.SaveAsync();

        return Result.Ok(new SuccessResponseDto { Message = "Work log updated successfully." });
    }

    /// <inheritdoc/>
    public async Task<Result<List<WorkLogDto>>> GetByIncidentIdAsync(long incidentId)
    {
        Incident? incident = await _incidentRepository.GetByIdAsync(incidentId);
        if (incident == null)
        {
            _logger.LogError("Work log with id {IncidentId} not found", incidentId);
            return Result.Fail<List<WorkLogDto>>($"Worklog not found");
        }

        return _mapper.Map<List<WorkLogDto>>(await _workLogRepository.GetByIncidentIdAsync(incidentId));
    }
}
