using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for TLD registry policy rules.
/// </summary>
public class TldRegistryRuleService : ITldRegistryRuleService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<TldRegistryRuleService>();

    /// <summary>
    /// Initializes a new instance of the <see cref="TldRegistryRuleService"/> class.
    /// </summary>
    /// <param name="context">Application database context.</param>
    public TldRegistryRuleService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TldRegistryRuleDto>> GetAllAsync()
    {
        try
        {
            var items = await _context.TldRegistryRules
                .AsNoTracking()
                .Include(x => x.Tld)
                .OrderBy(x => x.Tld.Extension)
                .ThenByDescending(x => x.IsActive)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto);
        }
        catch (SqliteException ex) when (IsMissingTableError(ex))
        {
            _log.Warning(ex, "TldRegistryRules table not found. Returning empty list.");
            return [];
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TldRegistryRuleDto>> GetActiveAsync()
    {
        try
        {
            var items = await _context.TldRegistryRules
                .AsNoTracking()
                .Include(x => x.Tld)
                .Where(x => x.IsActive)
                .OrderBy(x => x.Tld.Extension)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto);
        }
        catch (SqliteException ex) when (IsMissingTableError(ex))
        {
            _log.Warning(ex, "TldRegistryRules table not found. Returning empty active list.");
            return [];
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TldRegistryRuleDto>> GetByTldIdAsync(int tldId)
    {
        try
        {
            var items = await _context.TldRegistryRules
                .AsNoTracking()
                .Include(x => x.Tld)
                .Where(x => x.TldId == tldId)
                .OrderByDescending(x => x.IsActive)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return items.Select(MapToDto);
        }
        catch (SqliteException ex) when (IsMissingTableError(ex))
        {
            _log.Warning(ex, "TldRegistryRules table not found. Returning empty list for TLD {TldId}.", tldId);
            return [];
        }
    }

    /// <inheritdoc />
    public async Task<TldRegistryRuleDto?> GetByIdAsync(int id)
    {
        try
        {
            var item = await _context.TldRegistryRules
                .AsNoTracking()
                .Include(x => x.Tld)
                .FirstOrDefaultAsync(x => x.Id == id);

            return item == null ? null : MapToDto(item);
        }
        catch (SqliteException ex) when (IsMissingTableError(ex))
        {
            _log.Warning(ex, "TldRegistryRules table not found while loading rule {RuleId}.", id);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<TldRegistryRuleDto> CreateAsync(CreateTldRegistryRuleDto createDto)
    {
        try
        {
            var tld = await _context.Tlds.FirstOrDefaultAsync(x => x.Id == createDto.TldId);
            if (tld == null)
            {
                throw new InvalidOperationException($"TLD with ID {createDto.TldId} not found");
            }

            var entity = new TldRegistryRule
            {
                TldId = createDto.TldId,
                RequireRegistrantContact = createDto.RequireRegistrantContact,
                RequireAdministrativeContact = createDto.RequireAdministrativeContact,
                RequireTechnicalContact = createDto.RequireTechnicalContact,
                RequireBillingContact = createDto.RequireBillingContact,
                RequiresAuthCodeForTransfer = createDto.RequiresAuthCodeForTransfer,
                TransferLockDays = createDto.TransferLockDays,
                RenewalGracePeriodDays = createDto.RenewalGracePeriodDays,
                RedemptionGracePeriodDays = createDto.RedemptionGracePeriodDays,
                Notes = createDto.Notes,
                IsActive = createDto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TldRegistryRules.Add(entity);
            await _context.SaveChangesAsync();

            await _context.Entry(entity).Reference(x => x.Tld).LoadAsync();

            _log.Information("Created TLD registry rule {RuleId} for TLD {TldId}", entity.Id, entity.TldId);
            return MapToDto(entity);
        }
        catch (SqliteException ex) when (IsMissingTableError(ex))
        {
            _log.Warning(ex, "Cannot create TLD registry rule because TldRegistryRules table is missing.");
            throw new InvalidOperationException("TLD registry rule table is missing. Restart API to apply database migrations.");
        }
    }

    /// <inheritdoc />
    public async Task<TldRegistryRuleDto?> UpdateAsync(int id, UpdateTldRegistryRuleDto updateDto)
    {
        try
        {
            var entity = await _context.TldRegistryRules.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return null;
            }

            var tldExists = await _context.Tlds.AnyAsync(x => x.Id == updateDto.TldId);
            if (!tldExists)
            {
                throw new InvalidOperationException($"TLD with ID {updateDto.TldId} not found");
            }

            entity.TldId = updateDto.TldId;
            entity.RequireRegistrantContact = updateDto.RequireRegistrantContact;
            entity.RequireAdministrativeContact = updateDto.RequireAdministrativeContact;
            entity.RequireTechnicalContact = updateDto.RequireTechnicalContact;
            entity.RequireBillingContact = updateDto.RequireBillingContact;
            entity.RequiresAuthCodeForTransfer = updateDto.RequiresAuthCodeForTransfer;
            entity.TransferLockDays = updateDto.TransferLockDays;
            entity.RenewalGracePeriodDays = updateDto.RenewalGracePeriodDays;
            entity.RedemptionGracePeriodDays = updateDto.RedemptionGracePeriodDays;
            entity.Notes = updateDto.Notes;
            entity.IsActive = updateDto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _context.Entry(entity).Reference(x => x.Tld).LoadAsync();

            _log.Information("Updated TLD registry rule {RuleId}", entity.Id);
            return MapToDto(entity);
        }
        catch (SqliteException ex) when (IsMissingTableError(ex))
        {
            _log.Warning(ex, "Cannot update TLD registry rule because TldRegistryRules table is missing.");
            throw new InvalidOperationException("TLD registry rule table is missing. Restart API to apply database migrations.");
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var entity = await _context.TldRegistryRules.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return false;
            }

            _context.TldRegistryRules.Remove(entity);
            await _context.SaveChangesAsync();

            _log.Information("Deleted TLD registry rule {RuleId}", id);
            return true;
        }
        catch (SqliteException ex) when (IsMissingTableError(ex))
        {
            _log.Warning(ex, "Cannot delete TLD registry rule because TldRegistryRules table is missing.");
            throw new InvalidOperationException("TLD registry rule table is missing. Restart API to apply database migrations.");
        }
    }

    private static TldRegistryRuleDto MapToDto(TldRegistryRule item)
    {
        return new TldRegistryRuleDto
        {
            Id = item.Id,
            TldId = item.TldId,
            TldExtension = item.Tld?.Extension ?? string.Empty,
            RequireRegistrantContact = item.RequireRegistrantContact,
            RequireAdministrativeContact = item.RequireAdministrativeContact,
            RequireTechnicalContact = item.RequireTechnicalContact,
            RequireBillingContact = item.RequireBillingContact,
            RequiresAuthCodeForTransfer = item.RequiresAuthCodeForTransfer,
            TransferLockDays = item.TransferLockDays,
            RenewalGracePeriodDays = item.RenewalGracePeriodDays,
            RedemptionGracePeriodDays = item.RedemptionGracePeriodDays,
            Notes = item.Notes,
            IsActive = item.IsActive,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }

    private static bool IsMissingTableError(SqliteException ex)
    {
        return ex.SqliteErrorCode == 1
            && ex.Message.Contains("no such table", StringComparison.OrdinalIgnoreCase)
            && ex.Message.Contains("TldRegistryRules", StringComparison.OrdinalIgnoreCase);
    }
}
