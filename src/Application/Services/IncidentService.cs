﻿using Application.Dtos.CommonDtos;
using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.Incidents;
using Application.Dtos.CRUD.Incidents.Request;
using Application.Dtos.CRUD.Incidents.Response;
using Application.Interfaces;
using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Interfaces;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    /// <summary>
    /// Service for managing incident-related operations.
    /// </summary>
    public class IncidentService : IIncidentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncidentService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="mapper">The mapper for object mapping.</param>
        public IncidentService(IUnitOfWork unitOfWork, IMapper mapper)
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
        public async Task<Result<CreatedResponseDto>> AddAsync(IncidentAddRequestDto addRequestDto)
        {
            var incident = _mapper.Map<Incident>(addRequestDto); // Asumiendo que tienes un DTO de solicitud
            await _unitOfWork.IncidentRepository.AddAsync(incident);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new CreatedResponseDto { Id = incident.Id, Message = "Incident added successfully.", StatusCode = StatusCodes.Status201Created });
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> DeleteAsync(long id)
        {
            var exists = await _unitOfWork.IncidentRepository.ExistsAsync(id);
            if (!exists)
            {
                return Result.Fail<SuccessResponseDto>("Incident not found.");
            }

            await _unitOfWork.IncidentRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Incident deleted successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<PaginatedList<IncidentsDto>>> GetAsync(QueryFilterDto queryFilter)
        {
            var paginatedList = await _unitOfWork.IncidentRepository.GetAsync(queryFilter);

            if (paginatedList == null || paginatedList.Items == null)
            {
                return Result.Fail<PaginatedList<IncidentsDto>>("Error retrieving incidents.");
            }

            var incidentDtos = _mapper.Map<List<IncidentsDto>>(paginatedList.Items);

            return Result.Ok(new PaginatedList<IncidentsDto>(incidentDtos, paginatedList.TotalCount));
        }

        /// <inheritdoc/>
        public async Task<Result<IncidentResponseDto>> GetByIdAsync(long id)
        {
            var incident = await _unitOfWork.IncidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return Result.Fail<IncidentResponseDto>("Incident not found.");
            }

            var response = _mapper.Map<IncidentResponseDto>(incident);
            return Result.Ok(response);
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> UpdateAsync(long id, IncidentUpdateRequestDto updateRequestDto)
        {
            var incident = await _unitOfWork.IncidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return Result.Fail<SuccessResponseDto>("Incident not found.");
            }

            _mapper.Map(updateRequestDto, incident);
            await _unitOfWork.IncidentRepository.UpdateAsync(incident);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Incident updated successfully." });
        }
    }
}