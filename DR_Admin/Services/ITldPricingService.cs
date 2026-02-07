using ISPAdmin.DTOs;
using ISPAdmin.Data.Entities;
using ISPAdmin.Services.Helpers;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for comprehensive TLD pricing management
/// Handles cost pricing, sales pricing, discounts, and margin analysis
/// </summary>
public interface ITldPricingService
{
    // ==================== RegistrarTldCostPricing Methods ====================

    /// <summary>
    /// Gets all cost pricing records for a specific registrar-TLD relationship
    /// </summary>
    /// <param name="registrarTldId">The registrar-TLD ID</param>
    /// <param name="includeArchived">Whether to include archived (past) pricing</param>
    /// <returns>List of cost pricing records</returns>
    Task<List<RegistrarTldCostPricingDto>> GetCostPricingHistoryAsync(int registrarTldId, bool includeArchived = false);

    /// <summary>
    /// Gets the current effective cost pricing for a registrar-TLD
    /// </summary>
    /// <param name="registrarTldId">The registrar-TLD ID</param>
    /// <param name="effectiveDate">The date to check (default: now)</param>
    /// <returns>Current cost pricing or null if none found</returns>
    Task<RegistrarTldCostPricingDto?> GetCurrentCostPricingAsync(int registrarTldId, DateTime? effectiveDate = null);

    /// <summary>
    /// Gets future scheduled cost pricing for a registrar-TLD
    /// </summary>
    /// <param name="registrarTldId">The registrar-TLD ID</param>
    /// <returns>List of future cost pricing records</returns>
    Task<List<RegistrarTldCostPricingDto>> GetFutureCostPricingAsync(int registrarTldId);

    /// <summary>
    /// Creates new cost pricing for a registrar-TLD
    /// </summary>
    /// <param name="createDto">The creation data</param>
    /// <param name="createdBy">User creating the record</param>
    /// <returns>Created cost pricing</returns>
    Task<RegistrarTldCostPricingDto> CreateCostPricingAsync(CreateRegistrarTldCostPricingDto createDto, string? createdBy);

    /// <summary>
    /// Updates existing cost pricing (only future pricing if AllowEditingFuturePrices is true)
    /// </summary>
    /// <param name="id">The cost pricing ID</param>
    /// <param name="updateDto">The update data</param>
    /// <param name="modifiedBy">User modifying the record</param>
    /// <returns>Updated cost pricing or null if not found/not allowed</returns>
    Task<RegistrarTldCostPricingDto?> UpdateCostPricingAsync(int id, UpdateRegistrarTldCostPricingDto updateDto, string? modifiedBy);

    /// <summary>
    /// Deletes future cost pricing (only if not yet effective and AllowEditingFuturePrices is true)
    /// </summary>
    /// <param name="id">The cost pricing ID</param>
    /// <returns>True if deleted, false if not found/not allowed</returns>
    Task<bool> DeleteFutureCostPricingAsync(int id);

    // ==================== TldSalesPricing Methods ====================

    /// <summary>
    /// Gets all sales pricing records for a specific TLD
    /// </summary>
    /// <param name="tldId">The TLD ID</param>
    /// <param name="includeArchived">Whether to include archived (past) pricing</param>
    /// <returns>List of sales pricing records</returns>
    Task<List<TldSalesPricingDto>> GetSalesPricingHistoryAsync(int tldId, bool includeArchived = false);

    /// <summary>
    /// Gets the current effective sales pricing for a TLD
    /// </summary>
    /// <param name="tldId">The TLD ID</param>
    /// <param name="effectiveDate">The date to check (default: now)</param>
    /// <returns>Current sales pricing or null if none found</returns>
    Task<TldSalesPricingDto?> GetCurrentSalesPricingAsync(int tldId, DateTime? effectiveDate = null);

    /// <summary>
    /// Gets future scheduled sales pricing for a TLD
    /// </summary>
    /// <param name="tldId">The TLD ID</param>
    /// <returns>List of future sales pricing records</returns>
    Task<List<TldSalesPricingDto>> GetFutureSalesPricingAsync(int tldId);

    /// <summary>
    /// Creates new sales pricing for a TLD
    /// </summary>
    /// <param name="createDto">The creation data</param>
    /// <param name="createdBy">User creating the record</param>
    /// <returns>Created sales pricing</returns>
    Task<TldSalesPricingDto> CreateSalesPricingAsync(CreateTldSalesPricingDto createDto, string? createdBy);

    /// <summary>
    /// Updates existing sales pricing (only future pricing if AllowEditingFuturePrices is true)
    /// </summary>
    /// <param name="id">The sales pricing ID</param>
    /// <param name="updateDto">The update data</param>
    /// <param name="modifiedBy">User modifying the record</param>
    /// <returns>Updated sales pricing or null if not found/not allowed</returns>
    Task<TldSalesPricingDto?> UpdateSalesPricingAsync(int id, UpdateTldSalesPricingDto updateDto, string? modifiedBy);

    /// <summary>
    /// Deletes future sales pricing (only if not yet effective and AllowEditingFuturePrices is true)
    /// </summary>
    /// <param name="id">The sales pricing ID</param>
    /// <returns>True if deleted, false if not found/not allowed</returns>
    Task<bool> DeleteFutureSalesPricingAsync(int id);

    // ==================== ResellerTldDiscount Methods ====================

    /// <summary>
    /// Gets all discount records for a specific reseller company
    /// </summary>
    /// <param name="resellerCompanyId">The reseller company ID</param>
    /// <param name="includeArchived">Whether to include archived (past) discounts</param>
    /// <returns>List of discount records</returns>
    Task<List<ResellerTldDiscountDto>> GetResellerDiscountsAsync(int resellerCompanyId, bool includeArchived = false);

    /// <summary>
    /// Gets current effective discount for a reseller company and TLD
    /// </summary>
    /// <param name="resellerCompanyId">The reseller company ID</param>
    /// <param name="tldId">The TLD ID</param>
    /// <param name="effectiveDate">The date to check (default: now)</param>
    /// <returns>Current discount or null if none found</returns>
    Task<ResellerTldDiscountDto?> GetCurrentDiscountAsync(int resellerCompanyId, int tldId, DateTime? effectiveDate = null);

    /// <summary>
    /// Creates new discount for a reseller company and TLD
    /// </summary>
    /// <param name="createDto">The creation data</param>
    /// <param name="createdBy">User creating the record</param>
    /// <returns>Created discount</returns>
    Task<ResellerTldDiscountDto> CreateDiscountAsync(CreateResellerTldDiscountDto createDto, string? createdBy);

    /// <summary>
    /// Updates existing discount
    /// </summary>
    /// <param name="id">The discount ID</param>
    /// <param name="updateDto">The update data</param>
    /// <param name="modifiedBy">User modifying the record</param>
    /// <returns>Updated discount or null if not found</returns>
    Task<ResellerTldDiscountDto?> UpdateDiscountAsync(int id, UpdateResellerTldDiscountDto updateDto, string? modifiedBy);

    /// <summary>
    /// Deletes a discount
    /// </summary>
    /// <param name="id">The discount ID</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteDiscountAsync(int id);

    // ==================== RegistrarSelectionPreference Methods ====================

    /// <summary>
    /// Gets all selection preferences for registrars
    /// </summary>
    /// <returns>List of selection preferences</returns>
    Task<List<RegistrarSelectionPreferenceDto>> GetAllSelectionPreferencesAsync();

    /// <summary>
    /// Gets selection preference for a specific registrar
    /// </summary>
    /// <param name="registrarId">The registrar ID</param>
    /// <returns>Selection preference or null if not found</returns>
    Task<RegistrarSelectionPreferenceDto?> GetSelectionPreferenceAsync(int registrarId);

    /// <summary>
    /// Creates new selection preference for a registrar
    /// </summary>
    /// <param name="createDto">The creation data</param>
    /// <returns>Created selection preference</returns>
    Task<RegistrarSelectionPreferenceDto> CreateSelectionPreferenceAsync(CreateRegistrarSelectionPreferenceDto createDto);

    /// <summary>
    /// Updates existing selection preference
    /// </summary>
    /// <param name="id">The preference ID</param>
    /// <param name="updateDto">The update data</param>
    /// <returns>Updated preference or null if not found</returns>
    Task<RegistrarSelectionPreferenceDto?> UpdateSelectionPreferenceAsync(int id, UpdateRegistrarSelectionPreferenceDto updateDto);

    /// <summary>
    /// Deletes a selection preference
    /// </summary>
    /// <param name="id">The preference ID</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteSelectionPreferenceAsync(int id);

    // ==================== Pricing Calculation Methods ====================

    /// <summary>
    /// Calculates final pricing for a TLD including discounts and promotions
    /// </summary>
    /// <param name="request">The calculation request</param>
    /// <returns>Calculated pricing response</returns>
    Task<CalculatePricingResponse> CalculatePricingAsync(CalculatePricingRequest request);

    /// <summary>
    /// Selects the optimal registrar for a TLD based on cost and preferences
    /// </summary>
    /// <param name="tldId">The TLD ID</param>
    /// <param name="customerId">Optional customer ID for bundling preferences</param>
    /// <returns>Selected registrar ID or null if none available</returns>
    Task<int?> SelectOptimalRegistrarAsync(int tldId, int? customerId = null);

    // ==================== Margin Analysis Methods ====================

    /// <summary>
    /// Calculates margin for a specific TLD and operation type
    /// </summary>
    /// <param name="tldId">The TLD ID</param>
    /// <param name="operationType">The operation type (Registration, Renewal, Transfer)</param>
    /// <param name="registrarId">Optional specific registrar ID (otherwise uses optimal)</param>
    /// <returns>Margin analysis result</returns>
    Task<MarginAnalysisResult> CalculateMarginAsync(int tldId, string operationType, int? registrarId = null);

    /// <summary>
    /// Gets all TLDs with negative margins
    /// </summary>
    /// <returns>List of margin analysis results with negative margins</returns>
    Task<List<MarginAnalysisResult>> GetNegativeMarginReportAsync();

    /// <summary>
    /// Gets all TLDs with low margins (below threshold)
    /// </summary>
    /// <returns>List of margin analysis results with low margins</returns>
    Task<List<MarginAnalysisResult>> GetLowMarginReportAsync();

    // ==================== Currency Conversion Methods ====================

    /// <summary>
    /// Converts amount from one currency to another using exchange rates
    /// </summary>
    /// <param name="amount">The amount to convert</param>
    /// <param name="fromCurrency">Source currency code</param>
    /// <param name="toCurrency">Target currency code</param>
    /// <param name="effectiveDate">The date for exchange rate (default: now)</param>
    /// <returns>Converted amount or null if rate not found</returns>
    Task<decimal?> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency, DateTime? effectiveDate = null);

    // ==================== Archive Management Methods ====================

    /// <summary>
    /// Archives old cost pricing data based on retention policy
    /// </summary>
    /// <returns>Number of records archived</returns>
    Task<int> ArchiveOldCostPricingAsync();

    /// <summary>
    /// Archives old sales pricing data based on retention policy
    /// </summary>
    /// <returns>Number of records archived</returns>
    Task<int> ArchiveOldSalesPricingAsync();

    /// <summary>
    /// Archives old discount data based on retention policy
    /// </summary>
    /// <returns>Number of records archived</returns>
    Task<int> ArchiveOldDiscountsAsync();
}
