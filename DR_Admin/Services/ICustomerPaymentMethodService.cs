using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing customer payment methods
/// </summary>
public interface ICustomerPaymentMethodService
{
    /// <summary>
    /// Retrieves all payment methods for a customer
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>A collection of customer payment method DTOs</returns>
    Task<IEnumerable<CustomerPaymentMethodDto>> GetPaymentMethodsByCustomerIdAsync(int customerId);

    /// <summary>
    /// Retrieves a payment method by ID
    /// </summary>
    /// <param name="id">The payment method ID</param>
    /// <returns>The payment method DTO if found, otherwise null</returns>
    Task<CustomerPaymentMethodDto?> GetPaymentMethodByIdAsync(int id);

    /// <summary>
    /// Retrieves the default payment method for a customer
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>The default payment method DTO if found, otherwise null</returns>
    Task<CustomerPaymentMethodDto?> GetDefaultPaymentMethodAsync(int customerId);

    /// <summary>
    /// Creates a new payment method
    /// </summary>
    /// <param name="createDto">The payment method creation data</param>
    /// <returns>The created payment method DTO</returns>
    Task<CustomerPaymentMethodDto> CreatePaymentMethodAsync(CreateCustomerPaymentMethodDto createDto);

    /// <summary>
    /// Sets a payment method as default
    /// </summary>
    /// <param name="id">The payment method ID</param>
    /// <param name="customerId">The customer ID</param>
    /// <returns>True if successful, otherwise false</returns>
    Task<bool> SetAsDefaultAsync(int id, int customerId);

    /// <summary>
    /// Deletes a payment method
    /// </summary>
    /// <param name="id">The payment method ID</param>
    /// <param name="customerId">The customer ID</param>
    /// <returns>True if successful, otherwise false</returns>
    Task<bool> DeletePaymentMethodAsync(int id, int customerId);
}
