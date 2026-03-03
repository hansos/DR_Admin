using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for support ticket and support message operations.
/// </summary>
public class SupportTicketService : ISupportTicketService
{
    private static readonly Serilog.ILogger _log = Log.ForContext<SupportTicketService>();
    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase) { "Open", "InProgress", "WaitingForCustomer", "Closed" };
    private static readonly HashSet<string> AllowedPriorities = new(StringComparer.OrdinalIgnoreCase) { "Low", "Normal", "High" };

    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SupportTicketService"/> class.
    /// </summary>
    /// <param name="context">Application database context.</param>
    public SupportTicketService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<object> GetTicketsAsync(bool isSupportUser, int userId, int? customerId, string? status, int? pageNumber, int? pageSize)
    {
        var query = BuildScopedTicketQuery(isSupportUser, customerId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(t => t.Status == status);
        }

        query = query
            .OrderByDescending(t => t.LastMessageAt ?? t.UpdatedAt)
            .ThenByDescending(t => t.Id);

        _log.Information("Loading support tickets for user {UserId}. SupportUser={IsSupportUser}", userId, isSupportUser);

        if (pageNumber.HasValue || pageSize.HasValue)
        {
            var normalizedPageNumber = pageNumber.GetValueOrDefault(1);
            var normalizedPageSize = Math.Clamp(pageSize.GetValueOrDefault(25), 1, 200);
            var totalCount = await query.CountAsync();

            var pagedTickets = await query
                .Skip((normalizedPageNumber - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .ToListAsync();

            var ticketDtos = pagedTickets.Select(MapToDto).ToList();
            return new PagedResult<SupportTicketDto>(ticketDtos, totalCount, normalizedPageNumber, normalizedPageSize);
        }

        var tickets = await query.ToListAsync();
        return tickets.Select(MapToDto).ToList();
    }

    /// <inheritdoc />
    public async Task<SupportTicketDto?> GetTicketByIdAsync(int id, bool isSupportUser, int? customerId)
    {
        var ticket = await BuildScopedTicketQuery(isSupportUser, customerId)
            .FirstOrDefaultAsync(t => t.Id == id);

        return ticket == null ? null : MapToDto(ticket);
    }

    /// <inheritdoc />
    public async Task<SupportTicketDto> CreateTicketAsync(int userId, int customerId, CreateSupportTicketDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Subject))
        {
            throw new ArgumentException("Subject is required.", nameof(dto));
        }

        if (string.IsNullOrWhiteSpace(dto.Message))
        {
            throw new ArgumentException("Message is required.", nameof(dto));
        }

        var normalizedPriority = NormalizePriority(dto.Priority);
        var now = DateTime.UtcNow;

        var ticket = new SupportTicket
        {
            CustomerId = customerId,
            CreatedByUserId = userId,
            Subject = dto.Subject.Trim(),
            Priority = normalizedPriority,
            Status = "Open",
            LastMessageAt = now,
            CreatedAt = now,
            UpdatedAt = now,
            Messages =
            [
                new SupportTicketMessage
                {
                    SenderUserId = userId,
                    SenderRole = "Customer",
                    Message = dto.Message.Trim(),
                    CreatedAt = now,
                    UpdatedAt = now
                }
            ]
        };

        _context.SupportTickets.Add(ticket);
        await _context.SaveChangesAsync();

        var created = await BuildScopedTicketQuery(isSupportUser: true, customerId: null)
            .FirstAsync(t => t.Id == ticket.Id);

        _log.Information("Created support ticket {SupportTicketId} by user {UserId}", ticket.Id, userId);
        return MapToDto(created);
    }

    /// <inheritdoc />
    public async Task<SupportTicketDto?> AddMessageAsync(int ticketId, int userId, int? customerId, bool isSupportUser, CreateSupportTicketMessageDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Message))
        {
            throw new ArgumentException("Message is required.", nameof(dto));
        }

        var ticket = await _context.SupportTickets
            .Include(t => t.Messages)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
        {
            return null;
        }

        if (!isSupportUser && ticket.CustomerId != customerId)
        {
            return null;
        }

        if (ticket.Status.Equals("Closed", StringComparison.OrdinalIgnoreCase))
        {
            ticket.Status = "InProgress";
            ticket.ClosedAt = null;
        }

        var now = DateTime.UtcNow;
        ticket.Messages.Add(new SupportTicketMessage
        {
            SupportTicketId = ticket.Id,
            SenderUserId = userId,
            SenderRole = isSupportUser ? "Support" : "Customer",
            Message = dto.Message.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        });

        ticket.LastMessageAt = now;
        ticket.UpdatedAt = now;

        if (isSupportUser)
        {
            ticket.AssignedToUserId ??= userId;
            if (ticket.Status.Equals("Open", StringComparison.OrdinalIgnoreCase))
            {
                ticket.Status = "InProgress";
            }
        }
        else if (ticket.Status.Equals("WaitingForCustomer", StringComparison.OrdinalIgnoreCase))
        {
            ticket.Status = "InProgress";
        }

        await _context.SaveChangesAsync();

        var updated = await BuildScopedTicketQuery(isSupportUser: true, customerId: null)
            .FirstAsync(t => t.Id == ticket.Id);

        return MapToDto(updated);
    }

    /// <inheritdoc />
    public async Task<SupportTicketDto?> UpdateStatusAsync(int ticketId, string status, int? assignedToUserId)
    {
        if (!AllowedStatuses.Contains(status))
        {
            throw new ArgumentException("Invalid support ticket status.", nameof(status));
        }

        var ticket = await _context.SupportTickets
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
        {
            return null;
        }

        ticket.Status = status;
        ticket.AssignedToUserId = assignedToUserId;
        ticket.ClosedAt = status.Equals("Closed", StringComparison.OrdinalIgnoreCase) ? DateTime.UtcNow : null;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var updated = await BuildScopedTicketQuery(isSupportUser: true, customerId: null)
            .FirstAsync(t => t.Id == ticket.Id);

        return MapToDto(updated);
    }

    private IQueryable<SupportTicket> BuildScopedTicketQuery(bool isSupportUser, int? customerId)
    {
        var scopedQuery = _context.SupportTickets
            .AsNoTracking()
            .Include(t => t.Customer)
            .Include(t => t.CreatedByUser)
            .Include(t => t.AssignedToUser)
            .Include(t => t.Messages.OrderBy(m => m.CreatedAt))
                .ThenInclude(m => m.SenderUser)
            .AsQueryable();

        if (!isSupportUser)
        {
            scopedQuery = scopedQuery.Where(t => t.CustomerId == customerId);
        }

        return scopedQuery;
    }

    private static string NormalizePriority(string? priority)
    {
        if (string.IsNullOrWhiteSpace(priority))
        {
            return "Normal";
        }

        if (AllowedPriorities.Contains(priority))
        {
            if (priority.Equals("Low", StringComparison.OrdinalIgnoreCase))
            {
                return "Low";
            }

            if (priority.Equals("High", StringComparison.OrdinalIgnoreCase))
            {
                return "High";
            }
        }

        return "Normal";
    }

    private static SupportTicketDto MapToDto(SupportTicket ticket)
    {
        return new SupportTicketDto
        {
            Id = ticket.Id,
            CustomerId = ticket.CustomerId,
            CustomerName = ticket.Customer?.Name ?? string.Empty,
            CreatedByUserId = ticket.CreatedByUserId,
            CreatedByUsername = ticket.CreatedByUser?.Username ?? string.Empty,
            AssignedToUserId = ticket.AssignedToUserId,
            AssignedToUsername = ticket.AssignedToUser?.Username,
            Subject = ticket.Subject,
            Status = ticket.Status,
            Priority = ticket.Priority,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            LastMessageAt = ticket.LastMessageAt,
            ClosedAt = ticket.ClosedAt,
            Messages = ticket.Messages
                .OrderBy(m => m.CreatedAt)
                .Select(m => new SupportTicketMessageDto
                {
                    Id = m.Id,
                    SupportTicketId = m.SupportTicketId,
                    SenderUserId = m.SenderUserId,
                    SenderUsername = m.SenderUser?.Username ?? string.Empty,
                    SenderRole = m.SenderRole,
                    Message = m.Message,
                    CreatedAt = m.CreatedAt
                })
                .ToList()
        };
    }
}
