using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing subscription billing history records
/// </summary>
public class SubscriptionBillingHistoryService : ISubscriptionBillingHistoryService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<SubscriptionBillingHistoryService>();

    public SubscriptionBillingHistoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all billing history records
    /// </summary>
    public async Task<IEnumerable<SubscriptionBillingHistoryDto>> GetAllBillingHistoriesAsync()
    {
        try
        {
            _log.Information("Fetching all subscription billing histories");

            var histories = await _context.Set<SubscriptionBillingHistory>()
                .AsNoTracking()
                .OrderByDescending(h => h.BillingDate)
                .ToListAsync();

            var historyDtos = histories.Select(MapToDto);

            _log.Information("Successfully fetched {Count} billing histories", histories.Count);
            return historyDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all billing histories");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all billing history records for a specific subscription
    /// </summary>
    public async Task<IEnumerable<SubscriptionBillingHistoryDto>> GetBillingHistoriesBySubscriptionIdAsync(int subscriptionId)
    {
        try
        {
            _log.Information("Fetching billing histories for subscription ID: {SubscriptionId}", subscriptionId);

            var histories = await _context.Set<SubscriptionBillingHistory>()
                .AsNoTracking()
                .Where(h => h.SubscriptionId == subscriptionId)
                .OrderByDescending(h => h.BillingDate)
                .ToListAsync();

            var historyDtos = histories.Select(MapToDto);

            _log.Information("Successfully fetched {Count} billing histories for subscription ID: {SubscriptionId}",
                histories.Count, subscriptionId);
            return historyDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching billing histories for subscription ID: {SubscriptionId}", subscriptionId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a billing history record by its unique identifier
    /// </summary>
    public async Task<SubscriptionBillingHistoryDto?> GetBillingHistoryByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching billing history with ID: {BillingHistoryId}", id);

            var history = await _context.Set<SubscriptionBillingHistory>()
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);

            if (history == null)
            {
                _log.Warning("Billing history with ID: {BillingHistoryId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched billing history with ID: {BillingHistoryId}", id);
            return MapToDto(history);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching billing history with ID: {BillingHistoryId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new billing history record
    /// </summary>
    public async Task<SubscriptionBillingHistoryDto> CreateBillingHistoryAsync(CreateSubscriptionBillingHistoryDto createDto)
    {
        try
        {
            _log.Information("Creating new billing history for subscription ID: {SubscriptionId}", createDto.SubscriptionId);

            var history = new SubscriptionBillingHistory
            {
                SubscriptionId = createDto.SubscriptionId,
                InvoiceId = createDto.InvoiceId,
                PaymentTransactionId = createDto.PaymentTransactionId,
                BillingDate = createDto.BillingDate,
                AmountCharged = createDto.AmountCharged,
                CurrencyCode = createDto.CurrencyCode,
                Status = createDto.Status,
                AttemptCount = createDto.AttemptCount,
                ErrorMessage = createDto.ErrorMessage,
                PeriodStart = createDto.PeriodStart,
                PeriodEnd = createDto.PeriodEnd,
                IsAutomatic = createDto.IsAutomatic,
                ProcessedByUserId = createDto.ProcessedByUserId,
                Notes = createDto.Notes,
                Metadata = createDto.Metadata,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Set<SubscriptionBillingHistory>().Add(history);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created billing history with ID: {BillingHistoryId} for subscription ID: {SubscriptionId}",
                history.Id, createDto.SubscriptionId);

            return MapToDto(history);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating billing history for subscription ID: {SubscriptionId}",
                createDto.SubscriptionId);
            throw;
        }
    }

    /// <summary>
    /// Deletes a billing history record
    /// </summary>
    public async Task<bool> DeleteBillingHistoryAsync(int id)
    {
        try
        {
            _log.Information("Deleting billing history with ID: {BillingHistoryId}", id);

            var history = await _context.Set<SubscriptionBillingHistory>()
                .FirstOrDefaultAsync(h => h.Id == id);

            if (history == null)
            {
                _log.Warning("Billing history with ID: {BillingHistoryId} not found", id);
                return false;
            }

            _context.Set<SubscriptionBillingHistory>().Remove(history);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted billing history with ID: {BillingHistoryId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting billing history with ID: {BillingHistoryId}", id);
            throw;
        }
    }

    // Private helper methods

    private static SubscriptionBillingHistoryDto MapToDto(SubscriptionBillingHistory history)
    {
        return new SubscriptionBillingHistoryDto
        {
            Id = history.Id,
            SubscriptionId = history.SubscriptionId,
            InvoiceId = history.InvoiceId,
            PaymentTransactionId = history.PaymentTransactionId,
            BillingDate = history.BillingDate,
            AmountCharged = history.AmountCharged,
            CurrencyCode = history.CurrencyCode,
            Status = history.Status,
            AttemptCount = history.AttemptCount,
            ErrorMessage = history.ErrorMessage,
            PeriodStart = history.PeriodStart,
            PeriodEnd = history.PeriodEnd,
            IsAutomatic = history.IsAutomatic,
            ProcessedByUserId = history.ProcessedByUserId,
            Notes = history.Notes,
            Metadata = history.Metadata,
            CreatedAt = history.CreatedAt,
            UpdatedAt = history.UpdatedAt
        };
    }
}
