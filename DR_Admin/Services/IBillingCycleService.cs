using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IBillingCycleService
{
    Task<IEnumerable<BillingCycleDto>> GetAllBillingCyclesAsync();
    Task<BillingCycleDto?> GetBillingCycleByIdAsync(int id);
    Task<BillingCycleDto> CreateBillingCycleAsync(CreateBillingCycleDto createDto);
    Task<BillingCycleDto?> UpdateBillingCycleAsync(int id, UpdateBillingCycleDto updateDto);
    Task<bool> DeleteBillingCycleAsync(int id);
}
