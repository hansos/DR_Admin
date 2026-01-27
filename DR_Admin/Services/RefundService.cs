using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing refunds (stub implementation)
/// </summary>
public class RefundService : IRefundService
{
    public Task<IEnumerable<RefundDto>> GetAllRefundsAsync()
    {
        return Task.FromResult(Enumerable.Empty<RefundDto>());
    }

    public Task<RefundDto?> GetRefundByIdAsync(int id)
    {
        return Task.FromResult<RefundDto?>(null);
    }

    public Task<IEnumerable<RefundDto>> GetRefundsByInvoiceIdAsync(int invoiceId)
    {
        return Task.FromResult(Enumerable.Empty<RefundDto>());
    }

    public Task<RefundDto> CreateRefundAsync(CreateRefundDto createDto, int userId)
    {
        throw new NotImplementedException("RefundService.CreateRefundAsync not yet implemented");
    }

    public Task<bool> ProcessRefundAsync(int id)
    {
        throw new NotImplementedException("RefundService.ProcessRefundAsync not yet implemented");
    }
}

