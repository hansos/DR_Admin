using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing customer operations
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Retrieves all customers from the database
    /// </summary>
    /// <returns>A collection of all customers</returns>
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    
    /// <summary>
    /// Retrieves a paginated list of customers
    /// </summary>
    /// <param name="parameters">Pagination parameters including page number and page size</param>
    /// <returns>A paged result containing customers and pagination metadata</returns>
    Task<PagedResult<CustomerDto>> GetAllCustomersPagedAsync(PaginationParameters parameters);
    
    /// <summary>
    /// Retrieves a customer by their unique identifier
    /// </summary>
    /// <param name="id">The customer's unique identifier</param>
    /// <returns>The customer if found; otherwise, null</returns>
    Task<CustomerDto?> GetCustomerByIdAsync(int id);

    /// <summary>
    /// Retrieves a customer by their email address
    /// </summary>
    /// <param name="email">The email address to search for</param>
    /// <param name="includeContacts"></param>
    /// <returns>The customer if found; otherwise, null</returns>
    Task<CustomerDto?> GetCustomerByEmailAsync(string email, bool? includeContacts = false);
    
    /// <summary>
    /// Checks if an email address exists in the customer database
    /// </summary>
    /// <param name="email">The email address to check</param>
    /// <returns>True if the email exists; otherwise, false</returns>
    Task<bool> CheckEmailExistsAsync(string email);
    
    /// <summary>
    /// Creates a new customer
    /// </summary>
    /// <param name="createDto">The customer data for creation</param>
    /// <returns>The newly created customer</returns>
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createDto);
    
    /// <summary>
    /// Updates an existing customer
    /// </summary>
    /// <param name="id">The customer's unique identifier</param>
    /// <param name="updateDto">The updated customer data</param>
    /// <returns>The updated customer if found; otherwise, null</returns>
    Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto updateDto);
    
    /// <summary>
    /// Deletes a customer from the database
    /// </summary>
    /// <param name="id">The customer's unique identifier</param>
    /// <returns>True if the customer was deleted; otherwise, false</returns>
    Task<bool> DeleteCustomerAsync(int id);
}
