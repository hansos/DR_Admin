using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing tax rules and calculations
/// </summary>
public interface ITaxService
{
    /// <summary>
    /// Retrieves all tax rules
    /// </summary>
    /// <returns>A collection of tax rule DTOs</returns>
    Task<IEnumerable<TaxRuleDto>> GetAllTaxRulesAsync();

    /// <summary>
    /// Retrieves active tax rules
    /// </summary>
    /// <returns>A collection of active tax rule DTOs</returns>
    Task<IEnumerable<TaxRuleDto>> GetActiveTaxRulesAsync();

    /// <summary>
    /// Retrieves a tax rule by ID
    /// </summary>
    /// <param name="id">The tax rule ID</param>
    /// <returns>The tax rule DTO if found, otherwise null</returns>
    Task<TaxRuleDto?> GetTaxRuleByIdAsync(int id);

    /// <summary>
    /// Retrieves tax rules by country and state
    /// </summary>
    /// <param name="countryCode">The country code</param>
    /// <param name="stateCode">The state code (optional)</param>
    /// <returns>A collection of applicable tax rule DTOs</returns>
    Task<IEnumerable<TaxRuleDto>> GetTaxRulesByLocationAsync(string countryCode, string? stateCode = null);

    /// <summary>
    /// Creates a new tax rule
    /// </summary>
    /// <param name="createDto">The tax rule creation data</param>
    /// <returns>The created tax rule DTO</returns>
    Task<TaxRuleDto> CreateTaxRuleAsync(CreateTaxRuleDto createDto);

    /// <summary>
    /// Updates an existing tax rule
    /// </summary>
    /// <param name="id">The tax rule ID</param>
    /// <param name="updateDto">The tax rule update data</param>
    /// <returns>The updated tax rule DTO if successful, otherwise null</returns>
    Task<TaxRuleDto?> UpdateTaxRuleAsync(int id, UpdateTaxRuleDto updateDto);

    /// <summary>
    /// Deletes a tax rule (soft delete)
    /// </summary>
    /// <param name="id">The tax rule ID</param>
    /// <returns>True if successful, otherwise false</returns>
    Task<bool> DeleteTaxRuleAsync(int id);

    /// <summary>
    /// Calculates tax for a customer based on their location
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="amount">The taxable amount</param>
    /// <param name="isSetupFee">Whether this is a setup fee</param>
    /// <returns>The calculated tax amount and rate</returns>
    Task<(decimal taxAmount, decimal taxRate, string taxName)> CalculateTaxAsync(int customerId, decimal amount, bool isSetupFee = false);

    /// <summary>
    /// Validates a VAT number (EU VIES check)
    /// </summary>
    /// <param name="vatNumber">The VAT number to validate</param>
    /// <param name="countryCode">The country code</param>
    /// <returns>True if valid, otherwise false</returns>
    Task<bool> ValidateVatNumberAsync(string vatNumber, string countryCode);
}
