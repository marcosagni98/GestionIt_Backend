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
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// Service for managing work log-related operations.
    /// </summary>
    public class WorkLogService : IWorkLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkLogService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="mapper">The mapper for object mapping.</param>
        public WorkLogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Dispose
        private bool disposed = false;

        /// <summary>
        /// Releases the unmanaged resources used by the service and optionally releases
        /// the managed resources if disposing is true.
        /// </summary>
        /// <param name="disposing">Indicates whether the method was called directly
        /// or from a finalizer. If true, the method has been called directly
        /// and managed resources should be disposed. If false, it was called by the
        /// runtime from inside the finalizer and only unmanaged resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _unitOfWork.Dispose();
                }
            }
            disposed = true;
        }

        /// <summary>
        /// Releases all resources used by the service.
        /// This method is called by consumers of the service when they are done
        /// using it to free resources promptly.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <inheritdoc/>
        public async Task<Result<CreatedResponseDto>> AddAsync(WorkLogAddRequestDto addRequestDto)
        {
            var workLog = _mapper.Map<WorkLog>(addRequestDto);
            await _unitOfWork.WorkLogRepository.AddAsync(workLog);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new CreatedResponseDto(workLog.Id));
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> DeleteAsync(long id)
        {
            var exists = await _unitOfWork.WorkLogRepository.ExistsAsync(id);
            if (!exists)
            {
                return Result.Fail<SuccessResponseDto>("Work log not found.");
            }

            await _unitOfWork.WorkLogRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Work log deleted successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<PaginatedList<WorkLogDto>>> GetAsync(QueryFilterDto queryFilter)
        {
            var paginatedList = await _unitOfWork.WorkLogRepository.GetAsync(queryFilter);

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
            var workLog = await _unitOfWork.WorkLogRepository.GetByIdAsync(id);
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
            var workLog = await _unitOfWork.WorkLogRepository.GetByIdAsync(id);
            if (workLog == null)
            {
                return Result.Fail<SuccessResponseDto>("Work log not found.");
            }

            _mapper.Map(updateRequestDto, workLog);
            _unitOfWork.WorkLogRepository.Update(workLog);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Work log updated successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<List<WorkLogDto>>> GetByIncidentIdAsync(long incidentId)
        {
            Incident? incident = await _unitOfWork.IncidentRepository.GetByIdAsync(incidentId);
            if (incident == null) return Result.Fail("Incident not found");

            return _mapper.Map<List<WorkLogDto>>(await _unitOfWork.WorkLogRepository.GetByIncidentIdAsync(incidentId));
        }
    }
}
