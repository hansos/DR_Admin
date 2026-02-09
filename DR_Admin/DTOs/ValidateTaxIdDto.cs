namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for tax ID validation requests
/// </summary>
public class ValidateTaxIdDto
{
    /// <summary>
    /// Gets or sets the customer tax profile ID
    /// </summary>
    public int CustomerTaxProfileId { get; set; }

    /// <summary>
    /// Gets or sets whether to force revalidation even if already validated
    /// </summary>
    public bool ForceRevalidation { get; set; } = false;
}
