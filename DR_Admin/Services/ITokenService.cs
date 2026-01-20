using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface ITokenService
{
    Task<IEnumerable<TokenDto>> GetAllTokensAsync();
    Task<TokenDto?> GetTokenByIdAsync(int id);
    Task<TokenDto> CreateTokenAsync(CreateTokenDto createDto);
    Task<TokenDto?> UpdateTokenAsync(int id, UpdateTokenDto updateDto);
    Task<bool> DeleteTokenAsync(int id);
}
