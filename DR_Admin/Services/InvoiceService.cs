using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class InvoiceService : IInvoiceService
{
    private readonly ApplicationDbContext _context;
    private readonly Serilog.ILogger _logger;

    public InvoiceService(ApplicationDbContext context, Serilog.ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
    {
        try
        {
            _logger.Information("Fetching all invoices");
            var invoices = await _context.Invoices.AsNoTracking().ToListAsync();
            _logger.Information("Successfully fetched {Count} invoices", invoices.Count);
            return invoices.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching all invoices");
            throw;
        }
    }

    public async Task<InvoiceDto?> GetInvoiceByIdAsync(int id)
    {
        try
        {
            _logger.Information("Fetching invoice with ID: {InvoiceId}", id);
            var invoice = await _context.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                _logger.Warning("Invoice with ID {InvoiceId} not found", id);
                return null;
            }

            _logger.Information("Successfully fetched invoice with ID: {InvoiceId}", id);
            return MapToDto(invoice);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching invoice with ID: {InvoiceId}", id);
            throw;
        }
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createDto)
    {
        try
        {
            _logger.Information("Creating new invoice for order: {OrderId}", createDto.OrderId);

            var invoice = new Invoice
            {
                OrderId = createDto.OrderId,
                CustomerId = createDto.CustomerId,
                Amount = createDto.Amount,
                Status = createDto.Status,
                DueDate = createDto.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            _logger.Information("Successfully created invoice with ID: {InvoiceId}", invoice.Id);
            return MapToDto(invoice);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while creating invoice");
            throw;
        }
    }

    public async Task<InvoiceDto?> UpdateInvoiceAsync(int id, UpdateInvoiceDto updateDto)
    {
        try
        {
            _logger.Information("Updating invoice with ID: {InvoiceId}", id);
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                _logger.Warning("Invoice with ID {InvoiceId} not found for update", id);
                return null;
            }

            invoice.OrderId = updateDto.OrderId;
            invoice.CustomerId = updateDto.CustomerId;
            invoice.Amount = updateDto.Amount;
            invoice.Status = updateDto.Status;
            invoice.DueDate = updateDto.DueDate;
            invoice.PaidAt = updateDto.PaidAt;

            await _context.SaveChangesAsync();

            _logger.Information("Successfully updated invoice with ID: {InvoiceId}", id);
            return MapToDto(invoice);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while updating invoice with ID: {InvoiceId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteInvoiceAsync(int id)
    {
        try
        {
            _logger.Information("Deleting invoice with ID: {InvoiceId}", id);
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                _logger.Warning("Invoice with ID {InvoiceId} not found for deletion", id);
                return false;
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            _logger.Information("Successfully deleted invoice with ID: {InvoiceId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while deleting invoice with ID: {InvoiceId}", id);
            throw;
        }
    }

    private static InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            OrderId = invoice.OrderId,
            CustomerId = invoice.CustomerId,
            Amount = invoice.Amount,
            Status = invoice.Status,
            DueDate = invoice.DueDate,
            CreatedAt = invoice.CreatedAt,
            PaidAt = invoice.PaidAt
        };
    }
}
