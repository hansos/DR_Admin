using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing login history entries.
/// </summary>
public interface ILoginHistoryService
{
    /// <summary>
    /// Retrieves all login history entries.
    /// </summary>
    /// <returns>List of login history entries.</returns>
    Task<IEnumerable<LoginHistoryDto>> GetAllLoginHistoriesAsync();

    /// <summary>
    /// Retrieves paginated login history entries.
    /// </summary>
    /// <param name="parameters">Pagination parameters.</param>
    /// <param name="userId">Optional filter by user identifier.</param>
    /// <param name="isSuccessful">Optional filter by success flag.</param>
    /// <param name="from">Optional filter start timestamp (inclusive, UTC).</param>
    /// <param name="to">Optional filter end timestamp (inclusive, UTC).</param>
    /// <returns>Paginated result containing login history entries.</returns>
    Task<PagedResult<LoginHistoryDto>> GetLoginHistoriesPagedAsync(
        PaginationParameters parameters,
        int? userId = null,
        bool? isSuccessful = null,
        DateTime? from = null,
        DateTime? to = null);

    /// <summary>
    /// Retrieves a specific login history entry by identifier.
    /// </summary>
    /// <param name="id">Login history identifier.</param>
    /// <returns>The login history entry, or null if not found.</returns>
    Task<LoginHistoryDto?> GetLoginHistoryByIdAsync(int id);

    /// <summary>
    /// Creates a new login history entry.
    /// </summary>
    /// <param name="dto">Login history creation data.</param>
    /// <returns>The created login history entry.</returns>
    Task<LoginHistoryDto> CreateLoginHistoryAsync(CreateLoginHistoryDto dto);
}
