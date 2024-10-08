using System.Collections.Generic;

namespace Domain.Dtos.CommonDtos.Response;

/// <summary>
/// Represents a paginated list of items.
/// </summary>
public class PaginatedList<T>
{
    /// <summary>
    /// The items on the current page.
    /// </summary>
    public List<T> Items { get; private set; }

    /// <summary>
    /// The total count of items across all pages.
    /// </summary>
    public int TotalCount { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedList{T}"/> class.
    /// </summary>
    /// <param name="items">The items on the current page.</param>
    /// <param name="totalCount">The total count of items across all pages.</param>
    public PaginatedList(List<T> items, int totalCount)
    {
        Items = items ?? new List<T>();
        TotalCount = totalCount;
    }
}