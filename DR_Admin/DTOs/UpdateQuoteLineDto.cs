namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for updating a quote line item
/// </summary>
public class UpdateQuoteLineDto
{
    /// <summary>
    /// Quote line ID (0 for new lines)
    /// </summary>
    public int Id { get; set; }

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
    /// Custom description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Setup fee
    /// </summary>
    public decimal SetupFee { get; set; }

    /// <summary>
    /// Recurring price
    /// </summary>
    public decimal RecurringPrice { get; set; }

    /// <summary>
    /// Discount amount
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string Notes { get; set; } = string.Empty;
}
