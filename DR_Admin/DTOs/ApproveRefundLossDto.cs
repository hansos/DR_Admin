namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for approving refund loss audits
/// </summary>
public class ApproveRefundLossDto
{
    /// <summary>
    /// Gets or sets the refund loss audit ID
    /// </summary>
    public int RefundLossAuditId { get; set; }

    /// <summary>
    /// Gets or sets the user ID who is approving
    /// </summary>
    public int ApprovedByUserId { get; set; }

    /// <summary>
    /// Gets or sets whether to approve (true) or deny (false)
    /// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// Gets or sets the reason for denial (required if IsApproved is false)
    /// </summary>
    public string? DenialReason { get; set; }

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    public string? Notes { get; set; }
}
