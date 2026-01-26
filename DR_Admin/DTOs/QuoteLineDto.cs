namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a quote line item
/// </summary>
public class QuoteLineDto
{
    /// <summary>
    /// Unique identifier for the quote line
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Quote ID
    /// </summary>
    public int QuoteId { get; set; }

    /// <summary>
    /// Service ID
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    /// Service name
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Billing cycle ID
    /// </summary>
    public int BillingCycleId { get; set; }

    /// <summary>
    /// Billing cycle name
    /// </summary>
    public string BillingCycleName { get; set; } = string.Empty;

    /// <summary>
    /// Line number for ordering
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// Description of the service
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Setup fee per unit
    /// </summary>
    public decimal SetupFee { get; set; }

    /// <summary>
    /// Recurring price per unit
    /// </summary>
    public decimal RecurringPrice { get; set; }

    /// <summary>
    /// Discount amount
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Total setup fee
    /// </summary>
    public decimal TotalSetupFee { get; set; }

    /// <summary>
    /// Total recurring price
    /// </summary>
    public decimal TotalRecurringPrice { get; set; }

    /// <summary>
    /// Tax rate
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Tax amount
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Total including tax
    /// </summary>
    public decimal TotalWithTax { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string Notes { get; set; } = string.Empty;
}
