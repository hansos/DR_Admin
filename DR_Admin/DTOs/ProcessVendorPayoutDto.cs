namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for processing vendor payouts
/// </summary>
public class ProcessVendorPayoutDto
{
    /// <summary>
    /// Gets or sets the vendor payout ID
    /// </summary>
    public int VendorPayoutId { get; set; }

    /// <summary>
    /// Gets or sets the external transaction reference from payment system
    /// </summary>
    public string TransactionReference { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the raw response from payment gateway (JSON)
    /// </summary>
    public string PaymentGatewayResponse { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the payout was successful
    /// </summary>
    public bool IsSuccessful { get; set; }

    /// <summary>
    /// Gets or sets the reason for failure (if applicable)
    /// </summary>
    public string? FailureReason { get; set; }
}
