using EmailReceiverLib.Models;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for persisting inbound emails to communication entities.
/// </summary>
public class CommunicationIngestionService : ICommunicationIngestionService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<CommunicationIngestionService>();

    /// <summary>
    /// Initializes a new instance of the <see cref="CommunicationIngestionService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CommunicationIngestionService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Persists inbound email messages to communication threads and messages.
    /// </summary>
    /// <param name="messages">Inbound messages to persist.</param>
    /// <returns>The number of newly persisted inbound messages.</returns>
    public async Task<int> PersistInboundMessagesAsync(IEnumerable<InboundEmailMessage> messages)
    {
        var messageList = messages?.ToList() ?? [];
        if (messageList.Count == 0)
        {
            return 0;
        }

        var createdCount = 0;

        foreach (var inbound in messageList)
        {
            if (string.IsNullOrWhiteSpace(inbound.ExternalMessageId))
            {
                continue;
            }

            var exists = await _context.CommunicationMessages
                .AsNoTracking()
                .AnyAsync(m => m.Direction == CommunicationMessageDirection.Inbound
                    && (m.ExternalMessageId == inbound.ExternalMessageId
                        || (inbound.InternetMessageId != null && m.InternetMessageId == inbound.InternetMessageId)));

            if (exists)
            {
                continue;
            }

            var now = DateTime.UtcNow;
            var subject = string.IsNullOrWhiteSpace(inbound.Subject) ? "(No subject)" : inbound.Subject.Trim();
            var normalizedSubject = NormalizeThreadSubject(subject);
            var fromAddress = inbound.FromAddress?.Trim() ?? string.Empty;
            var toAddresses = string.Join(";", inbound.ToAddresses.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => a.Trim()));

            var participantAddresses = inbound.ToAddresses
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(a => a.Trim())
                .Append(fromAddress)
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var candidateThreadIds = await _context.CommunicationParticipants
                .AsNoTracking()
                .Where(p => participantAddresses.Contains(p.EmailAddress))
                .Select(p => p.CommunicationThreadId)
                .Distinct()
                .ToListAsync();

            var threadCandidates = await _context.CommunicationThreads
                .Where(t => t.Status == CommunicationThreadStatus.Open
                    && (candidateThreadIds.Count == 0 || candidateThreadIds.Contains(t.Id)))
                .OrderByDescending(t => t.LastMessageAtUtc ?? t.CreatedAt)
                .Take(200)
                .ToListAsync();

            var thread = threadCandidates
                .FirstOrDefault(t => string.Equals(
                    NormalizeThreadSubject(t.Subject),
                    normalizedSubject,
                    StringComparison.OrdinalIgnoreCase));

            if (thread == null)
            {
                thread = new CommunicationThread
                {
                    Subject = subject,
                    Status = CommunicationThreadStatus.Open,
                    LastMessageAtUtc = inbound.ReceivedAtUtc ?? now,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                _context.CommunicationThreads.Add(thread);
                await _context.SaveChangesAsync();
            }
            else
            {
                thread.LastMessageAtUtc = inbound.ReceivedAtUtc ?? now;
                thread.UpdatedAt = now;
            }

            var message = new CommunicationMessage
            {
                CommunicationThreadId = thread.Id,
                Direction = CommunicationMessageDirection.Inbound,
                ExternalMessageId = inbound.ExternalMessageId,
                InternetMessageId = inbound.InternetMessageId,
                FromAddress = fromAddress,
                ToAddresses = toAddresses,
                Subject = subject,
                BodyText = inbound.BodyPreview,
                BodyHtml = null,
                IsRead = inbound.IsRead,
                ReceivedAtUtc = inbound.ReceivedAtUtc,
                ReadAtUtc = inbound.IsRead ? (inbound.ReceivedAtUtc ?? now) : null,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.CommunicationMessages.Add(message);

            await EnsureParticipantAsync(thread.Id, fromAddress, CommunicationParticipantRole.From, true, now);
            foreach (var toAddress in inbound.ToAddresses.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => a.Trim()))
            {
                await EnsureParticipantAsync(thread.Id, toAddress, CommunicationParticipantRole.To, false, now);
            }

            _context.CommunicationStatusEvents.Add(new CommunicationStatusEvent
            {
                CommunicationMessage = message,
                Status = "Received",
                Details = "Inbound email received from mailbox provider.",
                Source = nameof(CommunicationIngestionService),
                OccurredAtUtc = inbound.ReceivedAtUtc ?? now,
                CreatedAt = now,
                UpdatedAt = now
            });

            await _context.SaveChangesAsync();
            createdCount++;
        }

        _log.Information("Persisted {CreatedCount} inbound communication message(s)", createdCount);
        return createdCount;
    }

    private async Task EnsureParticipantAsync(int threadId, string? emailAddress, string role, bool isPrimary, DateTime now)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
        {
            return;
        }

        var normalized = emailAddress.Trim();
        var exists = await _context.CommunicationParticipants
            .AsNoTracking()
            .AnyAsync(p => p.CommunicationThreadId == threadId
                && p.Role == role
                && p.EmailAddress.ToLower() == normalized.ToLower());

        if (exists)
        {
            return;
        }

        _context.CommunicationParticipants.Add(new CommunicationParticipant
        {
            CommunicationThreadId = threadId,
            EmailAddress = normalized,
            Role = role,
            IsPrimary = isPrimary,
            CreatedAt = now,
            UpdatedAt = now
        });
    }

    private static string NormalizeThreadSubject(string? subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            return "(no subject)";
        }

        var value = subject.Trim();
        var changed = true;
        while (changed)
        {
            changed = false;
            if (value.StartsWith("Re:", StringComparison.OrdinalIgnoreCase))
            {
                value = value[3..].TrimStart();
                changed = true;
            }
            else if (value.StartsWith("Fw:", StringComparison.OrdinalIgnoreCase))
            {
                value = value[3..].TrimStart();
                changed = true;
            }
            else if (value.StartsWith("Fwd:", StringComparison.OrdinalIgnoreCase))
            {
                value = value[4..].TrimStart();
                changed = true;
            }
        }

        return value.ToLowerInvariant();
    }
}
