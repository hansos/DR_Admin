using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing quotes
/// </summary>
public interface IQuoteService
{
    /// <summary>
    /// Retrieves all quotes
    /// </summary>
    /// <returns>A collection of quote DTOs</returns>
    Task<IEnumerable<QuoteDto>> GetAllQuotesAsync();

    /// <summary>
    /// Retrieves a quote by ID
    /// </summary>
    /// <param name="id">The quote ID</param>
    /// <returns>The quote DTO if found, otherwise null</returns>
    Task<QuoteDto?> GetQuoteByIdAsync(int id);

    /// <summary>
    /// Retrieves quotes by customer ID
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>A collection of quote DTOs</returns>
    Task<IEnumerable<QuoteDto>> GetQuotesByCustomerIdAsync(int customerId);

    /// <summary>
    /// Retrieves quotes by status
    /// </summary>
    /// <param name="status">The quote status</param>
    /// <returns>A collection of quote DTOs</returns>
    Task<IEnumerable<QuoteDto>> GetQuotesByStatusAsync(QuoteStatus status);

    /// <summary>
    /// Creates a new quote
    /// </summary>
    /// <param name="createDto">The quote creation data</param>
    /// <param name="userId">The ID of the user creating the quote</param>
    /// <returns>The created quote DTO</returns>
    Task<QuoteDto> CreateQuoteAsync(CreateQuoteDto createDto, int userId);

    /// <summary>
    /// Updates an existing quote
    /// </summary>
    /// <param name="id">The quote ID</param>
    /// <param name="updateDto">The quote update data</param>
    /// <returns>The updated quote DTO if successful, otherwise null</returns>
    Task<QuoteDto?> UpdateQuoteAsync(int id, UpdateQuoteDto updateDto);

    /// <summary>
    /// Deletes a quote (soft delete)
    /// </summary>
    /// <param name="id">The quote ID</param>
    /// <returns>True if successful, otherwise false</returns>
    Task<bool> DeleteQuoteAsync(int id);

    /// <summary>
    /// Sends a quote to the customer
    /// </summary>
    /// <param name="id">The quote ID</param>
    /// <returns>True if successful, otherwise false</returns>
    Task<bool> SendQuoteAsync(int id);

    /// <summary>
    /// Accepts a quote using the acceptance token
    /// </summary>
    /// <param name="token">The acceptance token</param>
    /// <returns>True if successful, otherwise false</returns>
    Task<bool> AcceptQuoteAsync(string token);

    /// <summary>
    /// Rejects a quote
    /// </summary>
    /// <param name="id">The quote ID</param>
    /// <param name="reason">The rejection reason</param>
    /// <returns>True if successful, otherwise false</returns>
    Task<bool> RejectQuoteAsync(int id, string reason);

    /// <summary>
    /// Converts a quote to an order
    /// </summary>
    /// <param name="id">The quote ID</param>
    /// <returns>The created order ID if successful, otherwise null</returns>
    Task<int?> ConvertQuoteToOrderAsync(int id);

    /// <summary>
    /// Generates a PDF for the quote
    /// </summary>
    /// <param name="id">The quote ID</param>
    /// <returns>The PDF as byte array</returns>
    Task<byte[]> GenerateQuotePdfAsync(int id);
}
