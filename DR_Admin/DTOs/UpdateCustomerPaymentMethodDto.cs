using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for updating a customer payment method
/// </summary>
public class UpdateCustomerPaymentMethodDto
{
    /// <summary>
    /// Payment instrument selected by user (e.g., CreditCard, PayPal, Cash)
    /// </summary>
    public string PaymentInstrument { get; set; } = string.Empty;

    /// <summary>
    /// Payment method type
    /// </summary>
    public PaymentMethodType Type { get; set; }

    /// <summary>
    /// Make this the default payment method
    /// </summary>
    public bool IsDefault { get; set; }
}
