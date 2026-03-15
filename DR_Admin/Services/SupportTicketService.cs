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
    private static readonly HashSet<string> AllowedSources = new(StringComparer.OrdinalIgnoreCase)
    {
        SupportTicketSource.CustomerWeb,
        SupportTicketSource.Phone,
        SupportTicketSource.InPerson,
        SupportTicketSource.Email
    };

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
    public async Task<SupportTicketDto> CreateTicketAsync(int userId, int customerId, bool isSupportUser, CreateSupportTicketDto dto)
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
        var normalizedSource = NormalizeSource(dto.Source, isSupportUser);
        var now = DateTime.UtcNow;

        var ticket = new SupportTicket
        {
            CustomerId = customerId,
            CreatedByUserId = userId,
            Subject = dto.Subject.Trim(),
            Description = dto.Message.Trim(),
            Priority = normalizedPriority,
            Source = normalizedSource,
            Status = "Open",
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.SupportTickets.Add(ticket);
        await _context.SaveChangesAsync();

        var created = await BuildScopedTicketQuery(isSupportUser: true, customerId: null)
            .FirstAsync(t => t.Id == ticket.Id);

        _log.Information("Created support ticket {SupportTicketId} by user {UserId}", ticket.Id, userId);
        return MapToDto(created);
    }

    /// <inheritdoc />
    public async Task<SupportTicketDto?> UpdateStatusAsync(int ticketId, string status, string? assignedDepartment, int? assignedToUserId)
    {
        var normalizedStatus = NormalizeStatus(status);

        if (!AllowedStatuses.Contains(normalizedStatus))
        {
            throw new ArgumentException("Invalid support ticket status.", nameof(status));
        }

        var ticket = await _context.SupportTickets
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
        {
            return null;
        }

        var normalizedDepartment = NormalizeDepartment(assignedDepartment) ?? ticket.AssignedDepartment;
        var effectiveAssignedToUserId = assignedToUserId ?? ticket.AssignedToUserId;

        if (normalizedStatus.Equals("InProgress", StringComparison.OrdinalIgnoreCase)
            || normalizedStatus.Equals("WaitingForCustomer", StringComparison.OrdinalIgnoreCase)
            || normalizedStatus.Equals("Closed", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(normalizedDepartment))
            {
                throw new ArgumentException("AssignedDepartment is required when moving ticket beyond Open.", nameof(assignedDepartment));
            }

            if (!effectiveAssignedToUserId.HasValue)
            {
                throw new ArgumentException("AssignedToUserId is required when moving ticket beyond Open.", nameof(assignedToUserId));
            }
        }

        ticket.Status = normalizedStatus;
        ticket.AssignedDepartment = normalizedDepartment;
        ticket.AssignedToUserId = effectiveAssignedToUserId;
        ticket.ClosedAt = normalizedStatus.Equals("Closed", StringComparison.OrdinalIgnoreCase) ? DateTime.UtcNow : null;
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

    private static string NormalizeSource(string? source, bool isSupportUser)
    {
        if (!isSupportUser)
        {
            return SupportTicketSource.CustomerWeb;
        }

        if (string.IsNullOrWhiteSpace(source))
        {
            return SupportTicketSource.CustomerWeb;
        }

        if (AllowedSources.Contains(source))
        {
            if (source.Equals(SupportTicketSource.CustomerWeb, StringComparison.OrdinalIgnoreCase))
            {
                return SupportTicketSource.CustomerWeb;
            }

            if (source.Equals(SupportTicketSource.Phone, StringComparison.OrdinalIgnoreCase))
            {
                return SupportTicketSource.Phone;
            }

            if (source.Equals(SupportTicketSource.InPerson, StringComparison.OrdinalIgnoreCase))
            {
                return SupportTicketSource.InPerson;
            }

            if (source.Equals(SupportTicketSource.Email, StringComparison.OrdinalIgnoreCase))
            {
                return SupportTicketSource.Email;
            }
        }

        throw new ArgumentException("Invalid support ticket source.", nameof(source));
    }

    private static string NormalizeStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return string.Empty;
        }

        if (status.Equals("Open", StringComparison.OrdinalIgnoreCase))
        {
            return "Open";
        }

        if (status.Equals("InProgress", StringComparison.OrdinalIgnoreCase))
        {
            return "InProgress";
        }

        if (status.Equals("WaitingForCustomer", StringComparison.OrdinalIgnoreCase))
        {
            return "WaitingForCustomer";
        }

        if (status.Equals("Closed", StringComparison.OrdinalIgnoreCase))
        {
            return "Closed";
        }

        return status.Trim();
    }

    private static string? NormalizeDepartment(string? department)
    {
        return string.IsNullOrWhiteSpace(department) ? null : department.Trim();
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
            AssignedDepartment = ticket.AssignedDepartment,
            AssignedToUsername = ticket.AssignedToUser?.Username,
            Subject = ticket.Subject,
            Description = ticket.Description,
            Status = ticket.Status,
            Priority = ticket.Priority,
            Source = ticket.Source,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            LastMessageAt = ticket.LastMessageAt,
            ClosedAt = ticket.ClosedAt,
            Messages = []
        };
    }
}
