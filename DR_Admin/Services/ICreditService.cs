using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing customer credits
/// </summary>
public interface ICreditService
{
    /// <summary>
    /// Retrieves customer credit by customer ID
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>The customer credit DTO if found, otherwise null</returns>
    Task<CustomerCreditDto?> GetCustomerCreditAsync(int customerId);

    /// <summary>
    /// Retrieves credit transactions for a customer
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>A collection of credit transaction DTOs</returns>
    Task<IEnumerable<CreditTransactionDto>> GetCreditTransactionsAsync(int customerId);

    /// <summary>
    /// Creates a credit transaction
    /// </summary>
    /// <param name="createDto">The credit transaction creation data</param>
    /// <param name="userId">The ID of the user creating the transaction</param>
    /// <returns>The created credit transaction DTO</returns>
    Task<CreditTransactionDto> CreateCreditTransactionAsync(CreateCreditTransactionDto createDto, int userId);

    /// <summary>
    /// Adds credit to a customer account
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="amount">The amount to add</param>
    /// <param name="description">The description</param>
    /// <param name="userId">The ID of the user adding credit</param>
    /// <returns>The new credit balance</returns>
    Task<decimal> AddCreditAsync(int customerId, decimal amount, string description, int userId);

    /// <summary>
    /// Deducts credit from a customer account
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="amount">The amount to deduct</param>
    /// <param name="invoiceId">The invoice ID (if applicable)</param>
    /// <param name="description">The description</param>
    /// <param name="userId">The ID of the user deducting credit</param>
    /// <returns>The new credit balance</returns>
    Task<decimal> DeductCreditAsync(int customerId, decimal amount, int? invoiceId, string description, int userId);

    /// <summary>
    /// Checks if a customer has sufficient credit
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="amount">The required amount</param>
    /// <returns>True if sufficient credit, otherwise false</returns>
    Task<bool> HasSufficientCreditAsync(int customerId, decimal amount);
}
