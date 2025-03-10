﻿using System.Text.Json.Serialization;

namespace Application.Dtos.CRUD.WorkLogs.Request;

/// <summary>
/// work log add request dto
/// </summary>
public class WorkLogAddRequestDto
{
    /// <summary>
    /// The id of the incident
    /// </summary>
    public long IncidentId { get; set; }

    /// <summary>
    /// Id of the technician
    /// </summary>
    [JsonIgnore]
    public long TechnicianId { get; set; }

    /// <summary>
    /// The time worked on the incident
    /// </summary>
    public decimal MinWorked { get; set; }
}
