namespace ISPAdmin.Services;

/// <summary>
/// Service interface for validating VAT or tax identification numbers.
/// </summary>
public interface IVatValidationService
{
    /// <summary>
    /// Validates a tax identifier for a country.
    /// </summary>
    /// <param name="countryCode">Country code for validation.</param>
    /// <param name="taxId">Tax identifier value.</param>
    /// <returns>True when tax identifier is valid; otherwise false.</returns>
    Task<bool> ValidateAsync(string countryCode, string taxId);
}
