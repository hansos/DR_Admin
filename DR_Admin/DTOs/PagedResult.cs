namespace ISPAdmin.DTOs;

/// <summary>
/// Generic wrapper for paginated results
/// </summary>
/// <typeparam name="T">The type of items in the result set</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// The current page of data
    /// </summary>
    public List<T> Data { get; set; } = new();

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Indicates if there is a previous page
    /// </summary>
    public bool HasPrevious => CurrentPage > 1;

    /// <summary>
    /// Indicates if there is a next page
    /// </summary>
    public bool HasNext => CurrentPage < TotalPages;

    /// <summary>
    /// Creates a new paginated result
    /// </summary>
    /// <param name="items">The items for the current page</param>
    /// <param name="count">Total count of items</param>
    /// <param name="pageNumber">Current page number</param>
    /// <param name="pageSize">Page size</param>
    public PagedResult(List<T> items, int count, int pageNumber, int pageSize)
    {
        Data = items;
        TotalCount = count;
        CurrentPage = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    /// <summary>
    /// Creates an empty paginated result
    /// </summary>
    public PagedResult()
    {
    }
}
