using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IServiceService
{
    Task<IEnumerable<ServiceDto>> GetAllServicesAsync();
    Task<ServiceDto?> GetServiceByIdAsync(int id);
    Task<ServiceDto> CreateServiceAsync(CreateServiceDto createDto);
    Task<ServiceDto?> UpdateServiceAsync(int id, UpdateServiceDto updateDto);
    Task<bool> DeleteServiceAsync(int id);
}
