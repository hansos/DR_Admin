using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for support ticket management.
/// </summary>
public interface ISupportTicketService
{
    /// <summary>
    /// Retrieves support tickets with optional filters and optional pagination.
    /// </summary>
    /// <param name="isSupportUser">Indicates whether the caller is support staff.</param>
    /// <param name="userId">The current authenticated user identifier.</param>
    /// <param name="customerId">The customer identifier linked to the current user, if any.</param>
    /// <param name="status">Optional ticket status filter.</param>
    /// <param name="pageNumber">Optional page number (1-based).</param>
    /// <param name="pageSize">Optional page size.</param>
    /// <returns>Either a list of tickets or a paged ticket result.</returns>
    Task<object> GetTicketsAsync(bool isSupportUser, int userId, int? customerId, string? status, int? pageNumber, int? pageSize);

    /// <summary>
    /// Retrieves a support ticket by identifier.
    /// </summary>
    /// <param name="id">Support ticket identifier.</param>
    /// <param name="isSupportUser">Indicates whether the caller is support staff.</param>
    /// <param name="customerId">The customer identifier linked to the current user, if any.</param>
    /// <returns>The support ticket, or null when not found or unauthorized.</returns>
    Task<SupportTicketDto?> GetTicketByIdAsync(int id, bool isSupportUser, int? customerId);

    /// <summary>
    /// Creates a support ticket with an initial customer message.
    /// </summary>
    /// <param name="userId">The current authenticated user identifier.</param>
    /// <param name="customerId">The customer identifier linked to the ticket.</param>
    /// <param name="isSupportUser">Indicates whether the creator is support staff.</param>
    /// <param name="dto">Ticket creation payload.</param>
    /// <returns>The created support ticket.</returns>
    Task<SupportTicketDto> CreateTicketAsync(int userId, int customerId, bool isSupportUser, CreateSupportTicketDto dto);

    /// <summary>
    /// Updates ticket status.
    /// </summary>
    /// <param name="ticketId">Support ticket identifier.</param>
    /// <param name="status">New ticket status.</param>
    /// <param name="assignedDepartment">Optional assigned support department.</param>
    /// <param name="assignedToUserId">Optional assigned support user identifier.</param>
    /// <returns>The updated support ticket, or null when not found.</returns>
    Task<SupportTicketDto?> UpdateStatusAsync(int ticketId, string status, string? assignedDepartment, int? assignedToUserId);
}
