using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface ISoldHostingPackageService
{
    Task<IEnumerable<SoldHostingPackageDto>> GetAllAsync();
    Task<IEnumerable<SoldHostingPackageDto>> GetByCustomerIdAsync(int customerId);
    Task<SoldHostingPackageDto?> GetByIdAsync(int id);
    Task<SoldHostingPackageDto> CreateAsync(CreateSoldHostingPackageDto createDto);
    Task<SoldHostingPackageDto?> UpdateAsync(int id, UpdateSoldHostingPackageDto updateDto);
    Task<bool> DeleteAsync(int id);
}
