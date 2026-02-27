namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a line item in a quote
/// </summary>
public class QuoteLine : EntityBase
{
    /// <summary>
    /// Foreign key to the parent quote
    /// </summary>
    public int QuoteId { get; set; }

    /// <summary>
    /// Foreign key to the service being quoted (null for domain-only lines)
    /// </summary>
    public int? ServiceId { get; set; }

    /// <summary>
    /// Foreign key to the billing cycle
    /// </summary>
    public int BillingCycleId { get; set; }

    /// <summary>
    /// Line number for ordering items on the quote
    /// </summary>
    public int LineNumber { get; set; } = 1;

    /// <summary>
    /// Description of the service
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Quantity of the service
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// One-time setup fee per unit
    /// </summary>
    public decimal SetupFee { get; set; }

    /// <summary>
    /// Recurring price per unit
    /// </summary>
    public decimal RecurringPrice { get; set; }

    /// <summary>
    /// Discount amount applied to this line
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Total setup fee (SetupFee * Quantity)
    /// </summary>
    public decimal TotalSetupFee { get; set; }

    /// <summary>
    /// Total recurring price (RecurringPrice * Quantity - Discount)
    /// </summary>
    public decimal TotalRecurringPrice { get; set; }

    /// <summary>
    /// Tax rate for this line item
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Tax amount for this line item
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Total amount including tax
    /// </summary>
    public decimal TotalWithTax { get; set; }

    /// <summary>
    /// Snapshot of service name for historical accuracy
    /// </summary>
    public string ServiceNameSnapshot { get; set; } = string.Empty;

    /// <summary>
    /// Snapshot of billing cycle name
    /// </summary>
    public string BillingCycleNameSnapshot { get; set; } = string.Empty;

    /// <summary>
    /// Additional notes for this line item
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Soft delete timestamp
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Navigation property to the parent quote
    /// </summary>
    public Quote Quote { get; set; } = null!;

    /// <summary>
    /// Navigation property to the service
    /// </summary>
    public Service? Service { get; set; }

    /// <summary>
    /// Navigation property to the billing cycle
    /// </summary>
    public BillingCycle BillingCycle { get; set; } = null!;
}
