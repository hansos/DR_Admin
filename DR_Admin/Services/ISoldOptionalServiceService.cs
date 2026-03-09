using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface ISoldOptionalServiceService
{
    Task<IEnumerable<SoldOptionalServiceDto>> GetAllAsync();
    Task<IEnumerable<SoldOptionalServiceDto>> GetByCustomerIdAsync(int customerId);
    Task<SoldOptionalServiceDto?> GetByIdAsync(int id);
    Task<SoldOptionalServiceDto> CreateAsync(CreateSoldOptionalServiceDto createDto);
    Task<SoldOptionalServiceDto?> UpdateAsync(int id, UpdateSoldOptionalServiceDto updateDto);
    Task<bool> DeleteAsync(int id);
}
