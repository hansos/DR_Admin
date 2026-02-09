using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for updating vendor payouts
/// </summary>
public class UpdateVendorPayoutDto
{
    /// <summary>
    /// Gets or sets the current payout status
    /// </summary>
    public VendorPayoutStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the scheduled processing date
    /// </summary>
    public DateTime ScheduledDate { get; set; }

    /// <summary>
    /// Gets or sets the reason for payout failure
    /// </summary>
    public string FailureReason { get; set; } = string.Empty;

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
    /// Gets or sets internal notes
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;
}
