using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IRegistrarTldService
{
    Task<IEnumerable<RegistrarTldDto>> GetAllRegistrarTldsAsync();
    Task<IEnumerable<RegistrarTldDto>> GetAvailableRegistrarTldsAsync();
    Task<IEnumerable<RegistrarTldDto>> GetRegistrarTldsByRegistrarAsync(int registrarId);
    Task<IEnumerable<RegistrarTldDto>> GetRegistrarTldsByTldAsync(int tldId);
    Task<RegistrarTldDto?> GetRegistrarTldByIdAsync(int id);
    Task<RegistrarTldDto?> GetRegistrarTldByRegistrarAndTldAsync(int registrarId, int tldId);
    Task<RegistrarTldDto> CreateRegistrarTldAsync(CreateRegistrarTldDto createDto);
    Task<RegistrarTldDto?> UpdateRegistrarTldAsync(int id, UpdateRegistrarTldDto updateDto);
    Task<bool> DeleteRegistrarTldAsync(int id);
}
