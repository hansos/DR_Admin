using ISPAdmin.Data;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for querying communication threads.
/// </summary>
public class CommunicationThreadService : ICommunicationThreadService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<CommunicationThreadService>();

    /// <summary>
    /// Initializes a new instance of the <see cref="CommunicationThreadService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CommunicationThreadService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves communication threads with optional filters.
    /// </summary>
    /// <param name="customerId">Optional customer identifier filter.</param>
    /// <param name="userId">Optional user identifier filter.</param>
    /// <param name="relatedEntityType">Optional related entity type filter.</param>
    /// <param name="relatedEntityId">Optional related entity identifier filter.</param>
    /// <param name="status">Optional thread status filter.</param>
    /// <param name="search">Optional free-text search over subject and participants.</param>
    /// <param name="maxItems">Maximum number of items to return.</param>
    /// <returns>A collection of communication thread summaries.</returns>
    public async Task<IEnumerable<CommunicationThreadDto>> GetThreadsAsync(
        int? customerId,
        int? userId,
        string? relatedEntityType,
        int? relatedEntityId,
        string? status,
        string? search,
        int maxItems)
    {
        var effectiveMaxItems = maxItems <= 0 ? 50 : Math.Min(maxItems, 500);

        _log.Information(
            "Fetching communication threads with filters CustomerId={CustomerId}, UserId={UserId}, RelatedEntityType={RelatedEntityType}, RelatedEntityId={RelatedEntityId}, Status={Status}, Search={Search}, MaxItems={MaxItems}",
            customerId,
            userId,
            relatedEntityType,
            relatedEntityId,
            status,
            search,
            effectiveMaxItems);

        var query = _context.CommunicationThreads
            .AsNoTracking()
            .AsQueryable();

        if (customerId.HasValue)
        {
            query = query.Where(x => x.CustomerId == customerId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(relatedEntityType))
        {
            var normalizedEntityType = relatedEntityType.Trim().ToLower();
            query = query.Where(x => x.RelatedEntityType != null && x.RelatedEntityType.ToLower() == normalizedEntityType);
        }

        if (relatedEntityId.HasValue)
        {
            query = query.Where(x => x.RelatedEntityId == relatedEntityId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToLower();
            query = query.Where(x => x.Status.ToLower() == normalizedStatus);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();
            query = query.Where(x =>
                x.Subject.ToLower().Contains(normalizedSearch)
                || x.Participants.Any(p => p.EmailAddress.ToLower().Contains(normalizedSearch))
                || x.Participants.Any(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(normalizedSearch)));
        }

        var items = await query
            .OrderByDescending(x => x.LastMessageAtUtc ?? x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Take(effectiveMaxItems)
            .Select(x => new CommunicationThreadDto
            {
                Id = x.Id,
                Subject = x.Subject,
                CustomerId = x.CustomerId,
                UserId = x.UserId,
                RelatedEntityType = x.RelatedEntityType,
                RelatedEntityId = x.RelatedEntityId,
                LastMessageAtUtc = x.LastMessageAtUtc,
                Status = x.Status,
                UnreadCount = x.Messages.Count(m => !m.IsRead),
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();

        _log.Information("Fetched {Count} communication threads", items.Count);
        return items;
    }

    /// <summary>
    /// Retrieves a communication thread with participants and messages.
    /// </summary>
    /// <param name="threadId">The communication thread identifier.</param>
    /// <returns>The thread details when found; otherwise <see langword="null"/>.</returns>
    public async Task<CommunicationThreadDetailsDto?> GetThreadByIdAsync(int threadId)
    {
        _log.Information("Fetching communication thread details for ThreadId={ThreadId}", threadId);

        var thread = await _context.CommunicationThreads
            .AsNoTracking()
            .Where(x => x.Id == threadId)
            .Select(x => new CommunicationThreadDetailsDto
            {
                Id = x.Id,
                Subject = x.Subject,
                CustomerId = x.CustomerId,
                UserId = x.UserId,
                RelatedEntityType = x.RelatedEntityType,
                RelatedEntityId = x.RelatedEntityId,
                LastMessageAtUtc = x.LastMessageAtUtc,
                Status = x.Status,
                UnreadCount = x.Messages.Count(m => !m.IsRead),
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Participants = x.Participants
                    .OrderBy(p => p.Role)
                    .ThenBy(p => p.EmailAddress)
                    .Select(p => new CommunicationParticipantDto
                    {
                        EmailAddress = p.EmailAddress,
                        DisplayName = p.DisplayName,
                        Role = p.Role,
                        IsPrimary = p.IsPrimary
                    })
                    .ToList(),
                Messages = x.Messages
                    .OrderBy(m => m.SentAtUtc ?? m.ReceivedAtUtc ?? m.CreatedAt)
                    .ThenBy(m => m.Id)
                    .Select(m => new CommunicationMessageDto
                    {
                        Id = m.Id,
                        Direction = m.Direction,
                        ExternalMessageId = m.ExternalMessageId,
                        FromAddress = m.FromAddress,
                        ToAddresses = m.ToAddresses,
                        CcAddresses = m.CcAddresses,
                        BccAddresses = m.BccAddresses,
                        Subject = m.Subject,
                        BodyText = m.BodyText,
                        BodyHtml = m.BodyHtml,
                        Provider = m.Provider,
                        IsRead = m.IsRead,
                        ReceivedAtUtc = m.ReceivedAtUtc,
                        SentAtUtc = m.SentAtUtc,
                        ReadAtUtc = m.ReadAtUtc,
                        CreatedAt = m.CreatedAt,
                        UpdatedAt = m.UpdatedAt
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        return thread;
    }

    /// <summary>
    /// Updates the status of a communication thread.
    /// </summary>
    /// <param name="threadId">The communication thread identifier.</param>
    /// <param name="status">The target thread status.</param>
    /// <returns><see langword="true"/> if the thread was updated; otherwise <see langword="false"/>.</returns>
    public async Task<bool> UpdateThreadStatusAsync(int threadId, string status)
    {
        var normalizedStatus = status.Trim();

        var thread = await _context.CommunicationThreads
            .FirstOrDefaultAsync(x => x.Id == threadId);

        if (thread == null)
        {
            return false;
        }

        thread.Status = normalizedStatus;
        thread.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Updates the read state of a communication message.
    /// </summary>
    /// <param name="messageId">The communication message identifier.</param>
    /// <param name="isRead">The target read state.</param>
    /// <returns><see langword="true"/> if the message was updated; otherwise <see langword="false"/>.</returns>
    public async Task<bool> UpdateMessageReadStateAsync(int messageId, bool isRead)
    {
        var message = await _context.CommunicationMessages
            .FirstOrDefaultAsync(x => x.Id == messageId);

        if (message == null)
        {
            return false;
        }

        message.IsRead = isRead;
        message.ReadAtUtc = isRead ? DateTime.UtcNow : null;
        message.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Determines whether a communication message belongs to a thread accessible by customer/user scope.
    /// </summary>
    /// <param name="messageId">The communication message identifier.</param>
    /// <param name="customerId">The scoped customer identifier.</param>
    /// <param name="userId">The scoped user identifier.</param>
    /// <returns><see langword="true"/> when accessible in scope; otherwise <see langword="false"/>.</returns>
    public async Task<bool> CanAccessMessageAsync(int messageId, int customerId, int userId)
    {
        return await _context.CommunicationMessages
            .AsNoTracking()
            .Where(m => m.Id == messageId)
            .AnyAsync(m => m.CommunicationThread != null
                && (m.CommunicationThread.CustomerId == customerId || m.CommunicationThread.UserId == userId));
    }
}
