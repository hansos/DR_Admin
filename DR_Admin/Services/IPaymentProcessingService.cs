using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for payment processing operations
/// </summary>
public interface IPaymentProcessingService
{
    /// <summary>
    /// Processes an invoice payment with selected payment method
    /// </summary>
    Task<PaymentResultDto> ProcessInvoicePaymentAsync(ProcessInvoicePaymentDto dto);

    /// <summary>
    /// Handles payment callbacks from gateways (webhooks)
    /// </summary>
    Task<bool> HandlePaymentWebhookAsync(string gatewayName, string payload, string signature);

    /// <summary>
    /// Retries a failed payment
    /// </summary>
    Task<PaymentResultDto> RetryFailedPaymentAsync(int paymentAttemptId);

    /// <summary>
    /// Applies customer credit to an invoice
    /// </summary>
    Task<PaymentResultDto> ApplyCustomerCreditAsync(ApplyCustomerCreditDto dto);

    /// <summary>
    /// Processes a partial payment
    /// </summary>
    Task<PaymentResultDto> ProcessPartialPaymentAsync(ProcessPartialPaymentDto dto);

    /// <summary>
    /// Confirms a payment that requires authentication (3D Secure)
    /// </summary>
    Task<PaymentResultDto> ConfirmAuthenticationAsync(int paymentAttemptId);

    /// <summary>
    /// Gets payment attempts for an invoice
    /// </summary>
    Task<IEnumerable<PaymentAttemptDto>> GetPaymentAttemptsByInvoiceIdAsync(int invoiceId);

    /// <summary>
    /// Gets a payment attempt by ID
    /// </summary>
    Task<PaymentAttemptDto?> GetPaymentAttemptByIdAsync(int id);
}
