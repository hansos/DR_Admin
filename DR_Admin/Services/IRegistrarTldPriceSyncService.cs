namespace ISPAdmin.Services;

/// <summary>
/// Synchronizes registrar TLD cost prices from registrar APIs.
/// </summary>
public interface IRegistrarTldPriceSyncService
{
    /// <summary>
    /// Synchronize prices for all active registrars that have not completed a successful sync today.
    /// </summary>
    Task<int> SyncRegistrarsMissingTodayAsync(string triggerSource, CancellationToken cancellationToken = default);
}
