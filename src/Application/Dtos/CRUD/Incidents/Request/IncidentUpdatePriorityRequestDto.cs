using Domain.Enums;
namespace Application.Dtos.CRUD.Incidents.Request;

/// <summary>
/// DTO to update the priority of an incident.
/// </summary>
/// <param name="Priority">The new priority level of the incident.</param>
public record IncidentUpdatePriorityRequestDto(
    Priority PriorityId
    );

