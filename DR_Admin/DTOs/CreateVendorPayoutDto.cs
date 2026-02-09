using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for creating vendor payouts
/// </summary>
public class CreateVendorPayoutDto
{
    /// <summary>
    /// Gets or sets the vendor ID
    /// </summary>
    public int VendorId { get; set; }

    /// <summary>
    /// Gets or sets the type of vendor
    /// </summary>
    public VendorType VendorType { get; set; }

    /// <summary>
    /// Gets or sets the vendor name
    /// </summary>
    public string VendorName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the payout method
    /// </summary>
    public PayoutMethod PayoutMethod { get; set; }

    /// <summary>
    /// Gets or sets the vendor's currency code
    /// </summary>
    public string VendorCurrency { get; set; } = "USD";

    /// <summary>
    /// Gets or sets the payout amount in vendor's currency
    /// </summary>
    public decimal VendorAmount { get; set; }

    /// <summary>
    /// Gets or sets the base currency code
    /// </summary>
    public string BaseCurrency { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets the payout amount in base currency
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
    /// Gets or sets the scheduled processing date
    /// </summary>
    public DateTime ScheduledDate { get; set; }

    /// <summary>
    /// Gets or sets internal notes
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of vendor cost IDs to include in this payout
    /// </summary>
    public List<int> VendorCostIds { get; set; } = new();
}
