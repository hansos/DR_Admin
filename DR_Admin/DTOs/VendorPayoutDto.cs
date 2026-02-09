using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing an outbound payment to a vendor
/// </summary>
public class VendorPayoutDto
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the vendor ID
    /// </summary>
    public int VendorId { get; set; }

    /// <summary>
    /// Gets or sets the type of vendor
    /// </summary>
    public VendorType VendorType { get; set; }

    /// <summary>
    /// Gets or sets the vendor name snapshot
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
    /// Gets or sets the current payout status
    /// </summary>
    public VendorPayoutStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the scheduled processing date
    /// </summary>
    public DateTime ScheduledDate { get; set; }

    /// <summary>
    /// Gets or sets the actual processing date
    /// </summary>
    public DateTime? ProcessedDate { get; set; }

    /// <summary>
    /// Gets or sets the reason for payout failure
    /// </summary>
    public string FailureReason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of failed payout attempts
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Gets or sets the external transaction reference
    /// </summary>
    public string TransactionReference { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether manual intervention is required
    /// </summary>
    public bool RequiresManualIntervention { get; set; }

    /// <summary>
    /// Gets or sets the reason for manual intervention
    /// </summary>
    public string InterventionReason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when manual intervention was resolved
    /// </summary>
    public DateTime? InterventionResolvedAt { get; set; }

    /// <summary>
    /// Gets or sets the user ID who resolved the intervention
    /// </summary>
    public int? InterventionResolvedByUserId { get; set; }

    /// <summary>
    /// Gets or sets internal notes
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of vendor costs included in this payout
    /// </summary>
    public List<VendorCostDto> VendorCosts { get; set; } = new();

    /// <summary>
    /// Gets or sets the timestamp when the record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
