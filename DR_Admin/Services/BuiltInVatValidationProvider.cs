namespace ISPAdmin.Services;

/// <summary>
/// Built-in VAT validation provider with format-level checks.
/// </summary>
public class BuiltInVatValidationProvider : IVatValidationProvider
{
    /// <summary>
    /// Gets provider name.
    /// </summary>
    public string Name => "BuiltInFormatProvider";

    /// <summary>
    /// Validates tax identifier format.
    /// </summary>
    /// <param name="countryCode">Country code.</param>
    /// <param name="taxId">Tax identifier.</param>
    /// <returns>Validation result.</returns>
    public Task<VatValidationResult> ValidateAsync(string countryCode, string taxId)
    {
        if (string.IsNullOrWhiteSpace(countryCode) || string.IsNullOrWhiteSpace(taxId))
        {
            return Task.FromResult(new VatValidationResult
            {
                IsValid = false,
                ProviderName = Name,
                RawResponse = "Missing country code or tax ID"
            });
        }

        var compact = taxId.Replace(" ", string.Empty).Replace("-", string.Empty);
        var startsWithCountry = compact.StartsWith(countryCode, StringComparison.OrdinalIgnoreCase);
        var isValid = compact.Length >= 6 && (startsWithCountry || compact.Any(char.IsDigit));

        return Task.FromResult(new VatValidationResult
        {
            IsValid = isValid,
            ProviderName = Name,
            RawResponse = $"compact={compact};startsWithCountry={startsWithCountry}"
        });
    }
}
