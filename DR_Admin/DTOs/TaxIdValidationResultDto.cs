namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing tax ID validation results
/// </summary>
public class TaxIdValidationResultDto
{
    /// <summary>
    /// Gets or sets whether the tax ID is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the validation timestamp
    /// </summary>
    public DateTime ValidationDate { get; set; }

    /// <summary>
    /// Gets or sets the validation service name
    /// </summary>
    public string ValidationService { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company name from tax authority (if available)
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Gets or sets the registered address from tax authority (if available)
    /// </summary>
    public string? RegisteredAddress { get; set; }

    /// <summary>
    /// Gets or sets the raw response from validation service (JSON)
    /// </summary>
    public string RawResponse { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets any error message from validation service
    /// </summary>
    public string? ErrorMessage { get; set; }
}
