using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IInvoiceLineService
{
    Task<IEnumerable<InvoiceLineDto>> GetAllInvoiceLinesAsync();
    Task<PagedResult<InvoiceLineDto>> GetAllInvoiceLinesPagedAsync(PaginationParameters parameters);
    Task<InvoiceLineDto?> GetInvoiceLineByIdAsync(int id);
    Task<IEnumerable<InvoiceLineDto>> GetInvoiceLinesByInvoiceIdAsync(int invoiceId);
    Task<InvoiceLineDto> CreateInvoiceLineAsync(CreateInvoiceLineDto createDto);
    Task<InvoiceLineDto?> UpdateInvoiceLineAsync(int id, UpdateInvoiceLineDto updateDto);
    Task<bool> DeleteInvoiceLineAsync(int id);
}
