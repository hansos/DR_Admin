using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing registrar mail address operations
/// </summary>
public interface IRegistrarMailAddressService
{
    /// <summary>
    /// Retrieves all mail addresses for a specific customer
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <returns>A collection of all mail addresses for the customer</returns>
    Task<IEnumerable<RegistrarMailAddressDto>> GetRegistrarMailAddressesAsync(int customerId);
    
    /// <summary>
    /// Retrieves a registrar mail address by its unique identifier
    /// </summary>
    /// <param name="id">The registrar mail address's unique identifier</param>
    /// <returns>The registrar mail address if found; otherwise, null</returns>
    Task<RegistrarMailAddressDto?> GetRegistrarMailAddressByIdAsync(int id);
    
    /// <summary>
    /// Retrieves the default mail address for a specific customer
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <returns>The default registrar mail address if found; otherwise, null</returns>
    Task<RegistrarMailAddressDto?> GetDefaultMailAddressAsync(int customerId);
    
    /// <summary>
    /// Creates a new registrar mail address
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <param name="createDto">The registrar mail address data for creation</param>
    /// <returns>The newly created registrar mail address</returns>
    Task<RegistrarMailAddressDto> CreateRegistrarMailAddressAsync(int customerId, CreateRegistrarMailAddressDto createDto);
    
    /// <summary>
    /// Updates an existing registrar mail address
    /// </summary>
    /// <param name="id">The registrar mail address's unique identifier</param>
    /// <param name="updateDto">The updated registrar mail address data</param>
    /// <returns>The updated registrar mail address if found; otherwise, null</returns>
    Task<RegistrarMailAddressDto?> UpdateRegistrarMailAddressAsync(int id, UpdateRegistrarMailAddressDto updateDto);
    
    /// <summary>
    /// Deletes a registrar mail address from the database
    /// </summary>
    /// <param name="id">The registrar mail address's unique identifier</param>
    /// <returns>True if the registrar mail address was deleted; otherwise, false</returns>
    Task<bool> DeleteRegistrarMailAddressAsync(int id);
    
    /// <summary>
    /// Sets a registrar mail address as the default mail address
    /// </summary>
    /// <param name="id">The registrar mail address's unique identifier</param>
    /// <returns>The updated registrar mail address if found; otherwise, null</returns>
    Task<RegistrarMailAddressDto?> SetDefaultMailAddressAsync(int id);
}
