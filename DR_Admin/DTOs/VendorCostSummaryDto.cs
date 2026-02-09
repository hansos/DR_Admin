namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing vendor cost summary for an invoice
/// </summary>
public class VendorCostSummaryDto
{
    /// <summary>
    /// Gets or sets the invoice ID
    /// </summary>
    public int InvoiceId { get; set; }

    /// <summary>
    /// Gets or sets the invoice number
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total invoice amount
    /// </summary>
    public decimal InvoiceTotal { get; set; }

    /// <summary>
    /// Gets or sets the currency code
    /// </summary>
    public string CurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets the total vendor costs
    /// </summary>
    public decimal TotalVendorCosts { get; set; }

    /// <summary>
    /// Gets or sets the total paid vendor costs
    /// </summary>
    public decimal TotalPaidVendorCosts { get; set; }

    /// <summary>
    /// Gets or sets the total unpaid vendor costs
    /// </summary>
    public decimal TotalUnpaidVendorCosts { get; set; }

    /// <summary>
    /// Gets or sets the total refundable vendor costs
    /// </summary>
    public decimal TotalRefundableVendorCosts { get; set; }

    /// <summary>
    /// Gets or sets the total non-refundable vendor costs
    /// </summary>
    public decimal TotalNonRefundableVendorCosts { get; set; }

    /// <summary>
    /// Gets or sets the gross profit (invoice total - vendor costs)
    /// </summary>
    public decimal GrossProfit { get; set; }

    /// <summary>
    /// Gets or sets the gross profit margin percentage
    /// </summary>
    public decimal GrossProfitMargin { get; set; }

    /// <summary>
    /// Gets or sets the list of vendor costs
    /// </summary>
    public List<VendorCostDto> VendorCosts { get; set; } = new();
}
