using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing payment intents
/// </summary>
public interface IPaymentIntentService
{
    /// <summary>
    /// Retrieves all payment intents
    /// </summary>
    /// <returns>A collection of payment intent DTOs</returns>
    Task<IEnumerable<PaymentIntentDto>> GetAllPaymentIntentsAsync();

    /// <summary>
    /// Retrieves a payment intent by ID
    /// </summary>
    /// <param name="id">The payment intent ID</param>
    /// <returns>The payment intent DTO if found, otherwise null</returns>
    Task<PaymentIntentDto?> GetPaymentIntentByIdAsync(int id);

    /// <summary>
    /// Retrieves payment intents by customer ID
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>A collection of payment intent DTOs</returns>
    Task<IEnumerable<PaymentIntentDto>> GetPaymentIntentsByCustomerIdAsync(int customerId);

    /// <summary>
    /// Creates a new payment intent
    /// </summary>
    /// <param name="createDto">The payment intent creation data</param>
    /// <param name="customerId">The customer ID</param>
    /// <returns>The created payment intent DTO</returns>
    Task<PaymentIntentDto> CreatePaymentIntentAsync(CreatePaymentIntentDto createDto, int customerId);

    /// <summary>
    /// Confirms a payment intent
    /// </summary>
    /// <param name="id">The payment intent ID</param>
    /// <param name="paymentMethodToken">The payment method token</param>
    /// <returns>True if successful, otherwise false</returns>
    Task<bool> ConfirmPaymentIntentAsync(int id, string paymentMethodToken);

    /// <summary>
    /// Cancels a payment intent
    /// </summary>
    /// <param name="id">The payment intent ID</param>
    /// <returns>True if successful, otherwise false</returns>
    Task<bool> CancelPaymentIntentAsync(int id);

    /// <summary>
    /// Processes a webhook from the payment gateway
    /// </summary>
    /// <param name="gatewayId">The payment gateway ID</param>
    /// <param name="payload">The webhook payload</param>
    /// <returns>True if successfully processed</returns>
    Task<bool> ProcessWebhookAsync(int gatewayId, string payload);
}
