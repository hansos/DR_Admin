namespace ISPAdmin.Services;

/// <summary>
/// Default VAT validation service implementation.
/// </summary>
public class VatValidationService : IVatValidationService
{
    /// <summary>
    /// Validates a tax identifier for a country.
    /// </summary>
    /// <param name="countryCode">Country code for validation.</param>
    /// <param name="taxId">Tax identifier value.</param>
    /// <returns>True when tax identifier is valid; otherwise false.</returns>
    public Task<bool> ValidateAsync(string countryCode, string taxId)
    {
        if (string.IsNullOrWhiteSpace(countryCode) || string.IsNullOrWhiteSpace(taxId))
        {
            return Task.FromResult(false);
        }

        var compact = taxId.Replace(" ", string.Empty).Replace("-", string.Empty);
        if (compact.Length < 6)
        {
            return Task.FromResult(false);
        }

        var startsWithCountry = compact.StartsWith(countryCode, StringComparison.OrdinalIgnoreCase);
        return Task.FromResult(startsWithCountry || compact.Any(char.IsDigit));
    }
}
