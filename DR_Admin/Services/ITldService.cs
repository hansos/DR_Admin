using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface ITldService
{
    Task<IEnumerable<TldDto>> GetAllTldsAsync();
    Task<IEnumerable<TldDto>> GetAllTldsAsync(bool isSecondLevel);
    Task<IEnumerable<TldDto>> GetActiveTldsAsync();
    Task<TldDto?> GetTldByIdAsync(int id);
    Task<TldDto?> GetTldByExtensionAsync(string extension);
    Task<TldDto> CreateTldAsync(CreateTldDto createDto);
    Task<TldDto?> UpdateTldAsync(int id, UpdateTldDto updateDto);
    Task<bool> DeleteTldAsync(int id);
    Task<IEnumerable<RegistrarDto>> GetRegistrarsByTldAsync(int tldId);
    Task<TldSyncResponseDto> SyncTldsFromIanaAsync(TldSyncRequestDto request);
}
