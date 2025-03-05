using Application.Dtos.CommonDtos;
using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.WorkLogs;
using Application.Dtos.CRUD.WorkLogs.Request;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
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
    public class WorkLogService(ILogger<WorkLogService> logger, IUnitOfWork unitOfWork, IMapper mapper, IWorkLogRepository workLogRepository, IIncidentRepository incidentRepository) : IWorkLogService
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
                string error = $"Work log with id {id} not found";
                _logger.LogError(error);
                return Result.Fail<SuccessResponseDto>(error);
            }

            await _workLogRepository.DeleteAsync(id);
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
                string error = $"Work log with id {id} not found";
                _logger.LogError(error);
                return Result.Fail<WorkLogDto>(error);
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
                string error = $"Work log with id {id} not found";
                _logger.LogError(error);
                return Result.Fail<SuccessResponseDto>(error);
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
                string error = $"Incident with id {incidentId} not found";
                _logger.LogError(error);
                return Result.Fail<List<WorkLogDto>>(error);
            }

            return _mapper.Map<List<WorkLogDto>>(await _workLogRepository.GetByIncidentIdAsync(incidentId));
        }
    }
}
