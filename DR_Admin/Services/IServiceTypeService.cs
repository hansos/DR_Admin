using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IServiceTypeService
{
    Task<IEnumerable<ServiceTypeDto>> GetAllServiceTypesAsync();
    Task<ServiceTypeDto?> GetServiceTypeByIdAsync(int id);
    Task<ServiceTypeDto?> GetServiceTypeByNameAsync(string name);
    Task<ServiceTypeDto> CreateServiceTypeAsync(CreateServiceTypeDto createDto);
    Task<ServiceTypeDto?> UpdateServiceTypeAsync(int id, UpdateServiceTypeDto updateDto);
    Task<bool> DeleteServiceTypeAsync(int id);
}
