using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing address type operations
/// </summary>
public interface IAddressTypeService
{
    /// <summary>
    /// Retrieves all address types from the database
    /// </summary>
    /// <returns>A collection of all address types</returns>
    Task<IEnumerable<AddressTypeDto>> GetAllAddressTypesAsync();
    
    /// <summary>
    /// Retrieves a paginated list of address types
    /// </summary>
    /// <param name="parameters">Pagination parameters including page number and page size</param>
    /// <returns>A paged result containing address types and pagination metadata</returns>
    Task<PagedResult<AddressTypeDto>> GetAllAddressTypesPagedAsync(PaginationParameters parameters);
    
    /// <summary>
    /// Retrieves an address type by its unique identifier
    /// </summary>
    /// <param name="id">The address type's unique identifier</param>
    /// <returns>The address type if found; otherwise, null</returns>
    Task<AddressTypeDto?> GetAddressTypeByIdAsync(int id);
    
    /// <summary>
    /// Retrieves an address type by its code
    /// </summary>
    /// <param name="code">The address type code to search for</param>
    /// <returns>The address type if found; otherwise, null</returns>
    Task<AddressTypeDto?> GetAddressTypeByCodeAsync(string code);
    
    /// <summary>
    /// Creates a new address type
    /// </summary>
    /// <param name="createDto">The address type data for creation</param>
    /// <returns>The newly created address type</returns>
    Task<AddressTypeDto> CreateAddressTypeAsync(CreateAddressTypeDto createDto);
    
    /// <summary>
    /// Updates an existing address type
    /// </summary>
    /// <param name="id">The address type's unique identifier</param>
    /// <param name="updateDto">The updated address type data</param>
    /// <returns>The updated address type if found; otherwise, null</returns>
    Task<AddressTypeDto?> UpdateAddressTypeAsync(int id, UpdateAddressTypeDto updateDto);
    
    /// <summary>
    /// Deletes an address type from the database
    /// </summary>
    /// <param name="id">The address type's unique identifier</param>
    /// <returns>True if the address type was deleted; otherwise, false</returns>
    Task<bool> DeleteAddressTypeAsync(int id);
}
