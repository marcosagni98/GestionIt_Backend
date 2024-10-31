using Application.Dtos.CommonDtos;
using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.IncidentHistories;
using Application.Dtos.CRUD.IncidentHistories.Request;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    /// <summary>
    /// Service for managing incident history-related operations.
    /// </summary>
    public class IncidentHistoryService : IIncidentHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncidentHistoryService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="mapper">The mapper for object mapping.</param>
        public IncidentHistoryService(IUnitOfWork unitOfWork, IMapper mapper)
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
        public async Task<Result<CreatedResponseDto>> AddAsync(IncidentHistoryAddRequestDto addRequestDto)
        {
            var incidentHistory = _mapper.Map<IncidentHistory>(addRequestDto);
            await _unitOfWork.IncidentHistoryRepository.AddAsync(incidentHistory);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new CreatedResponseDto (incidentHistory.Id));
        }

        /// <inheritdoc/>
        public Task<Result<SuccessResponseDto>> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<Result<PaginatedList<IncidentHistoryDto>>> GetAsync(QueryFilterDto queryFilter)
        {
            var paginatedList = await _unitOfWork.IncidentHistoryRepository.GetAsync(queryFilter);

            if (paginatedList == null || paginatedList.Items == null)
            {
                return Result.Fail<PaginatedList<IncidentHistoryDto>>("Error retrieving incident histories.");
            }

            var incidentHistoryDtos = _mapper.Map<List<IncidentHistoryDto>>(paginatedList.Items);

            return Result.Ok(new PaginatedList<IncidentHistoryDto>(incidentHistoryDtos, paginatedList.TotalCount));
        }

        /// <inheritdoc/>
        public async Task<Result<IncidentHistoryDto>> GetByIdAsync(long id)
        {
            var incidentHistory = await _unitOfWork.IncidentHistoryRepository.GetByIdAsync(id);
            if (incidentHistory == null)
            {
                return Result.Fail<IncidentHistoryDto>("Incident history not found.");
            }

            var response = _mapper.Map<IncidentHistoryDto>(incidentHistory);
            return Result.Ok(response);
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> UpdateAsync(long id, IncidentHistoryUpdateRequestDto updateRequestDto)
        {
            var incidentHistory = await _unitOfWork.IncidentHistoryRepository.GetByIdAsync(id);
            if (incidentHistory == null)
            {
                return Result.Fail<SuccessResponseDto>("Incident history not found.");
            }

            _mapper.Map(updateRequestDto, incidentHistory);
            _unitOfWork.IncidentHistoryRepository.Update(incidentHistory);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Incident history updated successfully." });
        }
    }
}
