using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for updating vendor costs
/// </summary>
public class UpdateVendorCostDto
{
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
}
