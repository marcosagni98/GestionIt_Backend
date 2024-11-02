namespace Application.Dtos.CRUD.Incidents.Request;

/// <summary>
/// DTO para actualizar el técnico asignado a un incidente.
/// </summary>
/// <param name="TechnitianId">ID del técnico.</param>
public record IncidentUpdateTechnitianRequestDto(
    long TechnitianId
    );
