using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IInitializationService
{
    Task<InitializationResponseDto?> InitializeAsync(InitializationRequestDto request);
}
