using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Defines read operations for registered domain history entries.
/// </summary>
public interface IRegisteredDomainHistoryService
{
    /// <summary>
    /// Retrieves all history entries for a specific registered domain.
    /// </summary>
    /// <param name="registeredDomainId">The registered domain identifier.</param>
    /// <returns>A collection of domain history entries ordered by occurrence date descending.</returns>
    Task<IEnumerable<RegisteredDomainHistoryDto>> GetByRegisteredDomainIdAsync(int registeredDomainId);

    /// <summary>
    /// Retrieves a registered domain history entry by identifier.
    /// </summary>
    /// <param name="id">The history entry identifier.</param>
    /// <returns>The matching history entry if found; otherwise <see langword="null"/>.</returns>
    Task<RegisteredDomainHistoryDto?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves DNS change history entries across all domains with optional filters.
    /// </summary>
    /// <param name="domainName">Optional domain name search filter.</param>
    /// <param name="occurredFrom">Optional lower bound for occurrence timestamp (UTC).</param>
    /// <param name="occurredTo">Optional upper bound for occurrence timestamp (UTC).</param>
    /// <returns>A collection of DNS change history entries ordered by occurrence date descending.</returns>
    Task<IEnumerable<RegisteredDomainHistoryDto>> GetDnsChangesAsync(string? domainName, DateTime? occurredFrom, DateTime? occurredTo);
}
