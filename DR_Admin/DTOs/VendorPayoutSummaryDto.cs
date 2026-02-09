using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing vendor payout summary
/// </summary>
public class VendorPayoutSummaryDto
{
    /// <summary>
    /// Gets or sets the vendor ID
    /// </summary>
    public int VendorId { get; set; }

    /// <summary>
    /// Gets or sets the vendor name
    /// </summary>
    public string VendorName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vendor type
    /// </summary>
    public VendorType VendorType { get; set; }

    /// <summary>
    /// Gets or sets the total pending payouts
    /// </summary>
    public decimal TotalPending { get; set; }

    /// <summary>
    /// Gets or sets the total processing payouts
    /// </summary>
    public decimal TotalProcessing { get; set; }

    /// <summary>
    /// Gets or sets the total paid payouts
    /// </summary>
    public decimal TotalPaid { get; set; }

    /// <summary>
    /// Gets or sets the total failed payouts
    /// </summary>
    public decimal TotalFailed { get; set; }

    /// <summary>
    /// Gets or sets the currency code
    /// </summary>
    public string CurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets the number of pending payouts
    /// </summary>
    public int PendingCount { get; set; }

    /// <summary>
    /// Gets or sets the number of payouts requiring manual intervention
    /// </summary>
    public int RequiresInterventionCount { get; set; }

    /// <summary>
    /// Gets or sets the next scheduled payout date
    /// </summary>
    public DateTime? NextScheduledDate { get; set; }

    /// <summary>
    /// Gets or sets the list of recent payouts
    /// </summary>
    public List<VendorPayoutDto> RecentPayouts { get; set; } = new();
}
