namespace ISPAdmin.Services;

using ISPAdmin.DTOs;

/// <summary>
/// Synchronizes registrar TLD cost prices from registrar APIs.
/// </summary>
public interface IRegistrarTldPriceSyncService
{
    /// <summary>
    /// Synchronize prices for all active registrars that have not completed a successful sync today.
    /// </summary>
    Task<int> SyncRegistrarsMissingTodayAsync(string triggerSource, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronize prices for active registrars connected to a specific TLD.
    /// </summary>
    /// <param name="tldId">The TLD ID.</param>
    /// <param name="triggerSource">Source identifier for audit/session logging.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of registrars synced.</returns>
    Task<int> SyncRegistrarsForTldAsync(int tldId, string triggerSource, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads registrar prices for a TLD extension without persisting to database.
    /// </summary>
    /// <param name="extension">The TLD extension.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Preview rows with registrar prices.</returns>
    Task<List<RegistrarCurrentCostByTldDto>> PreviewRegistrarCostsByExtensionAsync(string extension, CancellationToken cancellationToken = default);
}
