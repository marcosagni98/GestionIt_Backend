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
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Service for managing incident-related operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="IncidentService"/> class.
/// </remarks>
/// <param name="logger">Logger interface</param>
/// <param name="unitOfWork"></param>
/// <param name="mapper"></param>
/// <param name="incidentRepository"></param>
/// <param name="userRepository"></param>
/// <param name="incidentHistoryRepository"></param>
public class IncidentService(ILogger<IncidentHistoryService> logger, IUnitOfWork unitOfWork, IMapper mapper, IIncidentRepository incidentRepository, IUserRepository userRepository, IIncidentHistoryRepository incidentHistoryRepository) : IIncidentService
{
    private readonly ILogger<IncidentHistoryService> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IIncidentRepository _incidentRepository = incidentRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IIncidentHistoryRepository _incidentHistoryRepository = incidentHistoryRepository;

    /// <inheritdoc/>
    public async Task<Result<CreatedResponseDto>> AddAsync(IncidentAddRequestDto addRequestDto)
    {
        var incident = _mapper.Map<Incident>(addRequestDto); 
        await _incidentRepository.AddAsync(incident);
        await _unitOfWork.SaveAsync();

        return Result.Ok(new CreatedResponseDto (incident.Id));
    }

    /// <inheritdoc/>
    public async Task<Result<SuccessResponseDto>> DeleteAsync(long id)
    {
        var exists = await _incidentRepository.ExistsAsync(id);
        if (!exists)
        {
            string error = $"Incident with id {id} not found";
            _logger.LogError(error);
            return Result.Fail<SuccessResponseDto>(error);
        }

        await _incidentRepository.DeleteAsync(id);
        await _unitOfWork.SaveAsync();

        return Result.Ok(new SuccessResponseDto { Message = "Incident deleted successfully." });
    }

    /// <inheritdoc/>
    public async Task<Result<PaginatedList<IncidentDto>>> GetAsync(QueryFilterDto queryFilter, long userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) 
        {
            string error = $"User with id {userId} not found";
            _logger.LogError(error);
            return Result.Fail(error);
        } 

        PaginatedList<Incident> paginatedList = new([], 0);
        if(user.UserType == UserType.Admin)
        {
            paginatedList = await _incidentRepository.GetAsync(queryFilter);
        }
        else
        {
            paginatedList = await _incidentRepository.GetIncidentsOfUserAsync(queryFilter, userId);
        }

        if (paginatedList.Items == null)
        {
            string error = "Error retrieving incidents.";
            _logger.LogError(error);
            return Result.Fail<PaginatedList<IncidentDto>>(error);
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
            string error = "Error retrieving incidents.";
            _logger.LogError(error);
            return Result.Fail<PaginatedList<IncidentDto>>(error);
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
            string error = $"Incident with id {id} not found";
            _logger.LogError(error);
            return Result.Fail<IncidentDto>(error);
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
            string error = $"Incident with id {id} not found";
            _logger.LogError(error);
            return Result.Fail<SuccessResponseDto>(error;
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
            string error = $"Incident with id {id} not found";
            _logger.LogError(error);
            return Result.Fail<SuccessResponseDto>(error);
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
            string error = $"User with id {userId} not found";
            _logger.LogError(error);
            return Result.Fail(error);
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
            string error = $"Not incidents found for user {userId}";
            _logger.LogError(error);
            return Result.Fail<List<long>>(error);
        }

        return Result.Ok(incidentList.ToList());
    }

    /// <inheritdoc/>
    public async Task<Result<SuccessResponseDto>> UpdateTechnicianAsync(long id, IncidentUpdateTechnicianRequestDto incidentUpdateTechnicianRequestDto)
    {
        Incident? incident = await _incidentRepository.GetByIdAsync(id);
        if (incident == null)
        {
            string error = $"Incident with id {id} not found";
            _logger.LogError(error);
            return Result.Fail(error);
        }

        incident.TechnicianId = incidentUpdateTechnicianRequestDto.TechnicianId;
        if(incident.Status == Status.Unassigned)
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
        if (incident == null)
        {
            string error = $"Incident with id {id} not found";
            _logger.LogError(error);
            return Result.Fail(error);
        }

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
        if (user == null)
        {
            string error = $"User with id {userId} not found";
            _logger.LogError(error);
            return Result.Fail(error);
        }

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
            string error = "Error retrieving incidents.";
            _logger.LogError(error);
            return Result.Fail<PaginatedList<IncidentDto>>(error);
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
            string error = $"Incident with id {id} not found";
            _logger.LogError(error);
            return Result.Fail<SuccessResponseDto>(error);
        }

        await _incidentRepository.UpdateIncidentPriorityAsync(id, priorityRequestDto.PriorityId);
        await _unitOfWork.SaveAsync();

        return Result.Ok(new SuccessResponseDto { Message = "Incident updated successfully." });
    }
}
