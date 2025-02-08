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

namespace Application.Services
{
    /// <summary>
    /// Service for managing work log-related operations.
    /// </summary>
    public class WorkLogService : IWorkLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWorkLogRepository _workLogRepository;
        private readonly IIncidentRepository _incidentRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkLogService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="mapper">The mapper for object mapping.</param>
        public WorkLogService(IUnitOfWork unitOfWork, IMapper mapper, IWorkLogRepository workLogRepository, IIncidentRepository incidentRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _workLogRepository = workLogRepository;
            _incidentRepository = incidentRepository;
        }

        /// <inheritdoc/>
        public async Task<Result<CreatedResponseDto>> AddAsync(WorkLogAddRequestDto addRequestDto)
        {
            var workLog = _mapper.Map<WorkLog>(addRequestDto);
            await _workLogRepository.AddAsync(workLog);
            await _unitOfWork.CommitAsync();

            return Result.Ok(new CreatedResponseDto(workLog.Id));
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> DeleteAsync(long id)
        {
            var exists = await _workLogRepository.ExistsAsync(id);
            if (!exists)
            {
                return Result.Fail<SuccessResponseDto>("Work log not found.");
            }

            await _workLogRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Work log deleted successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<PaginatedList<WorkLogDto>>> GetAsync(QueryFilterDto queryFilter)
        {
            var paginatedList = await _workLogRepository.GetAsync(queryFilter);

            if (paginatedList == null || paginatedList.Items == null)
            {
                return Result.Fail<PaginatedList<WorkLogDto>>("Error retrieving work logs.");
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
                return Result.Fail<WorkLogDto>("Work log not found.");
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
                return Result.Fail<SuccessResponseDto>("Work log not found.");
            }

            _mapper.Map(updateRequestDto, workLog);
            _workLogRepository.Update(workLog);
            await _unitOfWork.CommitAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Work log updated successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<List<WorkLogDto>>> GetByIncidentIdAsync(long incidentId)
        {
            Incident? incident = await _incidentRepository.GetByIdAsync(incidentId);
            if (incident == null) return Result.Fail("Incident not found");

            return _mapper.Map<List<WorkLogDto>>(await _workLogRepository.GetByIncidentIdAsync(incidentId));
        }
    }
}
