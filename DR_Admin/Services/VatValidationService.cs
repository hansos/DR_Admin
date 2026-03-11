namespace ISPAdmin.Services;

/// <summary>
/// VAT validation service implementation using configured providers.
/// </summary>
public class VatValidationService : IVatValidationService
{
    private readonly IReadOnlyCollection<IVatValidationProvider> _providers;

    public VatValidationService(IEnumerable<IVatValidationProvider> providers)
    {
        _providers = providers.ToList();
    }

    /// <summary>
    /// Validates a tax identifier for a country.
    /// </summary>
    /// <param name="countryCode">Country code for validation.</param>
    /// <param name="taxId">Tax identifier value.</param>
    /// <returns>True when tax identifier is valid; otherwise false.</returns>
    public async Task<bool> ValidateAsync(string countryCode, string taxId)
    {
        var result = await ValidateDetailedAsync(countryCode, taxId);
        return result.IsValid;
    }

    /// <summary>
    /// Validates a tax identifier and returns detailed provider output.
    /// </summary>
    /// <param name="countryCode">Country code for validation.</param>
    /// <param name="taxId">Tax identifier value.</param>
    /// <returns>Detailed validation result.</returns>
    public async Task<VatValidationResult> ValidateDetailedAsync(string countryCode, string taxId)
    {
        VatValidationResult? lastResult = null;

        foreach (var provider in _providers)
        {
            var result = await provider.ValidateAsync(countryCode, taxId);
            lastResult = result;
            if (result.IsValid)
            {
                return result;
            }
        }

        return lastResult ?? new VatValidationResult
        {
            IsValid = false,
            ProviderName = _providers.FirstOrDefault()?.Name ?? "None",
            RawResponse = "All providers returned invalid"
        };
    }
}
