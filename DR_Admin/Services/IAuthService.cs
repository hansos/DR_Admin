using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> AuthenticateAsync(string username, string password);
    Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken);
}
