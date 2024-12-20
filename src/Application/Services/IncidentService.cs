﻿using Application.Dtos.CommonDtos;
using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.Incidents;
using Application.Dtos.CRUD.Incidents.Request;
using Application.Dtos.CRUD.Users;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
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
            var incident = _mapper.Map<Incident>(addRequestDto); 
            await _unitOfWork.IncidentRepository.AddAsync(incident);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new CreatedResponseDto (incident.Id));
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
        public async Task<Result<PaginatedList<IncidentDto>>> GetAsync(QueryFilterDto queryFilter, long userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null) return Result.Fail("User not found");

            PaginatedList<Incident> paginatedList = new([], 0);
            if(user.UserType == UserType.Admin)
            {
                paginatedList = await _unitOfWork.IncidentRepository.GetAsync(queryFilter);
            }
            else
            {
                paginatedList = await _unitOfWork.IncidentRepository.GetIncidentsOfUserAsync(queryFilter, userId);
            }

            if (paginatedList.Items == null)
            {
                return Result.Fail<PaginatedList<IncidentDto>>("Error retrieving incidents.");
            }

            var incidentDtos = _mapper.Map<List<IncidentDto>>(paginatedList.Items);

            return Result.Ok(new PaginatedList<IncidentDto>(incidentDtos, paginatedList.TotalCount));
        }

        /// <inheritdoc/>
        public Task<Result<PaginatedList<IncidentDto>>> GetAsync(QueryFilterDto queryFilter)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<Result<PaginatedList<IncidentDto>>> GetHistoricAsync(QueryFilterDto queryFilter)
        {
            var paginatedList = await _unitOfWork.IncidentRepository.GetHistoricAsync(queryFilter);
            if (paginatedList == null || paginatedList.Items == null)
            {
                return Result.Fail<PaginatedList<IncidentDto>>("Error retrieving incidnets.");
            }
            var incidetnsDtos = _mapper.Map<List<IncidentDto>>(paginatedList.Items);
            return Result.Ok(new PaginatedList<IncidentDto>(incidetnsDtos, paginatedList.TotalCount));
        }

        /// <inheritdoc/>
        public async Task<Result<IncidentDto>> GetByIdAsync(long id)
        {
            var incident = await _unitOfWork.IncidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return Result.Fail<IncidentDto>("Incident not found.");
            }

            var response = _mapper.Map<IncidentDto>(incident);
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
            _unitOfWork.IncidentRepository.Update(incident);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Incident updated successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> UpdateStatusAsync(long id, IncidentUpdateStatusRequestDto statusRequest)
        {
            var incident = await _unitOfWork.IncidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return Result.Fail<SuccessResponseDto>("Incident not found.");
            }

            IncidentHistory incidentHistory = new IncidentHistory
            {
                IncidentId = id,
                Status = statusRequest.StatusId,
                ChangedBy = statusRequest.ChangedBy,
            };
            if (statusRequest.ResolutionDetails != null)
            {
                incidentHistory.ResolutionDetails = statusRequest.ResolutionDetails;
            }

            await _unitOfWork.IncidentRepository.UpdateIncidentStatusAsync(id, statusRequest.StatusId);
            await _unitOfWork.IncidentHistoryRepository.AddAsync(incidentHistory);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Incident updated successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<List<long>>> GetIncidentIdsByUserIdAsync(long userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            List<long>? incidentList;
            if (user == null)
            {
                return Result.Fail("User not found");
            }
            else if (user.UserType == UserType.Admin)
            {
                incidentList = await _unitOfWork.IncidentRepository.GetIdsAsync();
            }
            else
            {
                incidentList = await _unitOfWork.IncidentRepository.GetIdsByUserIdAsync(userId);
            }
            
            if (incidentList == null || incidentList.Count == 0)
            {
                return Result.Fail<List<long>>($"Not incidents found for user {userId}");
            }

            return Result.Ok(incidentList.ToList());
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> UpdateTechnicianAsync(long id, IncidentUpdateTechnicianRequestDto incidentUpdateTechnicianRequestDto)
        {
            Incident? incident = await _unitOfWork.IncidentRepository.GetByIdAsync(id);
            if (incident == null) return Result.Fail("Incident not found");

            incident.TechnicianId = incidentUpdateTechnicianRequestDto.TechnicianId;
            if(incident.Status == Status.Unassigned)
            {
                incident.Status = Status.Pending;
            }

            _unitOfWork.IncidentRepository.Update(incident);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Ok(new SuccessResponseDto { Message = "Incident updated successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> UpdateTitleAndDescription(long id, IncidentUpdateTitleDescriptionRequestDto updateTitleDescriptionRequestDto)
        {
            Incident? incident = await _unitOfWork.IncidentRepository.GetByIdAsync(id);
            if (incident == null) return Result.Fail("Incident not found");

            UpdateValuesOfIncident(updateTitleDescriptionRequestDto, incident);

            _unitOfWork.IncidentRepository.Update(incident);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Incident updated successfully." });
        }
        #region UpdateTitleAndDescription private methods
        /// <summary>
        /// Updates the title and description of an incident.
        /// </summary>
        /// <param name="updateTitleDescriptionRequestDto">The new values of the incident</param>
        /// <param name="incident">The incident</param>
        private static void UpdateValuesOfIncident(IncidentUpdateTitleDescriptionRequestDto updateTitleDescriptionRequestDto, Incident incident)
        {
            if (!string.IsNullOrEmpty(updateTitleDescriptionRequestDto.Title))
            {
                incident.Title = updateTitleDescriptionRequestDto.Title;
            }
            if (!string.IsNullOrEmpty(updateTitleDescriptionRequestDto.Description))
            {
                incident.Description = updateTitleDescriptionRequestDto.Description;
            }
        }

        #endregion

        /// <inheritdoc/>
        public async Task<Result<PaginatedList<IncidentDto>>> GetByPriorityAsync(QueryFilterDto queryFilter, Priority priorityId, long userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null) return Result.Fail("User not found");

            PaginatedList<Incident> paginatedList = new([], 0);
            if (user.UserType == UserType.Admin)
            {
                paginatedList = await _unitOfWork.IncidentRepository.GetByPriorityAsync(queryFilter, priorityId);
            }
            else
            {
                paginatedList = await _unitOfWork.IncidentRepository.GetIncidentsByPriorityUserAsync(queryFilter, priorityId, userId);
            }

            if (paginatedList.Items == null)
            {
                return Result.Fail<PaginatedList<IncidentDto>>("Error retrieving incidents.");
            }

            var incidentDtos = _mapper.Map<List<IncidentDto>>(paginatedList.Items);

            return Result.Ok(new PaginatedList<IncidentDto>(incidentDtos, paginatedList.TotalCount));
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> UpdatePriorityAsync(long id, IncidentUpdatePriorityRequestDto priorityRequestDto)
        {
            var incident = await _unitOfWork.IncidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return Result.Fail<SuccessResponseDto>("Incident not found.");
            }

            await _unitOfWork.IncidentRepository.UpdateIncidentPriorityAsync(id, priorityRequestDto.PriorityId);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Incident updated successfully." });
        }
    }
}
