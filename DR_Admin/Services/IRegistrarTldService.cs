using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing registrar-TLD relationships and pricing
/// </summary>
public interface IRegistrarTldService
{
    /// <summary>
    /// Retrieves all registrar-TLD offerings
    /// </summary>
    /// <returns>Collection of registrar-TLD DTOs</returns>
    Task<IEnumerable<RegistrarTldDto>> GetAllRegistrarTldsAsync();

    /// <summary>
    /// Retrieves all registrar-TLD offerings with pagination
    /// </summary>
    /// <param name="parameters">Pagination parameters</param>
    /// <param name="isActive">Optional filter for active status</param>
    /// <returns>Paginated result of registrar-TLD DTOs</returns>
    Task<PagedResult<RegistrarTldDto>> GetAllRegistrarTldsPagedAsync(PaginationParameters parameters, bool? isActive = null);

    /// <summary>
    /// Retrieves only available registrar-TLD offerings
    /// </summary>
    /// <returns>Collection of available registrar-TLD DTOs</returns>
    Task<IEnumerable<RegistrarTldDto>> GetAvailableRegistrarTldsAsync();

    /// <summary>
    /// Retrieves all TLD offerings for a specific registrar
    /// </summary>
    /// <param name="registrarId">The registrar ID</param>
    /// <returns>Collection of registrar-TLD DTOs</returns>
    Task<IEnumerable<RegistrarTldDto>> GetRegistrarTldsByRegistrarAsync(int registrarId);

    /// <summary>
    /// Retrieves all TLD offerings for a specific registrar with pagination
    /// </summary>
    /// <param name="registrarId">The registrar ID</param>
    /// <param name="parameters">Pagination parameters</param>
    /// <param name="isActive">Optional filter for active status</param>
    /// <returns>Paginated result of registrar-TLD DTOs</returns>
    Task<PagedResult<RegistrarTldDto>> GetRegistrarTldsByRegistrarPagedAsync(int registrarId, PaginationParameters parameters, bool? isActive = null);

    /// <summary>
    /// Retrieves all registrar offerings for a specific TLD
    /// </summary>
    /// <param name="tldId">The TLD ID</param>
    /// <returns>Collection of registrar-TLD DTOs</returns>
    Task<IEnumerable<RegistrarTldDto>> GetRegistrarTldsByTldAsync(int tldId);

    /// <summary>
    /// Retrieves a registrar-TLD offering by its ID
    /// </summary>
    /// <param name="id">The registrar-TLD ID</param>
    /// <returns>The registrar-TLD DTO, or null if not found</returns>
    Task<RegistrarTldDto?> GetRegistrarTldByIdAsync(int id);

    /// <summary>
    /// Retrieves a registrar-TLD offering by registrar and TLD IDs
    /// </summary>
    /// <param name="registrarId">The registrar ID</param>
    /// <param name="tldId">The TLD ID</param>
    /// <returns>The registrar-TLD DTO, or null if not found</returns>
    Task<RegistrarTldDto?> GetRegistrarTldByRegistrarAndTldAsync(int registrarId, int tldId);

    /// <summary>
    /// Creates a new registrar-TLD offering
    /// </summary>
    /// <param name="createDto">The creation data</param>
    /// <returns>The created registrar-TLD DTO</returns>
    Task<RegistrarTldDto> CreateRegistrarTldAsync(CreateRegistrarTldDto createDto);

    /// <summary>
    /// Updates an existing registrar-TLD offering
    /// </summary>
    /// <param name="id">The registrar-TLD ID</param>
    /// <param name="updateDto">The update data</param>
    /// <returns>The updated registrar-TLD DTO, or null if not found</returns>
    Task<RegistrarTldDto?> UpdateRegistrarTldAsync(int id, UpdateRegistrarTldDto updateDto);

    /// <summary>
    /// Deletes a registrar-TLD offering
    /// </summary>
    /// <param name="id">The registrar-TLD ID</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteRegistrarTldAsync(int id);

    /// <summary>
    /// Imports TLDs for a registrar from content data
    /// </summary>
    /// <param name="registrarId">The registrar ID</param>
    /// <param name="importDto">The import data containing TLD extensions and pricing</param>
    /// <returns>Import result with statistics</returns>
    Task<ImportRegistrarTldsResponseDto> ImportRegistrarTldsAsync(int registrarId, ImportRegistrarTldsDto importDto);

    /// <summary>
    /// Imports TLDs for a registrar from a CSV file stream
    /// </summary>
    /// <param name="registrarId">The registrar ID</param>
    /// <param name="csvStream">The CSV file stream</param>
    /// <param name="defaultRegistrationCost">Default registration cost for imported TLDs</param>
    /// <param name="defaultRegistrationPrice">Default registration price for imported TLDs</param>
    /// <param name="defaultRenewalCost">Default renewal cost for imported TLDs</param>
    /// <param name="defaultRenewalPrice">Default renewal price for imported TLDs</param>
    /// <param name="defaultTransferCost">Default transfer cost for imported TLDs</param>
    /// <param name="defaultTransferPrice">Default transfer price for imported TLDs</param>
    /// <param name="isAvailable">Whether imported TLDs should be marked as available</param>
    /// <param name="activateNewTlds">Whether to activate TLDs that don't exist in the Tlds table</param>
    /// <param name="currency">The currency for pricing</param>
    /// <returns>Import result with statistics</returns>
    Task<ImportRegistrarTldsResponseDto> ImportRegistrarTldsFromCsvAsync(
        int registrarId,
        System.IO.Stream csvStream,
        decimal? defaultRegistrationCost,
        decimal? defaultRegistrationPrice,
        decimal? defaultRenewalCost,
        decimal? defaultRenewalPrice,
        decimal? defaultTransferCost,
        decimal? defaultTransferPrice,
        bool isAvailable,
        bool activateNewTlds,
        string currency);

    /// <summary>
    /// Updates the active status of all registrar-TLD offerings
    /// </summary>
    /// <param name="registrarId">Optional registrar ID to filter by (null updates all registrars)</param>
    /// <param name="isActive">Whether to set all offerings to active or inactive</param>
    /// <returns>Result containing the number of updated records</returns>
    Task<BulkUpdateResultDto> BulkUpdateAllRegistrarTldStatusAsync(int? registrarId, bool isActive);

    /// <summary>
    /// Updates the active status of registrar-TLD offerings for specific TLD extensions
    /// </summary>
    /// <param name="registrarId">Optional registrar ID to filter by (null updates all registrars)</param>
    /// <param name="tldExtensions">Comma-separated list of TLD extensions</param>
    /// <param name="isActive">Whether to set the offerings to active or inactive</param>
    /// <returns>Result containing the number of updated records</returns>
    Task<BulkUpdateResultDto> BulkUpdateRegistrarTldStatusByTldAsync(int? registrarId, string tldExtensions, bool isActive);
}

