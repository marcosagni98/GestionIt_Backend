namespace Application.Dtos.CRUD.Incidents.Request;

/// <summary>
/// DTO para actualizar el técnico asignado a un incidente.
/// </summary>
/// <param name="TechnicianId">ID del técnico.</param>
public record IncidentUpdateTechnicianRequestDto(
    long TechnicianId
    );
