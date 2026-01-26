namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for creating a quote line item
/// </summary>
public class CreateQuoteLineDto
{
    /// <summary>
    /// Service ID
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    /// Billing cycle ID
    /// </summary>
    public int BillingCycleId { get; set; }

    /// <summary>
    /// Quantity
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Optional custom description (overrides service description)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional custom setup fee (overrides service setup fee)
    /// </summary>
    public decimal? SetupFee { get; set; }

    /// <summary>
    /// Optional custom recurring price (overrides service price)
    /// </summary>
    public decimal? RecurringPrice { get; set; }

    /// <summary>
    /// Optional discount for this line
    /// </summary>
    public decimal? Discount { get; set; }

    /// <summary>
    /// Additional notes for this line
    /// </summary>
    public string Notes { get; set; } = string.Empty;
}
