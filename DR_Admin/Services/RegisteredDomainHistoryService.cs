using ISPAdmin.Data;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for querying registered domain history entries.
/// </summary>
public class RegisteredDomainHistoryService : IRegisteredDomainHistoryService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegisteredDomainHistoryService>();

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisteredDomainHistoryService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public RegisteredDomainHistoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all history entries for a specific registered domain.
    /// </summary>
    /// <param name="registeredDomainId">The registered domain identifier.</param>
    /// <returns>A collection of history entries ordered by occurrence date descending.</returns>
    public async Task<IEnumerable<RegisteredDomainHistoryDto>> GetByRegisteredDomainIdAsync(int registeredDomainId)
    {
        _log.Information("Fetching registered domain history for domain ID: {RegisteredDomainId}", registeredDomainId);

        var items = await _context.RegisteredDomainHistories
            .AsNoTracking()
            .Where(x => x.RegisteredDomainId == registeredDomainId)
            .OrderByDescending(x => x.OccurredAt)
            .ThenByDescending(x => x.Id)
            .Select(x => new RegisteredDomainHistoryDto
            {
                Id = x.Id,
                RegisteredDomainId = x.RegisteredDomainId,
                DomainName = x.RegisteredDomain.Name,
                ActionType = x.ActionType,
                Action = x.Action,
                Details = x.Details,
                OccurredAt = x.OccurredAt,
                SourceEntityType = x.SourceEntityType,
                SourceEntityId = x.SourceEntityId,
                PerformedByUserId = x.PerformedByUserId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();

        _log.Information("Fetched {Count} registered domain history entries for domain ID: {RegisteredDomainId}", items.Count, registeredDomainId);
        return items;
    }

    /// <summary>
    /// Retrieves a registered domain history entry by identifier.
    /// </summary>
    /// <param name="id">The history entry identifier.</param>
    /// <returns>The history entry if found; otherwise <see langword="null"/>.</returns>
    public async Task<RegisteredDomainHistoryDto?> GetByIdAsync(int id)
    {
        _log.Information("Fetching registered domain history by ID: {HistoryId}", id);

        var item = await _context.RegisteredDomainHistories
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new RegisteredDomainHistoryDto
            {
                Id = x.Id,
                RegisteredDomainId = x.RegisteredDomainId,
                DomainName = x.RegisteredDomain.Name,
                ActionType = x.ActionType,
                Action = x.Action,
                Details = x.Details,
                OccurredAt = x.OccurredAt,
                SourceEntityType = x.SourceEntityType,
                SourceEntityId = x.SourceEntityId,
                PerformedByUserId = x.PerformedByUserId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .FirstOrDefaultAsync();

        return item;
    }

    /// <summary>
    /// Retrieves DNS change history entries across all domains with optional filters.
    /// </summary>
    /// <param name="domainName">Optional domain name search filter.</param>
    /// <param name="occurredFrom">Optional lower bound for occurrence timestamp (UTC).</param>
    /// <param name="occurredTo">Optional upper bound for occurrence timestamp (UTC).</param>
    /// <returns>A collection of DNS change history entries ordered by occurrence date descending.</returns>
    public async Task<IEnumerable<RegisteredDomainHistoryDto>> GetDnsChangesAsync(string? domainName, DateTime? occurredFrom, DateTime? occurredTo)
    {
        _log.Information("Fetching DNS change history with filters DomainName={DomainName}, OccurredFrom={OccurredFrom}, OccurredTo={OccurredTo}",
            domainName,
            occurredFrom,
            occurredTo);

        var query = _context.RegisteredDomainHistories
            .AsNoTracking()
            .Where(x => x.ActionType == RegisteredDomainHistoryActionType.DnsChange);

        if (!string.IsNullOrWhiteSpace(domainName))
        {
            var normalizedDomainName = domainName.Trim().ToLower();
            query = query.Where(x => x.RegisteredDomain.Name.ToLower().Contains(normalizedDomainName));
        }

        if (occurredFrom.HasValue)
        {
            query = query.Where(x => x.OccurredAt >= occurredFrom.Value);
        }

        if (occurredTo.HasValue)
        {
            query = query.Where(x => x.OccurredAt <= occurredTo.Value);
        }

        var items = await query
            .OrderByDescending(x => x.OccurredAt)
            .ThenByDescending(x => x.Id)
            .Select(x => new RegisteredDomainHistoryDto
            {
                Id = x.Id,
                RegisteredDomainId = x.RegisteredDomainId,
                DomainName = x.RegisteredDomain.Name,
                ActionType = x.ActionType,
                Action = x.Action,
                Details = x.Details,
                OccurredAt = x.OccurredAt,
                SourceEntityType = x.SourceEntityType,
                SourceEntityId = x.SourceEntityId,
                PerformedByUserId = x.PerformedByUserId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();

        _log.Information("Fetched {Count} DNS change history entries", items.Count);
        return items;
    }
}
