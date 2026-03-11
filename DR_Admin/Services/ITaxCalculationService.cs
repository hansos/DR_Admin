using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for tax quote and finalize operations.
/// </summary>
public interface ITaxCalculationService
{
    /// <summary>
    /// Calculates a tax quote without persisting final tax snapshot.
    /// </summary>
    /// <param name="request">Tax quote request payload.</param>
    /// <returns>Calculated tax quote result.</returns>
    Task<TaxQuoteResultDto> QuoteTaxAsync(TaxQuoteRequestDto request);

    /// <summary>
    /// Finalizes a tax calculation and stores immutable tax snapshot for audit.
    /// </summary>
    /// <param name="request">Tax finalize request payload.</param>
    /// <returns>Finalized tax result with snapshot identifier.</returns>
    Task<TaxQuoteResultDto> FinalizeTaxAsync(TaxQuoteRequestDto request);
}
