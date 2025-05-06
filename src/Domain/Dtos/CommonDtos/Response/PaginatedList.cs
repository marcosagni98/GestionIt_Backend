namespace Domain.Dtos.CommonDtos.Response;

/// <summary>
/// Represents a paginated list of items.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PaginatedList{T}"/> class.
/// </remarks>
/// <param name="items">The items on the current page.</param>
/// <param name="totalCount">The total count of items across all pages.</param>
public class PaginatedList<T>(List<T> items, int totalCount)
{
    /// <summary>
    /// The items on the current page.
    /// </summary>
    public List<T> Items { get; private set; } = items ?? [];

    /// <summary>
    /// The total count of items across all pages.
    /// </summary>
    public int TotalCount { get; private set; } = totalCount;
}