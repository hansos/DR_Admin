using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class InvoiceService : IInvoiceService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<InvoiceService>();

    public InvoiceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
    {
        try
        {
            _log.Information("Fetching all invoices");
            
            var invoices = await _context.Invoices
                .AsNoTracking()
                .Where(i => i.DeletedAt == null)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var invoiceDtos = invoices.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} invoices", invoices.Count);
            return invoiceDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all invoices");
            throw;
        }
    }

    public async Task<IEnumerable<InvoiceDto>> GetInvoicesByCustomerIdAsync(int customerId)
    {
        try
        {
            _log.Information("Fetching invoices for customer ID: {CustomerId}", customerId);
            
            var invoices = await _context.Invoices
                .AsNoTracking()
                .Where(i => i.CustomerId == customerId && i.DeletedAt == null)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var invoiceDtos = invoices.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} invoices for customer ID: {CustomerId}", invoices.Count, customerId);
            return invoiceDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching invoices for customer ID: {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<IEnumerable<InvoiceDto>> GetInvoicesByStatusAsync(InvoiceStatus status)
    {
        try
        {
            _log.Information("Fetching invoices with status: {Status}", status);
            
            var invoices = await _context.Invoices
                .AsNoTracking()
                .Where(i => i.Status == status && i.DeletedAt == null)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var invoiceDtos = invoices.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} invoices with status: {Status}", invoices.Count, status);
            return invoiceDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching invoices with status: {Status}", status);
            throw;
        }
    }

    public async Task<InvoiceDto?> GetInvoiceByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching invoice with ID: {InvoiceId}", id);
            
            var invoice = await _context.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

            if (invoice == null)
            {
                _log.Warning("Invoice with ID {InvoiceId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched invoice with ID: {InvoiceId}", id);
            return MapToDto(invoice);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching invoice with ID: {InvoiceId}", id);
            throw;
        }
    }

    public async Task<InvoiceDto?> GetInvoiceByNumberAsync(string invoiceNumber)
    {
        try
        {
            _log.Information("Fetching invoice with number: {InvoiceNumber}", invoiceNumber);
            
            var invoice = await _context.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber && i.DeletedAt == null);

            if (invoice == null)
            {
                _log.Warning("Invoice with number {InvoiceNumber} not found", invoiceNumber);
                return null;
            }

            _log.Information("Successfully fetched invoice with number: {InvoiceNumber}", invoiceNumber);
            return MapToDto(invoice);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching invoice with number: {InvoiceNumber}", invoiceNumber);
            throw;
        }
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createDto)
    {
        try
        {
            _log.Information("Creating new invoice with number: {InvoiceNumber}", createDto.InvoiceNumber);

            var existingInvoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.InvoiceNumber == createDto.InvoiceNumber);

            if (existingInvoice != null)
            {
                _log.Warning("Invoice with number {InvoiceNumber} already exists", createDto.InvoiceNumber);
                throw new InvalidOperationException($"Invoice with number {createDto.InvoiceNumber} already exists");
            }

            var invoice = new Invoice
            {
                InvoiceNumber = createDto.InvoiceNumber,
                CustomerId = createDto.CustomerId,
                Status = createDto.Status,
                IssueDate = createDto.IssueDate,
                DueDate = createDto.DueDate,
                SubTotal = createDto.SubTotal,
                TaxAmount = createDto.TaxAmount,
                TotalAmount = createDto.TotalAmount,
                AmountPaid = 0,
                AmountDue = createDto.TotalAmount,
                CurrencyCode = createDto.CurrencyCode,
                TaxRate = createDto.TaxRate,
                TaxName = createDto.TaxName,
                CustomerName = createDto.CustomerName,
                CustomerAddress = createDto.CustomerAddress,
                CustomerTaxId = createDto.CustomerTaxId,
                PaymentReference = createDto.PaymentReference,
                PaymentMethod = createDto.PaymentMethod,
                Notes = createDto.Notes,
                InternalComment = createDto.InternalComment,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created invoice with ID: {InvoiceId} and number: {InvoiceNumber}", invoice.Id, invoice.InvoiceNumber);
            return MapToDto(invoice);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating invoice with number: {InvoiceNumber}", createDto.InvoiceNumber);
            throw;
        }
    }

    public async Task<InvoiceDto?> UpdateInvoiceAsync(int id, UpdateInvoiceDto updateDto)
    {
        try
        {
            _log.Information("Updating invoice with ID: {InvoiceId}", id);

            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null || invoice.DeletedAt != null)
            {
                _log.Warning("Invoice with ID {InvoiceId} not found for update", id);
                return null;
            }

            var duplicateNumber = await _context.Invoices
                .AnyAsync(i => i.InvoiceNumber == updateDto.InvoiceNumber && i.Id != id && i.DeletedAt == null);

            if (duplicateNumber)
            {
                _log.Warning("Cannot update invoice {InvoiceId}: number {InvoiceNumber} already exists", id, updateDto.InvoiceNumber);
                throw new InvalidOperationException($"Invoice with number {updateDto.InvoiceNumber} already exists");
            }

            invoice.InvoiceNumber = updateDto.InvoiceNumber;
            invoice.Status = updateDto.Status;
            invoice.IssueDate = updateDto.IssueDate;
            invoice.DueDate = updateDto.DueDate;
            invoice.PaidAt = updateDto.PaidAt;
            invoice.SubTotal = updateDto.SubTotal;
            invoice.TaxAmount = updateDto.TaxAmount;
            invoice.TotalAmount = updateDto.TotalAmount;
            invoice.AmountPaid = updateDto.AmountPaid;
            invoice.AmountDue = updateDto.AmountDue;
            invoice.CurrencyCode = updateDto.CurrencyCode;
            invoice.TaxRate = updateDto.TaxRate;
            invoice.TaxName = updateDto.TaxName;
            invoice.CustomerName = updateDto.CustomerName;
            invoice.CustomerAddress = updateDto.CustomerAddress;
            invoice.CustomerTaxId = updateDto.CustomerTaxId;
            invoice.PaymentReference = updateDto.PaymentReference;
            invoice.PaymentMethod = updateDto.PaymentMethod;
            invoice.Notes = updateDto.Notes;
            invoice.InternalComment = updateDto.InternalComment;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated invoice with ID: {InvoiceId}", id);
            return MapToDto(invoice);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating invoice with ID: {InvoiceId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteInvoiceAsync(int id)
    {
        try
        {
            _log.Information("Soft deleting invoice with ID: {InvoiceId}", id);

            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null || invoice.DeletedAt != null)
            {
                _log.Warning("Invoice with ID {InvoiceId} not found for deletion", id);
                return false;
            }

            invoice.DeletedAt = DateTime.UtcNow;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully soft deleted invoice with ID: {InvoiceId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting invoice with ID: {InvoiceId}", id);
            throw;
        }
    }

    private static InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerId = invoice.CustomerId,
            Status = invoice.Status,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            PaidAt = invoice.PaidAt,
            SubTotal = invoice.SubTotal,
            TaxAmount = invoice.TaxAmount,
            TotalAmount = invoice.TotalAmount,
            AmountPaid = invoice.AmountPaid,
            AmountDue = invoice.AmountDue,
            CurrencyCode = invoice.CurrencyCode,
            TaxRate = invoice.TaxRate,
            TaxName = invoice.TaxName,
            CustomerName = invoice.CustomerName,
            CustomerAddress = invoice.CustomerAddress,
            CustomerTaxId = invoice.CustomerTaxId,
            PaymentReference = invoice.PaymentReference,
            PaymentMethod = invoice.PaymentMethod,
            Notes = invoice.Notes,
            InternalComment = invoice.InternalComment,
            CreatedAt = invoice.CreatedAt,
            UpdatedAt = invoice.UpdatedAt,
            DeletedAt = invoice.DeletedAt
        };
    }
}

