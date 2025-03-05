using Application.Dtos.CommonDtos;
using Application.Dtos.CommonDtos.Response;
using Application.Dtos.CRUD.Incidents;
using Application.Dtos.CRUD.Incidents.Request;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Dtos.CommonDtos.Request;
using Domain.Dtos.CommonDtos.Response;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using FluentResults;

namespace Application.Services
{
    /// <summary>
    /// Service for managing incident-related operations.
    /// </summary>
    public class IncidentService : IIncidentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IIncidentRepository _incidentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IIncidentHistoryRepository _incidentHistoryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncidentService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="mapper">The mapper for object mapping.</param>
        public IncidentService(IUnitOfWork unitOfWork, IMapper mapper, IIncidentRepository incidentRepository, IUserRepository userRepository, IIncidentHistoryRepository incidentHistoryRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _incidentRepository = incidentRepository;
            _userRepository = userRepository;
            _incidentHistoryRepository = incidentHistoryRepository;
        }

        /// <inheritdoc/>
        public async Task<Result<CreatedResponseDto>> AddAsync(IncidentAddRequestDto addRequestDto)
        {
            var incident = _mapper.Map<Incident>(addRequestDto);
            await _incidentRepository.AddAsync(incident);
            await _unitOfWork.SaveAsync();

            return Result.Ok(new CreatedResponseDto(incident.Id));
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> DeleteAsync(long id)
        {
            var exists = await _incidentRepository.ExistsAsync(id);
            if (!exists)
            {
                return Result.Fail<SuccessResponseDto>("Incident not found.");
            }

            await _incidentRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Incident deleted successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<PaginatedList<IncidentDto>>> GetAsync(QueryFilterDto queryFilter, long userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return Result.Fail("User not found");

            PaginatedList<Incident> paginatedList = new([], 0);
            if (user.UserType == UserType.Admin)
            {
                paginatedList = await _incidentRepository.GetAsync(queryFilter);
            }
            else
            {
                paginatedList = await _incidentRepository.GetIncidentsOfUserAsync(queryFilter, userId);
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
            var paginatedList = await _incidentRepository.GetHistoricAsync(queryFilter);
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
            var incident = await _incidentRepository.GetByIdAsync(id);
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
            var incident = await _incidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return Result.Fail<SuccessResponseDto>("Incident not found.");
            }

            _mapper.Map(updateRequestDto, incident);
            _incidentRepository.Update(incident);
            await _unitOfWork.SaveAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Incident updated successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> UpdateStatusAsync(long id, IncidentUpdateStatusRequestDto statusRequest)
        {
            var incident = await _incidentRepository.GetByIdAsync(id);
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

            await _incidentRepository.UpdateIncidentStatusAsync(id, statusRequest.StatusId);
            await _incidentHistoryRepository.AddAsync(incidentHistory);
            await _unitOfWork.SaveAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Incident updated successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<List<long>>> GetIncidentIdsByUserIdAsync(long userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            List<long>? incidentList;
            if (user == null)
            {
                return Result.Fail("User not found");
            }
            else if (user.UserType == UserType.Admin)
            {
                incidentList = await _incidentRepository.GetIdsAsync();
            }
            else
            {
                incidentList = await _incidentRepository.GetIdsByUserIdAsync(userId);
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
            Incident? incident = await _incidentRepository.GetByIdAsync(id);
            if (incident == null) return Result.Fail("Incident not found");

            incident.TechnicianId = incidentUpdateTechnicianRequestDto.TechnicianId;
            if (incident.Status == Status.Unassigned)
            {
                incident.Status = Status.Pending;
            }

            _incidentRepository.Update(incident);
            await _unitOfWork.SaveAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Incident updated successfully." });
        }

        /// <inheritdoc/>
        public async Task<Result<SuccessResponseDto>> UpdateTitleAndDescription(long id, IncidentUpdateTitleDescriptionRequestDto updateTitleDescriptionRequestDto)
        {
            Incident? incident = await _incidentRepository.GetByIdAsync(id);
            if (incident == null) return Result.Fail("Incident not found");

            UpdateValuesOfIncident(updateTitleDescriptionRequestDto, incident);

            _incidentRepository.Update(incident);
            await _unitOfWork.SaveAsync();

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
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return Result.Fail("User not found");

            PaginatedList<Incident> paginatedList = new([], 0);
            if (user.UserType == UserType.Admin)
            {
                paginatedList = await _incidentRepository.GetByPriorityAsync(queryFilter, priorityId);
            }
            else
            {
                paginatedList = await _incidentRepository.GetIncidentsByPriorityUserAsync(queryFilter, priorityId, userId);
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
            var incident = await _incidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return Result.Fail<SuccessResponseDto>("Incident not found.");
            }

            await _incidentRepository.UpdateIncidentPriorityAsync(id, priorityRequestDto.PriorityId);
            await _unitOfWork.SaveAsync();

            return Result.Ok(new SuccessResponseDto { Message = "Incident updated successfully." });
        }
    }
}
