using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing payment gateways
/// </summary>
public interface IPaymentGatewayService
{
    /// <summary>
    /// Retrieves all payment gateways
    /// </summary>
    /// <returns>Collection of payment gateway DTOs</returns>
    Task<IEnumerable<PaymentGatewayDto>> GetAllPaymentGatewaysAsync();

    /// <summary>
    /// Retrieves all active payment gateways
    /// </summary>
    /// <returns>Collection of active payment gateway DTOs</returns>
    Task<IEnumerable<PaymentGatewayDto>> GetActivePaymentGatewaysAsync();

    /// <summary>
    /// Retrieves a specific payment gateway by ID
    /// </summary>
    /// <param name="id">Payment gateway identifier</param>
    /// <returns>Payment gateway DTO or null if not found</returns>
    Task<PaymentGatewayDto?> GetPaymentGatewayByIdAsync(int id);

    /// <summary>
    /// Retrieves the default payment gateway
    /// </summary>
    /// <returns>Default payment gateway DTO or null if not found</returns>
    Task<PaymentGatewayDto?> GetDefaultPaymentGatewayAsync();

    /// <summary>
    /// Retrieves a payment gateway by provider code
    /// </summary>
    /// <param name="providerCode">Provider code (stripe, paypal, square)</param>
    /// <returns>Payment gateway DTO or null if not found</returns>
    Task<PaymentGatewayDto?> GetPaymentGatewayByProviderAsync(string providerCode);

    /// <summary>
    /// Creates a new payment gateway
    /// </summary>
    /// <param name="dto">Payment gateway creation data</param>
    /// <returns>Created payment gateway DTO</returns>
    Task<PaymentGatewayDto> CreatePaymentGatewayAsync(CreatePaymentGatewayDto dto);

    /// <summary>
    /// Updates an existing payment gateway
    /// </summary>
    /// <param name="id">Payment gateway identifier</param>
    /// <param name="dto">Payment gateway update data</param>
    /// <returns>Updated payment gateway DTO or null if not found</returns>
    Task<PaymentGatewayDto?> UpdatePaymentGatewayAsync(int id, UpdatePaymentGatewayDto dto);

    /// <summary>
    /// Sets a payment gateway as the default
    /// </summary>
    /// <param name="id">Payment gateway identifier</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> SetDefaultPaymentGatewayAsync(int id);

    /// <summary>
    /// Activates or deactivates a payment gateway
    /// </summary>
    /// <param name="id">Payment gateway identifier</param>
    /// <param name="isActive">Active status</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> SetPaymentGatewayActiveStatusAsync(int id, bool isActive);

    /// <summary>
    /// Soft deletes a payment gateway
    /// </summary>
    /// <param name="id">Payment gateway identifier</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> DeletePaymentGatewayAsync(int id);
}
