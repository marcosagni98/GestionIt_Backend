using Application.Dtos.CRUD.IncidentHistories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    /// <summary>
    /// Service for managing incident history-related operations.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="IncidentHistoryService"/> class.
    /// </remarks>
    /// <param name="mapper">The mapper for object mapping.</param>
    public class IncidentHistoryService(ILogger<IncidentHistoryService> logger,  IMapper mapper, IIncidentRepository incidentRepository, IIncidentHistoryRepository incidentHistoryRepository) : IIncidentHistoryService
    {
        private readonly ILogger<IncidentHistoryService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IIncidentRepository _incidentRepository = incidentRepository;
        private readonly IIncidentHistoryRepository _incidentHistoryRepository = incidentHistoryRepository;

        /// <inheritdoc/>
        public async Task<Result<List<IncidentHistoryDto>>> GetByIncidentIdAsync(long incidentId)
        {
            Incident? incident = await _incidentRepository.GetByIdAsync(incidentId);
            if (incident == null)
            {
                string error = $"Incident with id {incidentId} not found";
                _logger.LogError(error);
                return Result.Fail(error);
            }

            return _mapper.Map<List<IncidentHistoryDto>>(await _incidentHistoryRepository.GetByIncidentIdAsync(incidentId));
        }

    }
}
