namespace Application.Dtos.NewFolder;

/// <summary>
/// DTO used to represent the request containing the title and the current description
/// for which an improved version is generated.
/// </summary>
public class ImproveDescriptionRequestDto
{
    /// <summary>
    /// The title of the incident or issue reported.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The current description provided by the user about the incident or issue. This field is optional.
    /// </summary>
    public string? CurrentDescription { get; set; }
}
