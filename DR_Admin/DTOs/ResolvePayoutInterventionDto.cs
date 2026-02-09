namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for resolving vendor payout manual interventions
/// </summary>
public class ResolvePayoutInterventionDto
{
    /// <summary>
    /// Gets or sets the vendor payout ID
    /// </summary>
    public int VendorPayoutId { get; set; }

    /// <summary>
    /// Gets or sets the user ID who resolved the intervention
    /// </summary>
    public int ResolvedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the resolution notes
    /// </summary>
    public string ResolutionNotes { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to proceed with the payout after resolution
    /// </summary>
    public bool ProceedWithPayout { get; set; }
}
