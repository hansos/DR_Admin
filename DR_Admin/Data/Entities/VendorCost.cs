using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Tracks vendor costs associated with invoice line items for refund and profit calculations
/// </summary>
public class VendorCost : EntityBase
{
    /// <summary>
    /// Foreign key to the invoice line that incurred this cost
    /// </summary>
    public int InvoiceLineId { get; set; }

    /// <summary>
    /// Foreign key to the vendor payout (null if not yet paid)
    /// </summary>
    public int? VendorPayoutId { get; set; }

    /// <summary>
    /// Type of vendor
    /// </summary>
    public VendorType VendorType { get; set; }

    /// <summary>
    /// Foreign key to the specific vendor (Registrar ID, HostingProvider ID, etc.)
    /// </summary>
    public int? VendorId { get; set; }

    /// <summary>
    /// Snapshot of vendor name at time of cost creation
    /// </summary>
    public string VendorName { get; set; } = string.Empty;

    /// <summary>
    /// Currency code used by vendor (ISO 4217)
    /// </summary>
    public string VendorCurrency { get; set; } = "USD";

    /// <summary>
    /// Cost amount in vendor's currency
    /// </summary>
    public decimal VendorAmount { get; set; }

    /// <summary>
    /// Base currency for accounting purposes (e.g., EUR)
    /// </summary>
    public string BaseCurrency { get; set; } = "EUR";

    /// <summary>
    /// Cost amount converted to base currency
    /// </summary>
    public decimal BaseAmount { get; set; }

    /// <summary>
    /// Exchange rate used for conversion (vendor currency to base currency)
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// Date of the exchange rate snapshot
    /// </summary>
    public DateTime ExchangeRateDate { get; set; }

    /// <summary>
    /// Indicates if this cost can be refunded by the vendor
    /// </summary>
    public bool IsRefundable { get; set; } = true;

    /// <summary>
    /// Refund policy for this vendor cost
    /// </summary>
    public RefundPolicy RefundPolicy { get; set; } = RefundPolicy.FullyRefundable;

    /// <summary>
    /// Deadline for refund eligibility (null if no deadline)
    /// </summary>
    public DateTime? RefundDeadline { get; set; }

    /// <summary>
    /// Current lifecycle status of this vendor cost
    /// </summary>
    public VendorCostStatus Status { get; set; } = VendorCostStatus.Estimated;

    /// <summary>
    /// Additional notes about this vendor cost
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the invoice line
    /// </summary>
    public InvoiceLine InvoiceLine { get; set; } = null!;

    /// <summary>
    /// Navigation property to the vendor payout
    /// </summary>
    public VendorPayout? VendorPayout { get; set; }
}
