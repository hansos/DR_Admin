using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing refunds
/// </summary>
public interface IRefundService
{
    /// <summary>
    /// Retrieves all refunds
    /// </summary>
    /// <returns>A collection of refund DTOs</returns>
    Task<IEnumerable<RefundDto>> GetAllRefundsAsync();

    /// <summary>
    /// Retrieves a refund by ID
    /// </summary>
    /// <param name="id">The refund ID</param>
    /// <returns>The refund DTO if found, otherwise null</returns>
    Task<RefundDto?> GetRefundByIdAsync(int id);

    /// <summary>
    /// Retrieves refunds by invoice ID
    /// </summary>
    /// <param name="invoiceId">The invoice ID</param>
    /// <returns>A collection of refund DTOs</returns>
    Task<IEnumerable<RefundDto>> GetRefundsByInvoiceIdAsync(int invoiceId);

    /// <summary>
    /// Creates a new refund
    /// </summary>
    /// <param name="createDto">The refund creation data</param>
    /// <param name="userId">The ID of the user creating the refund</param>
    /// <returns>The created refund DTO</returns>
    Task<RefundDto> CreateRefundAsync(CreateRefundDto createDto, int userId);

    /// <summary>
    /// Processes a pending refund
    /// </summary>
    /// <param name="id">The refund ID</param>
    /// <returns>True if successful, otherwise false</returns>
    Task<bool> ProcessRefundAsync(int id);
}
