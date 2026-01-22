using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IInvoiceLineService
{
    Task<IEnumerable<InvoiceLineDto>> GetAllInvoiceLinesAsync();
    Task<InvoiceLineDto?> GetInvoiceLineByIdAsync(int id);
    Task<IEnumerable<InvoiceLineDto>> GetInvoiceLinesByInvoiceIdAsync(int invoiceId);
    Task<InvoiceLineDto> CreateInvoiceLineAsync(CreateInvoiceLineDto createDto);
    Task<InvoiceLineDto?> UpdateInvoiceLineAsync(int id, UpdateInvoiceLineDto updateDto);
    Task<bool> DeleteInvoiceLineAsync(int id);
}
