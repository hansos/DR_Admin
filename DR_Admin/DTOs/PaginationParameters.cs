using System.ComponentModel.DataAnnotations;

namespace ISPAdmin.DTOs;

/// <summary>
/// Parameters for pagination requests
/// </summary>
public class PaginationParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page (1-100)
    /// </summary>
    [Range(1, MaxPageSize, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}
