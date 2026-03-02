using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IPaymentInstrumentService
{
    Task<IEnumerable<PaymentInstrumentDto>> GetAllAsync();
    Task<IEnumerable<PaymentInstrumentDto>> GetActiveAsync();
    Task<PaymentInstrumentDto?> GetByIdAsync(int id);
    Task<PaymentInstrumentDto?> GetByCodeAsync(string code);
    Task<PaymentInstrumentDto> CreateAsync(CreatePaymentInstrumentDto dto);
    Task<PaymentInstrumentDto?> UpdateAsync(int id, UpdatePaymentInstrumentDto dto);
    Task<bool> DeleteAsync(int id);
}
