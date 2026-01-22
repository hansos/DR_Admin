using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IUnitService
{
    Task<IEnumerable<UnitDto>> GetAllUnitsAsync();
    Task<UnitDto?> GetUnitByIdAsync(int id);
    Task<UnitDto?> GetUnitByCodeAsync(string code);
    Task<UnitDto> CreateUnitAsync(CreateUnitDto createDto);
    Task<UnitDto?> UpdateUnitAsync(int id, UpdateUnitDto updateDto);
    Task<bool> DeleteUnitAsync(int id);
}
