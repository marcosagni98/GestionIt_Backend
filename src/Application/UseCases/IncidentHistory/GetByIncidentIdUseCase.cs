using Application.Dtos.CRUD.IncidentHistories;
using Application.Interfaces.UseCases.IncidentHistory;
using Application.Validators;
using AutoMapper;
using FluentValidation;

namespace Application.UseCases.IncidentHistory;

/// <summary>
/// Use case for retrieving the history of an incident by its ID.
/// </summary>
/// <param name="logger">The logger for logging information and errors.</param>
/// <param name="mapper">The mapper for converting entities to DTOs.</param>
/// <param name="incidentRepository">The repository for accessing incident data.</param>
/// <param name="incidentHistoryRepository">The repository for accessing incident history data.</param>
public sealed class GetByIncidentIdUseCase(
    ILogger<GetByIncidentIdUseCase> logger,
    IMapper mapper,
    IIncidentRepository incidentRepository,
    IIncidentHistoryRepository incidentHistoryRepository
    ) : IGetByIncidentIdUseCase
{
    private readonly ILogger<GetByIncidentIdUseCase> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly IIncidentRepository _incidentRepository = incidentRepository;
    private readonly IIncidentHistoryRepository _incidentHistoryRepository = incidentHistoryRepository;
    private readonly IValidator<long> _validator = new GetByIncidentIdValidator();

    /// <inheritdoc/>
    public async Task<Result<List<IncidentHistoryDto>>> GetByIncidentIdAsync(long incidentId)
    {
        // Validate the incidentId
        var validationResult = await _validator.ValidateAsync(incidentId);
        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogError("Validation failed: {Errors}", errorMessage);
            return Result.Fail<List<IncidentHistoryDto>>(errorMessage);
        }

        // Check if the incident exists
        var incident = await _incidentRepository.GetByIdAsync(incidentId);
        if (incident == null)
        {
            _logger.LogError("Incident with id {IncidentId} not found", incidentId);
            return Result.Fail<List<IncidentHistoryDto>>("Incident not found");
        }

        // Retrieve and map the incident history records
        var incidentHistories = await _incidentHistoryRepository.GetByIncidentIdAsync(incidentId);
        return Result.Ok(_mapper.Map<List<IncidentHistoryDto>>(incidentHistories));
    }
}
