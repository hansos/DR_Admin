using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing vendor costs for invoice line items
/// </summary>
public class VendorCostDto
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the invoice line ID
    /// </summary>
    public int InvoiceLineId { get; set; }

    /// <summary>
    /// Gets or sets the vendor payout ID (null if not yet paid)
    /// </summary>
    public int? VendorPayoutId { get; set; }

    /// <summary>
    /// Gets or sets the type of vendor
    /// </summary>
    public VendorType VendorType { get; set; }

    /// <summary>
    /// Gets or sets the vendor ID
    /// </summary>
    public int? VendorId { get; set; }

    /// <summary>
    /// Gets or sets the vendor name snapshot
    /// </summary>
    public string VendorName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vendor's currency code
    /// </summary>
    public string VendorCurrency { get; set; } = "USD";

    /// <summary>
    /// Gets or sets the cost amount in vendor's currency
    /// </summary>
    public decimal VendorAmount { get; set; }

    /// <summary>
    /// Gets or sets the base currency code
    /// </summary>
    public string BaseCurrency { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets the cost amount in base currency
    /// </summary>
    public decimal BaseAmount { get; set; }

    /// <summary>
    /// Gets or sets the exchange rate used for conversion
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// Gets or sets the date of the exchange rate snapshot
    /// </summary>
    public DateTime ExchangeRateDate { get; set; }

    /// <summary>
    /// Gets or sets whether this cost can be refunded by the vendor
    /// </summary>
    public bool IsRefundable { get; set; }

    /// <summary>
    /// Gets or sets the refund policy
    /// </summary>
    public RefundPolicy RefundPolicy { get; set; }

    /// <summary>
    /// Gets or sets the refund deadline
    /// </summary>
    public DateTime? RefundDeadline { get; set; }

    /// <summary>
    /// Gets or sets the current lifecycle status
    /// </summary>
    public VendorCostStatus Status { get; set; }

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
