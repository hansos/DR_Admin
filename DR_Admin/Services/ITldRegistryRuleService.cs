using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Defines operations for TLD registry policy rules.
/// </summary>
public interface ITldRegistryRuleService
{
    /// <summary>
    /// Retrieves all TLD registry rules.
    /// </summary>
    /// <returns>Collection of TLD registry rules.</returns>
    Task<IEnumerable<TldRegistryRuleDto>> GetAllAsync();

    /// <summary>
    /// Retrieves active TLD registry rules.
    /// </summary>
    /// <returns>Collection of active TLD registry rules.</returns>
    Task<IEnumerable<TldRegistryRuleDto>> GetActiveAsync();

    /// <summary>
    /// Retrieves TLD registry rules for a specific TLD.
    /// </summary>
    /// <param name="tldId">The TLD identifier.</param>
    /// <returns>Collection of rules for the specified TLD.</returns>
    Task<IEnumerable<TldRegistryRuleDto>> GetByTldIdAsync(int tldId);

    /// <summary>
    /// Retrieves a TLD registry rule by identifier.
    /// </summary>
    /// <param name="id">The rule identifier.</param>
    /// <returns>The rule if found; otherwise null.</returns>
    Task<TldRegistryRuleDto?> GetByIdAsync(int id);

    /// <summary>
    /// Creates a TLD registry rule.
    /// </summary>
    /// <param name="createDto">The create payload.</param>
    /// <returns>The created TLD registry rule.</returns>
    Task<TldRegistryRuleDto> CreateAsync(CreateTldRegistryRuleDto createDto);

    /// <summary>
    /// Updates a TLD registry rule.
    /// </summary>
    /// <param name="id">The rule identifier.</param>
    /// <param name="updateDto">The update payload.</param>
    /// <returns>The updated rule if found; otherwise null.</returns>
    Task<TldRegistryRuleDto?> UpdateAsync(int id, UpdateTldRegistryRuleDto updateDto);

    /// <summary>
    /// Deletes a TLD registry rule.
    /// </summary>
    /// <param name="id">The rule identifier.</param>
    /// <returns>True if deleted; otherwise false.</returns>
    Task<bool> DeleteAsync(int id);
}
