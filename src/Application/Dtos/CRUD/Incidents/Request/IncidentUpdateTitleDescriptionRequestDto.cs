namespace Application.Dtos.CRUD.Incidents.Request;

/// <summary>
/// DTO for updating the title and description of an incident.
/// </summary>
/// <param name="Title">Title of the incident</param>
/// <param name="Description">Description of the incident</param>
public record IncidentUpdateTitleDescriptionRequestDto(
    string Title,
    string Description
);
