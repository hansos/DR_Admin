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

    public async Task<PagedResult<InvoiceDto>> GetAllInvoicesPagedAsync(PaginationParameters parameters)
    {
        try
        {
            _log.Information("Fetching paginated invoices - Page: {PageNumber}, PageSize: {PageSize}", 
                parameters.PageNumber, parameters.PageSize);
            
            var totalCount = await _context.Invoices
                .AsNoTracking()
                .Where(i => i.DeletedAt == null)
                .CountAsync();

            var invoices = await _context.Invoices
                .AsNoTracking()
                .Where(i => i.DeletedAt == null)
                .OrderByDescending(i => i.CreatedAt)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var invoiceDtos = invoices.Select(MapToDto).ToList();
            
            var result = new PagedResult<InvoiceDto>(
                invoiceDtos, 
                totalCount, 
                parameters.PageNumber, 
                parameters.PageSize);

            _log.Information("Successfully fetched page {PageNumber} of invoices - Returned {Count} of {TotalCount} total", 
                parameters.PageNumber, invoiceDtos.Count, totalCount);
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching paginated invoices");
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

    public async Task<PagedResult<InvoiceDto>> GetInvoicesByCustomerIdPagedAsync(int customerId, PaginationParameters parameters)
    {
        try
        {
            _log.Information("Fetching paginated invoices for customer ID: {CustomerId} - Page: {PageNumber}, PageSize: {PageSize}", 
                customerId, parameters.PageNumber, parameters.PageSize);
            
            var totalCount = await _context.Invoices
                .AsNoTracking()
                .Where(i => i.CustomerId == customerId && i.DeletedAt == null)
                .CountAsync();

            var invoices = await _context.Invoices
                .AsNoTracking()
                .Where(i => i.CustomerId == customerId && i.DeletedAt == null)
                .OrderByDescending(i => i.CreatedAt)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var invoiceDtos = invoices.Select(MapToDto).ToList();
            
            var result = new PagedResult<InvoiceDto>(
                invoiceDtos, 
                totalCount, 
                parameters.PageNumber, 
                parameters.PageSize);

            _log.Information("Successfully fetched page {PageNumber} of invoices for customer ID: {CustomerId} - Returned {Count} of {TotalCount} total", 
                parameters.PageNumber, customerId, invoiceDtos.Count, totalCount);
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching paginated invoices for customer ID: {CustomerId}", customerId);
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
                .Include(i => i.InvoiceLines)
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
                .Include(i => i.InvoiceLines)
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

            Order? order = null;
            if (createDto.OrderId.HasValue)
            {
                order = await _context.Orders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == createDto.OrderId.Value);

                if (order == null)
                {
                    throw new InvalidOperationException($"Order with ID {createDto.OrderId.Value} not found");
                }

                if (order.CustomerId != createDto.CustomerId)
                {
                    throw new InvalidOperationException("Invoice customer does not match order customer");
                }
            }

            OrderTaxSnapshot? taxSnapshot = null;
            if (createDto.OrderTaxSnapshotId.HasValue)
            {
                taxSnapshot = await _context.OrderTaxSnapshots
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == createDto.OrderTaxSnapshotId.Value);

                if (taxSnapshot == null)
                {
                    throw new InvalidOperationException($"Order tax snapshot with ID {createDto.OrderTaxSnapshotId.Value} not found");
                }
            }
            else if (createDto.OrderId.HasValue)
            {
                taxSnapshot = await _context.OrderTaxSnapshots
                    .AsNoTracking()
                    .Where(x => x.OrderId == createDto.OrderId.Value)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();

                if (taxSnapshot == null)
                {
                    throw new InvalidOperationException("A finalized tax snapshot is required before creating an invoice for this order");
                }
            }

            if (taxSnapshot != null && createDto.OrderId.HasValue && taxSnapshot.OrderId != createDto.OrderId.Value)
            {
                throw new InvalidOperationException("Provided tax snapshot does not belong to the provided order");
            }

            var subTotal = taxSnapshot?.NetAmount ?? createDto.SubTotal;
            var taxAmount = taxSnapshot?.TaxAmount ?? createDto.TaxAmount;
            var totalAmount = taxSnapshot?.GrossAmount ?? createDto.TotalAmount;
            var taxRate = taxSnapshot?.AppliedTaxRate ?? createDto.TaxRate;
            var taxName = taxSnapshot?.AppliedTaxName ?? createDto.TaxName;
            var customerTaxId = !string.IsNullOrWhiteSpace(createDto.CustomerTaxId)
                ? createDto.CustomerTaxId
                : taxSnapshot?.BuyerTaxId ?? string.Empty;

            var invoice = new Invoice
            {
                InvoiceNumber = createDto.InvoiceNumber,
                CustomerId = createDto.CustomerId,
                OrderId = createDto.OrderId,
                OrderTaxSnapshotId = taxSnapshot?.Id ?? createDto.OrderTaxSnapshotId,
                Status = createDto.Status,
                IssueDate = createDto.IssueDate,
                DueDate = createDto.DueDate,
                SubTotal = subTotal,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                AmountPaid = 0,
                AmountDue = totalAmount,
                CurrencyCode = createDto.CurrencyCode,
                TaxRate = taxRate,
                TaxName = taxName,
                CustomerName = createDto.CustomerName,
                CustomerAddress = createDto.CustomerAddress,
                CustomerTaxId = customerTaxId,
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

            // Only validate and update the invoice number when a new number is provided
            if (!string.IsNullOrWhiteSpace(updateDto.InvoiceNumber))
            {
                var duplicateNumber = await _context.Invoices
                    .AnyAsync(i => i.InvoiceNumber == updateDto.InvoiceNumber && i.Id != id && i.DeletedAt == null);

                if (duplicateNumber)
                {
                    _log.Warning("Cannot update invoice {InvoiceId}: number {InvoiceNumber} already exists", id, updateDto.InvoiceNumber);
                    throw new InvalidOperationException($"Invoice with number {updateDto.InvoiceNumber} already exists");
                }

                invoice.InvoiceNumber = updateDto.InvoiceNumber;
            }

            Order? order = null;
            if (updateDto.OrderId.HasValue)
            {
                order = await _context.Orders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == updateDto.OrderId.Value);

                if (order == null)
                {
                    throw new InvalidOperationException($"Order with ID {updateDto.OrderId.Value} not found");
                }

                if (order.CustomerId != invoice.CustomerId)
                {
                    throw new InvalidOperationException("Invoice customer does not match order customer");
                }
            }

            OrderTaxSnapshot? taxSnapshot = null;
            if (updateDto.OrderTaxSnapshotId.HasValue)
            {
                taxSnapshot = await _context.OrderTaxSnapshots
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == updateDto.OrderTaxSnapshotId.Value);

                if (taxSnapshot == null)
                {
                    throw new InvalidOperationException($"Order tax snapshot with ID {updateDto.OrderTaxSnapshotId.Value} not found");
                }
            }
            else if (updateDto.OrderId.HasValue)
            {
                taxSnapshot = await _context.OrderTaxSnapshots
                    .AsNoTracking()
                    .Where(x => x.OrderId == updateDto.OrderId.Value)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();

                if (taxSnapshot == null)
                {
                    throw new InvalidOperationException("A finalized tax snapshot is required before updating an order-linked invoice");
                }
            }

            if (taxSnapshot != null && updateDto.OrderId.HasValue && taxSnapshot.OrderId != updateDto.OrderId.Value)
            {
                throw new InvalidOperationException("Provided tax snapshot does not belong to the provided order");
            }

            invoice.Status = updateDto.Status;

            // Only update date/time fields when a non-default value is provided by the client
            if (updateDto.IssueDate != default(DateTime))
                invoice.IssueDate = updateDto.IssueDate;

            if (updateDto.DueDate != default(DateTime))
                invoice.DueDate = updateDto.DueDate;

            if (updateDto.PaidAt.HasValue)
                invoice.PaidAt = updateDto.PaidAt;

            if (taxSnapshot != null)
            {
                invoice.OrderId = updateDto.OrderId;
                invoice.OrderTaxSnapshotId = taxSnapshot.Id;
                invoice.SubTotal = taxSnapshot.NetAmount;
                invoice.TaxAmount = taxSnapshot.TaxAmount;
                invoice.TotalAmount = taxSnapshot.GrossAmount;
                invoice.TaxRate = taxSnapshot.AppliedTaxRate;
                invoice.TaxName = taxSnapshot.AppliedTaxName;
                invoice.CustomerTaxId = !string.IsNullOrWhiteSpace(updateDto.CustomerTaxId)
                    ? updateDto.CustomerTaxId
                    : taxSnapshot.BuyerTaxId;
            }
            else
            {
                invoice.OrderId = updateDto.OrderId;
                invoice.OrderTaxSnapshotId = updateDto.OrderTaxSnapshotId;
                invoice.SubTotal = updateDto.SubTotal;
                invoice.TaxAmount = updateDto.TaxAmount;
                invoice.TotalAmount = updateDto.TotalAmount;
                invoice.TaxRate = updateDto.TaxRate;
                invoice.TaxName = updateDto.TaxName;
                invoice.CustomerTaxId = updateDto.CustomerTaxId;
            }

            invoice.AmountPaid = updateDto.AmountPaid;
            invoice.AmountDue = updateDto.AmountDue;
            invoice.CurrencyCode = updateDto.CurrencyCode;
            invoice.CustomerName = updateDto.CustomerName;
            invoice.CustomerAddress = updateDto.CustomerAddress;
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
            OrderId = invoice.OrderId,
            OrderTaxSnapshotId = invoice.OrderTaxSnapshotId,
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
            BaseCurrencyCode = invoice.BaseCurrencyCode,
            DisplayCurrencyCode = invoice.DisplayCurrencyCode,
            ExchangeRate = invoice.ExchangeRate,
            BaseTotalAmount = invoice.BaseTotalAmount,
            ExchangeRateDate = invoice.ExchangeRateDate,
            CustomerName = invoice.CustomerName,
            CustomerAddress = invoice.CustomerAddress,
            CustomerTaxId = invoice.CustomerTaxId,
            PaymentReference = invoice.PaymentReference,
            PaymentMethod = invoice.PaymentMethod,
            Notes = invoice.Notes,
            InternalComment = invoice.InternalComment,
            InvoiceLines = invoice.InvoiceLines
                .Where(line => line.DeletedAt == null)
                .OrderBy(line => line.LineNumber)
                .Select(line => new InvoiceLineDto
                {
                    Id = line.Id,
                    InvoiceId = line.InvoiceId,
                    ServiceId = line.ServiceId,
                    UnitId = (int)(line.UnitId ?? 0),
                    LineNumber = line.LineNumber,
                    Description = line.Description,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    Discount = line.Discount,
                    TotalPrice = line.TotalPrice,
                    TaxRate = line.TaxRate,
                    TaxAmount = line.TaxAmount,
                    TotalWithTax = line.TotalWithTax,
                    ServiceNameSnapshot = line.ServiceNameSnapshot,
                    AccountingCode = line.AccountingCode,
                    CreatedAt = line.CreatedAt,
                    UpdatedAt = line.UpdatedAt,
                    DeletedAt = line.DeletedAt,
                    Notes = line.Notes
                })
                .ToList(),
            CreatedAt = invoice.CreatedAt,
            UpdatedAt = invoice.UpdatedAt,
            DeletedAt = invoice.DeletedAt
        };
    }
}

