namespace ISPAdmin.Services;

/// <summary>
/// Represents VAT validation output.
/// </summary>
public sealed class VatValidationResult
{
    /// <summary>
    /// Gets or sets whether the tax identifier is valid.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the provider name that produced this result.
    /// </summary>
    public string ProviderName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a raw response payload for audit.
    /// </summary>
    public string RawResponse { get; set; } = string.Empty;
}
