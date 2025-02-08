using Application.Dtos.CRUD.IncidentHistories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using FluentResults;

namespace Application.Services
{
    /// <summary>
    /// Service for managing incident history-related operations.
    /// </summary>
    public class IncidentHistoryService : IIncidentHistoryService
    {
        private readonly IMapper _mapper;
        private readonly IIncidentRepository _incidentRepository;
        private readonly IIncidentHistoryRepository _incidentHistoryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncidentHistoryService"/> class.
        /// </summary>
        /// <param name="mapper">The mapper for object mapping.</param>
        public IncidentHistoryService(IMapper mapper, IIncidentRepository incidentRepository, IIncidentHistoryRepository incidentHistoryRepository)
        {
            _mapper = mapper;
            _incidentRepository = incidentRepository;
            _incidentHistoryRepository = incidentHistoryRepository;
        }

        /// <inheritdoc/>
        public async Task<Result<List<IncidentHistoryDto>>> GetByIncidentIdAsync(long incidentId)
        {
            Incident? incident = await _incidentRepository.GetByIdAsync(incidentId);
            if (incident == null) return Result.Fail("Incident not found");

            return _mapper.Map<List<IncidentHistoryDto>>(await _incidentHistoryRepository.GetByIncidentIdAsync(incidentId));
        }

    }
}
