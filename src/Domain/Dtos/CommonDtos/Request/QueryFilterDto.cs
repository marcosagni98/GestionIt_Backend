namespace Domain.Dtos.CommonDtos.Request;

/// <summary>
/// Parameters for filtering queries (pagination, search, ordering)
/// </summary>
public class QueryFilterDto
{
    /// <summary>
    /// Pagination: Page number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Pagination: Page size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Searching: Search term
    /// </summary>
    public string? Search { get; set; } = null!;

    /// <summary>
    /// Ordering: Paramter to order by
    /// </summary>
    public string? OrderBy { get; set; } = null!;

    /// <summary>
    /// Ordering: Order direction (asc/desc)
    /// </summary>
    public string? OrderDirection { get; set; } = null!;
}
