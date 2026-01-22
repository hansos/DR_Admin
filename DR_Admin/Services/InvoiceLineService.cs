using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class InvoiceLineService : IInvoiceLineService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<InvoiceLineService>();

    public InvoiceLineService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InvoiceLineDto>> GetAllInvoiceLinesAsync()
    {
        try
        {
            _log.Information("Fetching all invoice lines");
            
            var invoiceLines = await _context.InvoiceLines
                .AsNoTracking()
                .ToListAsync();

            var invoiceLineDtos = invoiceLines.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} invoice lines", invoiceLines.Count);
            return invoiceLineDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all invoice lines");
            throw;
        }
    }

    public async Task<InvoiceLineDto?> GetInvoiceLineByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching invoice line with ID: {InvoiceLineId}", id);
            
            var invoiceLine = await _context.InvoiceLines
                .AsNoTracking()
                .FirstOrDefaultAsync(il => il.Id == id);

            if (invoiceLine == null)
            {
                _log.Warning("Invoice line with ID {InvoiceLineId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched invoice line with ID: {InvoiceLineId}", id);
            return MapToDto(invoiceLine);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching invoice line with ID: {InvoiceLineId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<InvoiceLineDto>> GetInvoiceLinesByInvoiceIdAsync(int invoiceId)
    {
        try
        {
            _log.Information("Fetching invoice lines for invoice ID: {InvoiceId}", invoiceId);
            
            var invoiceLines = await _context.InvoiceLines
                .AsNoTracking()
                .Where(il => il.InvoiceId == invoiceId)
                .OrderBy(il => il.LineNumber)
                .ToListAsync();

            var invoiceLineDtos = invoiceLines.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} invoice lines for invoice ID: {InvoiceId}", invoiceLines.Count, invoiceId);
            return invoiceLineDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching invoice lines for invoice ID: {InvoiceId}", invoiceId);
            throw;
        }
    }

    public async Task<InvoiceLineDto> CreateInvoiceLineAsync(CreateInvoiceLineDto createDto)
    {
        try
        {
            _log.Information("Creating new invoice line for invoice ID: {InvoiceId}", createDto.InvoiceId);

            // Get Unit by code
            var unit = await _context.Units.FirstOrDefaultAsync(u => u.Code == createDto.Unit);
            if (unit == null)
            {
                throw new InvalidOperationException($"Unit with code '{createDto.Unit}' not found");
            }

            var invoiceLine = new InvoiceLine
            {
                InvoiceId = createDto.InvoiceId,
                ServiceId = createDto.ServiceId,
                UnitId = unit.Id,
                LineNumber = createDto.LineNumber,
                Description = createDto.Description,
                Quantity = createDto.Quantity,
                UnitPrice = createDto.UnitPrice,
                Discount = createDto.Discount,
                TotalPrice = createDto.TotalPrice,
                TaxRate = createDto.TaxRate,
                TaxAmount = createDto.TaxAmount,
                TotalWithTax = createDto.TotalWithTax,
                ServiceNameSnapshot = createDto.ServiceNameSnapshot,
                AccountingCode = createDto.AccountingCode,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.InvoiceLines.Add(invoiceLine);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created invoice line with ID: {InvoiceLineId}", invoiceLine.Id);
            return MapToDto(invoiceLine);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating invoice line for invoice ID: {InvoiceId}", createDto.InvoiceId);
            throw;
        }
    }

    public async Task<InvoiceLineDto?> UpdateInvoiceLineAsync(int id, UpdateInvoiceLineDto updateDto)
    {
        try
        {
            _log.Information("Updating invoice line with ID: {InvoiceLineId}", id);

            var invoiceLine = await _context.InvoiceLines.FindAsync(id);

            if (invoiceLine == null)
            {
                _log.Warning("Invoice line with ID {InvoiceLineId} not found for update", id);
                return null;
            }

            // Get Unit by code
            var unit = await _context.Units.FirstOrDefaultAsync(u => u.Code == updateDto.Unit);
            if (unit == null)
            {
                throw new InvalidOperationException($"Unit with code '{updateDto.Unit}' not found");
            }

            invoiceLine.InvoiceId = updateDto.InvoiceId;
            invoiceLine.ServiceId = updateDto.ServiceId;
            invoiceLine.UnitId = unit.Id;
            invoiceLine.LineNumber = updateDto.LineNumber;
            invoiceLine.Description = updateDto.Description;
            invoiceLine.Quantity = updateDto.Quantity;
            invoiceLine.UnitPrice = updateDto.UnitPrice;
            invoiceLine.Discount = updateDto.Discount;
            invoiceLine.TotalPrice = updateDto.TotalPrice;
            invoiceLine.TaxRate = updateDto.TaxRate;
            invoiceLine.TaxAmount = updateDto.TaxAmount;
            invoiceLine.TotalWithTax = updateDto.TotalWithTax;
            invoiceLine.ServiceNameSnapshot = updateDto.ServiceNameSnapshot;
            invoiceLine.AccountingCode = updateDto.AccountingCode;
            invoiceLine.Notes = updateDto.Notes;
            invoiceLine.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated invoice line with ID: {InvoiceLineId}", id);
            return MapToDto(invoiceLine);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating invoice line with ID: {InvoiceLineId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteInvoiceLineAsync(int id)
    {
        try
        {
            _log.Information("Deleting invoice line with ID: {InvoiceLineId}", id);

            var invoiceLine = await _context.InvoiceLines.FindAsync(id);

            if (invoiceLine == null)
            {
                _log.Warning("Invoice line with ID {InvoiceLineId} not found for deletion", id);
                return false;
            }

            _context.InvoiceLines.Remove(invoiceLine);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted invoice line with ID: {InvoiceLineId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting invoice line with ID: {InvoiceLineId}", id);
            throw;
        }
    }

    private static InvoiceLineDto MapToDto(InvoiceLine invoiceLine)
    {
        return new InvoiceLineDto
        {
            Id = invoiceLine.Id,
            InvoiceId = invoiceLine.InvoiceId,
            ServiceId = invoiceLine.ServiceId,
            UnitId = invoiceLine.UnitId ?? 0,
            LineNumber = invoiceLine.LineNumber,
            Description = invoiceLine.Description,
            Quantity = invoiceLine.Quantity,
            UnitPrice = invoiceLine.UnitPrice,
            Discount = invoiceLine.Discount,
            TotalPrice = invoiceLine.TotalPrice,
            TaxRate = invoiceLine.TaxRate,
            TaxAmount = invoiceLine.TaxAmount,
            TotalWithTax = invoiceLine.TotalWithTax,
            ServiceNameSnapshot = invoiceLine.ServiceNameSnapshot,
            AccountingCode = invoiceLine.AccountingCode,
            Notes = invoiceLine.Notes,
            CreatedAt = invoiceLine.CreatedAt,
            UpdatedAt = invoiceLine.UpdatedAt,
            DeletedAt = invoiceLine.DeletedAt
        };
    }
}
