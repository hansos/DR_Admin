using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();
    Task<PagedResult<InvoiceDto>> GetAllInvoicesPagedAsync(PaginationParameters parameters);
    Task<IEnumerable<InvoiceDto>> GetInvoicesByCustomerIdAsync(int customerId);
    Task<PagedResult<InvoiceDto>> GetInvoicesByCustomerIdPagedAsync(int customerId, PaginationParameters parameters);
    Task<IEnumerable<InvoiceDto>> GetInvoicesByStatusAsync(Data.Enums.InvoiceStatus status);
    Task<InvoiceDto?> GetInvoiceByIdAsync(int id);
    Task<InvoiceDto?> GetInvoiceByNumberAsync(string invoiceNumber);
    Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createDto);
    Task<InvoiceDto?> UpdateInvoiceAsync(int id, UpdateInvoiceDto updateDto);
    Task<bool> DeleteInvoiceAsync(int id);
}

