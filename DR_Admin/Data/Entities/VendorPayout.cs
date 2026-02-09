using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents an outbound payment to a vendor
/// </summary>
public class VendorPayout : EntityBase
{
    /// <summary>
    /// Generic vendor identifier (Registrar ID, HostingProvider ID, etc.)
    /// </summary>
    public int VendorId { get; set; }

    /// <summary>
    /// Type of vendor receiving the payout
    /// </summary>
    public VendorType VendorType { get; set; }

    /// <summary>
    /// Snapshot of vendor name at payout time
    /// </summary>
    public string VendorName { get; set; } = string.Empty;

    /// <summary>
    /// Method used for this payout
    /// </summary>
    public PayoutMethod PayoutMethod { get; set; }

    /// <summary>
    /// Currency code for vendor payout (ISO 4217)
    /// </summary>
    public string VendorCurrency { get; set; } = "USD";

    /// <summary>
    /// Payout amount in vendor's currency
    /// </summary>
    public decimal VendorAmount { get; set; }

    /// <summary>
    /// Base currency for accounting (e.g., EUR)
    /// </summary>
    public string BaseCurrency { get; set; } = "EUR";

    /// <summary>
    /// Payout amount converted to base currency
    /// </summary>
    public decimal BaseAmount { get; set; }

    /// <summary>
    /// Exchange rate used for conversion
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// Date of the exchange rate snapshot
    /// </summary>
    public DateTime ExchangeRateDate { get; set; }

    /// <summary>
    /// Current status of the payout
    /// </summary>
    public VendorPayoutStatus Status { get; set; } = VendorPayoutStatus.Pending;

    /// <summary>
    /// Scheduled date for payout processing
    /// </summary>
    public DateTime ScheduledDate { get; set; }

    /// <summary>
    /// Actual date when payout was processed
    /// </summary>
    public DateTime? ProcessedDate { get; set; }

    /// <summary>
    /// Reason for payout failure
    /// </summary>
    public string FailureReason { get; set; } = string.Empty;

    /// <summary>
    /// Number of failed payout attempts
    /// </summary>
    public int FailureCount { get; set; } = 0;

    /// <summary>
    /// External transaction reference from payment system
    /// </summary>
    public string TransactionReference { get; set; } = string.Empty;

    /// <summary>
    /// Raw response from payment gateway (JSON format)
    /// </summary>
    public string PaymentGatewayResponse { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if this payout requires manual intervention
    /// </summary>
    public bool RequiresManualIntervention { get; set; } = false;

    /// <summary>
    /// Reason why manual intervention is required
    /// </summary>
    public string InterventionReason { get; set; } = string.Empty;

    /// <summary>
    /// Date when manual intervention was resolved
    /// </summary>
    public DateTime? InterventionResolvedAt { get; set; }

    /// <summary>
    /// Foreign key to user who resolved the intervention
    /// </summary>
    public int? InterventionResolvedByUserId { get; set; }

    /// <summary>
    /// Internal notes about this payout
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to user who resolved intervention
    /// </summary>
    public User? InterventionResolvedByUser { get; set; }

    /// <summary>
    /// Collection of vendor costs associated with this payout
    /// </summary>
    public ICollection<VendorCost> VendorCosts { get; set; } = new List<VendorCost>();
}
