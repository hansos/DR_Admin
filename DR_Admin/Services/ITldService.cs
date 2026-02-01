using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface ITldService
{
    Task<IEnumerable<TldDto>> GetAllTldsAsync();
    Task<IEnumerable<TldDto>> GetAllTldsAsync(bool isSecondLevel);
    Task<PagedResult<TldDto>> GetAllTldsPagedAsync(PaginationParameters parameters);
    Task<IEnumerable<TldDto>> GetActiveTldsAsync();
    Task<PagedResult<TldDto>> GetActiveTldsPagedAsync(PaginationParameters parameters);
    Task<PagedResult<TldDto>> GetSecondLevelTldsPagedAsync(PaginationParameters parameters);
    Task<PagedResult<TldDto>> GetTopLevelTldsPagedAsync(PaginationParameters parameters);
    Task<TldDto?> GetTldByIdAsync(int id);
    Task<TldDto?> GetTldByExtensionAsync(string extension);
    Task<TldDto> CreateTldAsync(CreateTldDto createDto);
    Task<TldDto?> UpdateTldAsync(int id, UpdateTldDto updateDto);
    Task<bool> DeleteTldAsync(int id);
    Task<IEnumerable<RegistrarDto>> GetRegistrarsByTldAsync(int tldId);
    Task<TldSyncResponseDto> SyncTldsFromIanaAsync(TldSyncRequestDto request);
    Task<SecondLevelDomainSyncResponseDto> SyncSecondLevelDomainsAsync();
}
