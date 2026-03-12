using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Defines read operations for payment transaction list data.
/// </summary>
public interface IPaymentTransactionService
{
    /// <summary>
    /// Retrieves all payment transactions with invoice and allocation details.
    /// </summary>
    /// <returns>A collection of payment transaction list items.</returns>
    Task<IEnumerable<PaymentTransactionListDto>> GetAllAsync();
}
