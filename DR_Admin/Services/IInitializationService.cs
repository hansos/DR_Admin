using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IInitializationService
{
    Task<bool> IsInitializedAsync();
    Task<InitializationResponseDto?> InitializeAsync(InitializationRequestDto request);
    Task<CodeTablesResponseDto> CheckAndUpdateCodeTablesAsync();
}
