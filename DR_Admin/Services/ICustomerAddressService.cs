using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing customer address operations
/// </summary>
public interface ICustomerAddressService
{
    /// <summary>
    /// Retrieves all addresses for a specific customer
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <returns>A collection of all addresses for the customer</returns>
    Task<IEnumerable<CustomerAddressDto>> GetCustomerAddressesAsync(int customerId);
    
    /// <summary>
    /// Retrieves a customer address by its unique identifier
    /// </summary>
    /// <param name="id">The customer address's unique identifier</param>
    /// <returns>The customer address if found; otherwise, null</returns>
    Task<CustomerAddressDto?> GetCustomerAddressByIdAsync(int id);
    
    /// <summary>
    /// Retrieves the primary address for a specific customer
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <returns>The primary customer address if found; otherwise, null</returns>
    Task<CustomerAddressDto?> GetPrimaryAddressAsync(int customerId);
    
    /// <summary>
    /// Creates a new customer address
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <param name="createDto">The customer address data for creation</param>
    /// <returns>The newly created customer address</returns>
    Task<CustomerAddressDto> CreateCustomerAddressAsync(int customerId, CreateCustomerAddressDto createDto);
    
    /// <summary>
    /// Updates an existing customer address
    /// </summary>
    /// <param name="id">The customer address's unique identifier</param>
    /// <param name="updateDto">The updated customer address data</param>
    /// <returns>The updated customer address if found; otherwise, null</returns>
    Task<CustomerAddressDto?> UpdateCustomerAddressAsync(int id, UpdateCustomerAddressDto updateDto);
    
    /// <summary>
    /// Deletes a customer address from the database
    /// </summary>
    /// <param name="id">The customer address's unique identifier</param>
    /// <returns>True if the customer address was deleted; otherwise, false</returns>
    Task<bool> DeleteCustomerAddressAsync(int id);
    
    /// <summary>
    /// Sets a customer address as the primary address
    /// </summary>
    /// <param name="id">The customer address's unique identifier</param>
    /// <returns>The updated customer address if found; otherwise, null</returns>
    Task<CustomerAddressDto?> SetPrimaryAddressAsync(int id);
}
