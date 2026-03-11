namespace ISPAdmin.Services;

/// <summary>
/// Contract for pluggable VAT/tax ID validation providers.
/// </summary>
public interface IVatValidationProvider
{
    /// <summary>
    /// Gets provider name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Validates a tax identifier.
    /// </summary>
    /// <param name="countryCode">Country code.</param>
    /// <param name="taxId">Tax identifier.</param>
    /// <returns>Validation result.</returns>
    Task<VatValidationResult> ValidateAsync(string countryCode, string taxId);
}
