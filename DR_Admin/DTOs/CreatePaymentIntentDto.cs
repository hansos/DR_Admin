namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for creating a payment intent
/// </summary>
public class CreatePaymentIntentDto
{
    /// <summary>
    /// Invoice ID to pay (optional if OrderId is provided)
    /// </summary>
    public int? InvoiceId { get; set; }

    /// <summary>
    /// Order ID to pay (optional if InvoiceId is provided)
    /// </summary>
    public int? OrderId { get; set; }

    /// <summary>
    /// Amount to charge
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>
    /// Payment gateway ID to use
    /// </summary>
    public int PaymentGatewayId { get; set; }

    /// <summary>
    /// Return URL after successful payment
    /// </summary>
    public string ReturnUrl { get; set; } = string.Empty;

    /// <summary>
    /// Cancel URL if payment is cancelled
    /// </summary>
    public string CancelUrl { get; set; } = string.Empty;

    /// <summary>
    /// Payment description
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
