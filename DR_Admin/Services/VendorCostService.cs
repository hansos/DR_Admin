using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing vendor cost operations
/// </summary>
public class VendorCostService : IVendorCostService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<VendorCostService>();

    public VendorCostService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VendorCostDto>> GetAllVendorCostsAsync()
    {
        try
        {
            _log.Information("Fetching all vendor costs");
            
            var costs = await _context.VendorCosts
                .AsNoTracking()
                .ToListAsync();

            var dtos = costs.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} vendor costs", costs.Count);
            return dtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all vendor costs");
            throw;
        }
    }

    public async Task<PagedResult<VendorCostDto>> GetAllVendorCostsPagedAsync(PaginationParameters parameters)
    {
        try
        {
            _log.Information("Fetching paginated vendor costs - Page: {PageNumber}, PageSize: {PageSize}", 
                parameters.PageNumber, parameters.PageSize);
            
            var totalCount = await _context.VendorCosts
                .AsNoTracking()
                .CountAsync();

            var costs = await _context.VendorCosts
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var dtos = costs.Select(MapToDto).ToList();
            
            var result = new PagedResult<VendorCostDto>(
                dtos, 
                totalCount, 
                parameters.PageNumber, 
                parameters.PageSize);

            _log.Information("Successfully fetched page {PageNumber} of vendor costs - Returned {Count} of {TotalCount} total", 
                parameters.PageNumber, dtos.Count, totalCount);
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching paginated vendor costs");
            throw;
        }
    }

    public async Task<VendorCostDto?> GetVendorCostByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching vendor cost with ID: {Id}", id);

            var cost = await _context.VendorCosts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cost == null)
            {
                _log.Warning("Vendor cost with ID {Id} not found", id);
                return null;
            }

            _log.Information("Successfully fetched vendor cost with ID: {Id}", id);
            return MapToDto(cost);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching vendor cost with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<VendorCostDto>> GetVendorCostsByInvoiceLineIdAsync(int invoiceLineId)
    {
        try
        {
            _log.Information("Fetching vendor costs for invoice line ID: {InvoiceLineId}", invoiceLineId);

            var costs = await _context.VendorCosts
                .AsNoTracking()
                .Where(c => c.InvoiceLineId == invoiceLineId)
                .ToListAsync();

            _log.Information("Successfully fetched {Count} vendor costs for invoice line ID: {InvoiceLineId}", 
                costs.Count, invoiceLineId);
            return costs.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching vendor costs for invoice line ID: {InvoiceLineId}", invoiceLineId);
            throw;
        }
    }

    public async Task<IEnumerable<VendorCostDto>> GetVendorCostsByPayoutIdAsync(int payoutId)
    {
        try
        {
            _log.Information("Fetching vendor costs for payout ID: {PayoutId}", payoutId);

            var costs = await _context.VendorCosts
                .AsNoTracking()
                .Where(c => c.VendorPayoutId == payoutId)
                .ToListAsync();

            _log.Information("Successfully fetched {Count} vendor costs for payout ID: {PayoutId}", 
                costs.Count, payoutId);
            return costs.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching vendor costs for payout ID: {PayoutId}", payoutId);
            throw;
        }
    }

    public async Task<VendorCostDto> CreateVendorCostAsync(CreateVendorCostDto dto)
    {
        try
        {
            _log.Information("Creating new vendor cost for invoice line ID: {InvoiceLineId}", dto.InvoiceLineId);

            var cost = new VendorCost
            {
                InvoiceLineId = dto.InvoiceLineId,
                VendorType = dto.VendorType,
                VendorId = dto.VendorId,
                VendorName = dto.VendorName,
                VendorCurrency = dto.VendorCurrency,
                VendorAmount = dto.VendorAmount,
                BaseCurrency = dto.BaseCurrency,
                BaseAmount = dto.BaseAmount,
                ExchangeRate = dto.ExchangeRate,
                ExchangeRateDate = dto.ExchangeRateDate,
                IsRefundable = dto.IsRefundable,
                RefundPolicy = dto.RefundPolicy,
                RefundDeadline = dto.RefundDeadline,
                Notes = dto.Notes,
                Status = Data.Enums.VendorCostStatus.Estimated,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.VendorCosts.Add(cost);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created vendor cost with ID: {Id}", cost.Id);
            return MapToDto(cost);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating vendor cost");
            throw;
        }
    }

    public async Task<VendorCostDto?> UpdateVendorCostAsync(int id, UpdateVendorCostDto dto)
    {
        try
        {
            _log.Information("Updating vendor cost with ID: {Id}", id);

            var cost = await _context.VendorCosts.FindAsync(id);

            if (cost == null)
            {
                _log.Warning("Vendor cost with ID {Id} not found for update", id);
                return null;
            }

            cost.IsRefundable = dto.IsRefundable;
            cost.RefundPolicy = dto.RefundPolicy;
            cost.RefundDeadline = dto.RefundDeadline;
            cost.Status = dto.Status;
            cost.Notes = dto.Notes;
            cost.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated vendor cost with ID: {Id}", id);
            return MapToDto(cost);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating vendor cost with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteVendorCostAsync(int id)
    {
        try
        {
            _log.Information("Deleting vendor cost with ID: {Id}", id);

            var cost = await _context.VendorCosts.FindAsync(id);

            if (cost == null)
            {
                _log.Warning("Vendor cost with ID {Id} not found for deletion", id);
                return false;
            }

            _context.VendorCosts.Remove(cost);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted vendor cost with ID: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting vendor cost with ID: {Id}", id);
            throw;
        }
    }

    public async Task<VendorCostSummaryDto?> GetVendorCostSummaryByInvoiceIdAsync(int invoiceId)
    {
        try
        {
            _log.Information("Fetching vendor cost summary for invoice ID: {InvoiceId}", invoiceId);

            var invoice = await _context.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == invoiceId);

            if (invoice == null)
            {
                _log.Warning("Invoice with ID {InvoiceId} not found", invoiceId);
                return null;
            }

            var costs = await _context.VendorCosts
                .AsNoTracking()
                .Include(c => c.InvoiceLine)
                .Where(c => c.InvoiceLine.InvoiceId == invoiceId)
                .ToListAsync();

            var totalVendorCosts = costs.Sum(c => c.BaseAmount);
            var totalPaid = costs.Where(c => c.VendorPayoutId != null).Sum(c => c.BaseAmount);
            var totalUnpaid = costs.Where(c => c.VendorPayoutId == null).Sum(c => c.BaseAmount);
            var totalRefundable = costs.Where(c => c.IsRefundable).Sum(c => c.BaseAmount);
            var totalNonRefundable = costs.Where(c => !c.IsRefundable).Sum(c => c.BaseAmount);
            var grossProfit = invoice.TotalAmount - totalVendorCosts;
            var grossProfitMargin = invoice.TotalAmount > 0 
                ? (grossProfit / invoice.TotalAmount) * 100 
                : 0;

            var summary = new VendorCostSummaryDto
            {
                InvoiceId = invoiceId,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceTotal = invoice.TotalAmount,
                CurrencyCode = invoice.CurrencyCode,
                TotalVendorCosts = totalVendorCosts,
                TotalPaidVendorCosts = totalPaid,
                TotalUnpaidVendorCosts = totalUnpaid,
                TotalRefundableVendorCosts = totalRefundable,
                TotalNonRefundableVendorCosts = totalNonRefundable,
                GrossProfit = grossProfit,
                GrossProfitMargin = grossProfitMargin,
                VendorCosts = costs.Select(MapToDto).ToList()
            };

            _log.Information("Successfully fetched vendor cost summary for invoice ID: {InvoiceId}", invoiceId);
            return summary;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching vendor cost summary for invoice ID: {InvoiceId}", invoiceId);
            throw;
        }
    }

    private static VendorCostDto MapToDto(VendorCost cost)
    {
        return new VendorCostDto
        {
            Id = cost.Id,
            InvoiceLineId = cost.InvoiceLineId,
            VendorPayoutId = cost.VendorPayoutId,
            VendorType = cost.VendorType,
            VendorId = cost.VendorId,
            VendorName = cost.VendorName,
            VendorCurrency = cost.VendorCurrency,
            VendorAmount = cost.VendorAmount,
            BaseCurrency = cost.BaseCurrency,
            BaseAmount = cost.BaseAmount,
            ExchangeRate = cost.ExchangeRate,
            ExchangeRateDate = cost.ExchangeRateDate,
            IsRefundable = cost.IsRefundable,
            RefundPolicy = cost.RefundPolicy,
            RefundDeadline = cost.RefundDeadline,
            Status = cost.Status,
            Notes = cost.Notes,
            CreatedAt = cost.CreatedAt,
            UpdatedAt = cost.UpdatedAt
        };
    }
}
