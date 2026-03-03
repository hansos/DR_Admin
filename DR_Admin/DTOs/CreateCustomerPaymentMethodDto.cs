using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for creating a customer payment method
/// </summary>
public class CreateCustomerPaymentMethodDto
{
    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Payment instrument selected by user (e.g., CreditCard, PayPal, Cash)
    /// </summary>
    public string PaymentInstrument { get; set; } = string.Empty;

    /// <summary>
    /// Payment method type
    /// </summary>
    public PaymentMethodType Type { get; set; }

    /// <summary>
    /// Payment method token from gateway
    /// </summary>
    public string PaymentMethodToken { get; set; } = string.Empty;

    /// <summary>
    /// Make this the default payment method
    /// </summary>
    public bool IsDefault { get; set; }
}
