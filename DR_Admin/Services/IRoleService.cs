using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<RoleDto?> GetRoleByIdAsync(int id);
    Task<RoleDto> CreateRoleAsync(CreateRoleDto createDto);
    Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto updateDto);
    Task<bool> DeleteRoleAsync(int id);
}
