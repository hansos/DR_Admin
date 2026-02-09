using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing vendor payout operations
/// </summary>
public class VendorPayoutService : IVendorPayoutService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<VendorPayoutService>();

    public VendorPayoutService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VendorPayoutDto>> GetAllVendorPayoutsAsync()
    {
        try
        {
            _log.Information("Fetching all vendor payouts");
            
            var payouts = await _context.VendorPayouts
                .Include(p => p.VendorCosts)
                .AsNoTracking()
                .ToListAsync();

            _log.Information("Successfully fetched {Count} vendor payouts", payouts.Count);
            return payouts.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all vendor payouts");
            throw;
        }
    }

    public async Task<PagedResult<VendorPayoutDto>> GetAllVendorPayoutsPagedAsync(PaginationParameters parameters)
    {
        try
        {
            _log.Information("Fetching paginated vendor payouts - Page: {PageNumber}, PageSize: {PageSize}", 
                parameters.PageNumber, parameters.PageSize);
            
            var totalCount = await _context.VendorPayouts.AsNoTracking().CountAsync();

            var payouts = await _context.VendorPayouts
                .Include(p => p.VendorCosts)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var dtos = payouts.Select(MapToDto).ToList();
            
            return new PagedResult<VendorPayoutDto>(dtos, totalCount, parameters.PageNumber, parameters.PageSize);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching paginated vendor payouts");
            throw;
        }
    }

    public async Task<VendorPayoutDto?> GetVendorPayoutByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching vendor payout with ID: {Id}", id);

            var payout = await _context.VendorPayouts
                .Include(p => p.VendorCosts)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payout == null)
            {
                _log.Warning("Vendor payout with ID {Id} not found", id);
                return null;
            }

            return MapToDto(payout);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching vendor payout with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<VendorPayoutDto>> GetVendorPayoutsByVendorIdAsync(int vendorId)
    {
        try
        {
            _log.Information("Fetching vendor payouts for vendor ID: {VendorId}", vendorId);

            var payouts = await _context.VendorPayouts
                .Include(p => p.VendorCosts)
                .AsNoTracking()
                .Where(p => p.VendorId == vendorId)
                .ToListAsync();

            return payouts.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching vendor payouts for vendor ID: {VendorId}", vendorId);
            throw;
        }
    }

    public async Task<VendorPayoutDto> CreateVendorPayoutAsync(CreateVendorPayoutDto dto)
    {
        try
        {
            _log.Information("Creating new vendor payout for vendor ID: {VendorId}", dto.VendorId);

            var payout = new VendorPayout
            {
                VendorId = dto.VendorId,
                VendorType = dto.VendorType,
                VendorName = dto.VendorName,
                PayoutMethod = dto.PayoutMethod,
                VendorCurrency = dto.VendorCurrency,
                VendorAmount = dto.VendorAmount,
                BaseCurrency = dto.BaseCurrency,
                BaseAmount = dto.BaseAmount,
                ExchangeRate = dto.ExchangeRate,
                ExchangeRateDate = dto.ExchangeRateDate,
                ScheduledDate = dto.ScheduledDate,
                InternalNotes = dto.InternalNotes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.VendorPayouts.Add(payout);

            if (dto.VendorCostIds.Any())
            {
                var costs = await _context.VendorCosts
                    .Where(c => dto.VendorCostIds.Contains(c.Id))
                    .ToListAsync();

                foreach (var cost in costs)
                {
                    cost.VendorPayoutId = payout.Id;
                }
            }

            await _context.SaveChangesAsync();

            _log.Information("Successfully created vendor payout with ID: {Id}", payout.Id);
            
            var created = await GetVendorPayoutByIdAsync(payout.Id);
            return created!;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating vendor payout");
            throw;
        }
    }

    public async Task<VendorPayoutDto?> UpdateVendorPayoutAsync(int id, UpdateVendorPayoutDto dto)
    {
        try
        {
            _log.Information("Updating vendor payout with ID: {Id}", id);

            var payout = await _context.VendorPayouts.FindAsync(id);

            if (payout == null)
            {
                _log.Warning("Vendor payout with ID {Id} not found for update", id);
                return null;
            }

            payout.Status = dto.Status;
            payout.ScheduledDate = dto.ScheduledDate;
            payout.FailureReason = dto.FailureReason;
            payout.TransactionReference = dto.TransactionReference;
            payout.RequiresManualIntervention = dto.RequiresManualIntervention;
            payout.InterventionReason = dto.InterventionReason;
            payout.InternalNotes = dto.InternalNotes;
            payout.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetVendorPayoutByIdAsync(id);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating vendor payout with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteVendorPayoutAsync(int id)
    {
        try
        {
            _log.Information("Deleting vendor payout with ID: {Id}", id);

            var payout = await _context.VendorPayouts.FindAsync(id);

            if (payout == null)
            {
                _log.Warning("Vendor payout with ID {Id} not found for deletion", id);
                return false;
            }

            _context.VendorPayouts.Remove(payout);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted vendor payout with ID: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting vendor payout with ID: {Id}", id);
            throw;
        }
    }

    public async Task<VendorPayoutDto?> ProcessVendorPayoutAsync(ProcessVendorPayoutDto dto)
    {
        try
        {
            _log.Information("Processing vendor payout ID: {Id}", dto.VendorPayoutId);

            var payout = await _context.VendorPayouts.FindAsync(dto.VendorPayoutId);

            if (payout == null)
            {
                _log.Warning("Vendor payout with ID {Id} not found for processing", dto.VendorPayoutId);
                return null;
            }

            payout.TransactionReference = dto.TransactionReference;
            payout.ProcessedDate = DateTime.UtcNow;

            if (dto.IsSuccessful)
            {
                payout.Status = Data.Enums.VendorPayoutStatus.Paid;
                payout.FailureReason = string.Empty;
            }
            else
            {
                payout.Status = Data.Enums.VendorPayoutStatus.Failed;
                payout.FailureReason = dto.FailureReason ?? string.Empty;
                payout.FailureCount++;
            }

            payout.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _log.Information("Successfully processed vendor payout ID: {Id}, Success: {IsSuccessful}", 
                dto.VendorPayoutId, dto.IsSuccessful);

            return await GetVendorPayoutByIdAsync(dto.VendorPayoutId);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while processing vendor payout ID: {Id}", dto.VendorPayoutId);
            throw;
        }
    }

    public async Task<VendorPayoutDto?> ResolvePayoutInterventionAsync(ResolvePayoutInterventionDto dto)
    {
        try
        {
            _log.Information("Resolving intervention for vendor payout ID: {Id}", dto.VendorPayoutId);

            var payout = await _context.VendorPayouts.FindAsync(dto.VendorPayoutId);

            if (payout == null)
            {
                _log.Warning("Vendor payout with ID {Id} not found for intervention resolution", dto.VendorPayoutId);
                return null;
            }

            payout.RequiresManualIntervention = false;
            payout.InterventionResolvedAt = DateTime.UtcNow;
            payout.InterventionResolvedByUserId = dto.ResolvedByUserId;
            payout.InternalNotes += $"\n[{DateTime.UtcNow}] Intervention resolved: {dto.ResolutionNotes}";

            if (dto.ProceedWithPayout)
            {
                payout.Status = Data.Enums.VendorPayoutStatus.Pending;
            }

            payout.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _log.Information("Successfully resolved intervention for vendor payout ID: {Id}", dto.VendorPayoutId);

            return await GetVendorPayoutByIdAsync(dto.VendorPayoutId);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while resolving intervention for vendor payout ID: {Id}", dto.VendorPayoutId);
            throw;
        }
    }

    public async Task<VendorPayoutSummaryDto?> GetVendorPayoutSummaryByVendorIdAsync(int vendorId)
    {
        try
        {
            _log.Information("Fetching vendor payout summary for vendor ID: {VendorId}", vendorId);

            var payouts = await _context.VendorPayouts
                .AsNoTracking()
                .Where(p => p.VendorId == vendorId)
                .ToListAsync();

            if (!payouts.Any())
            {
                return null;
            }

            var firstPayout = payouts.First();
            var totalPending = payouts.Where(p => p.Status == Data.Enums.VendorPayoutStatus.Pending).Sum(p => p.BaseAmount);
            var totalProcessing = payouts.Where(p => p.Status == Data.Enums.VendorPayoutStatus.Processing).Sum(p => p.BaseAmount);
            var totalPaid = payouts.Where(p => p.Status == Data.Enums.VendorPayoutStatus.Paid).Sum(p => p.BaseAmount);
            var totalFailed = payouts.Where(p => p.Status == Data.Enums.VendorPayoutStatus.Failed).Sum(p => p.BaseAmount);
            var pendingCount = payouts.Count(p => p.Status == Data.Enums.VendorPayoutStatus.Pending);
            var requiresInterventionCount = payouts.Count(p => p.RequiresManualIntervention);
            var nextScheduledDate = payouts.Where(p => p.Status == Data.Enums.VendorPayoutStatus.Pending)
                .OrderBy(p => p.ScheduledDate)
                .Select(p => p.ScheduledDate)
                .FirstOrDefault();

            var recentPayouts = await _context.VendorPayouts
                .Include(p => p.VendorCosts)
                .AsNoTracking()
                .Where(p => p.VendorId == vendorId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(10)
                .ToListAsync();

            return new VendorPayoutSummaryDto
            {
                VendorId = vendorId,
                VendorName = firstPayout.VendorName,
                VendorType = firstPayout.VendorType,
                TotalPending = totalPending,
                TotalProcessing = totalProcessing,
                TotalPaid = totalPaid,
                TotalFailed = totalFailed,
                CurrencyCode = firstPayout.BaseCurrency,
                PendingCount = pendingCount,
                RequiresInterventionCount = requiresInterventionCount,
                NextScheduledDate = nextScheduledDate == default ? null : nextScheduledDate,
                RecentPayouts = recentPayouts.Select(MapToDto).ToList()
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching vendor payout summary for vendor ID: {VendorId}", vendorId);
            throw;
        }
    }

    private static VendorPayoutDto MapToDto(VendorPayout payout)
    {
        return new VendorPayoutDto
        {
            Id = payout.Id,
            VendorId = payout.VendorId,
            VendorType = payout.VendorType,
            VendorName = payout.VendorName,
            PayoutMethod = payout.PayoutMethod,
            VendorCurrency = payout.VendorCurrency,
            VendorAmount = payout.VendorAmount,
            BaseCurrency = payout.BaseCurrency,
            BaseAmount = payout.BaseAmount,
            ExchangeRate = payout.ExchangeRate,
            ExchangeRateDate = payout.ExchangeRateDate,
            Status = payout.Status,
            ScheduledDate = payout.ScheduledDate,
            ProcessedDate = payout.ProcessedDate,
            FailureReason = payout.FailureReason,
            FailureCount = payout.FailureCount,
            TransactionReference = payout.TransactionReference,
            RequiresManualIntervention = payout.RequiresManualIntervention,
            InterventionReason = payout.InterventionReason,
            InterventionResolvedAt = payout.InterventionResolvedAt,
            InterventionResolvedByUserId = payout.InterventionResolvedByUserId,
            InternalNotes = payout.InternalNotes,
            VendorCosts = payout.VendorCosts?.Select(c => new VendorCostDto
            {
                Id = c.Id,
                InvoiceLineId = c.InvoiceLineId,
                VendorPayoutId = c.VendorPayoutId,
                VendorType = c.VendorType,
                VendorId = c.VendorId,
                VendorName = c.VendorName,
                VendorCurrency = c.VendorCurrency,
                VendorAmount = c.VendorAmount,
                BaseCurrency = c.BaseCurrency,
                BaseAmount = c.BaseAmount,
                ExchangeRate = c.ExchangeRate,
                ExchangeRateDate = c.ExchangeRateDate,
                IsRefundable = c.IsRefundable,
                RefundPolicy = c.RefundPolicy,
                RefundDeadline = c.RefundDeadline,
                Status = c.Status,
                Notes = c.Notes,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList() ?? new List<VendorCostDto>(),
            CreatedAt = payout.CreatedAt,
            UpdatedAt = payout.UpdatedAt
        };
    }
}
