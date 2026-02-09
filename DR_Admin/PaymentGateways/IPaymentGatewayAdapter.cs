namespace ISPAdmin.PaymentGateways;

/// <summary>
/// Unified interface for payment gateway operations
/// </summary>
public interface IPaymentGatewayAdapter
{
    /// <summary>
    /// Gets the gateway name
    /// </summary>
    string GatewayName { get; }

    /// <summary>
    /// Charges a customer's payment method
    /// </summary>
    Task<GatewayChargeResult> ChargeAsync(ChargeRequest request);

    /// <summary>
    /// Creates a payment intent for 3D Secure authentication
    /// </summary>
    Task<GatewayPaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request);

    /// <summary>
    /// Confirms and captures a payment intent
    /// </summary>
    Task<GatewayChargeResult> ConfirmPaymentIntentAsync(string intentId);

    /// <summary>
    /// Saves a payment method for future use
    /// </summary>
    Task<GatewayPaymentMethodResult> SavePaymentMethodAsync(SavePaymentMethodRequest request);

    /// <summary>
    /// Processes a refund
    /// </summary>
    Task<GatewayRefundResult> RefundAsync(RefundRequest request);

    /// <summary>
    /// Verifies webhook signature
    /// </summary>
    Task<bool> VerifyWebhookSignatureAsync(string payload, string signature);

    /// <summary>
    /// Parses webhook payload into event
    /// </summary>
    Task<GatewayWebhookEvent> ParseWebhookAsync(string payload);
}
