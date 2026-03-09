using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IInitializationService
{
    /// <summary>
    /// Checks whether any user exists in the system.
    /// </summary>
    /// <returns><c>true</c> when initialization has already occurred; otherwise <c>false</c>.</returns>
    Task<bool> IsInitializedAsync();

    /// <summary>
    /// Initializes the reseller panel by creating the first administrator user.
    /// </summary>
    /// <param name="request">Initialization payload for the first administrator.</param>
    /// <returns>Initialization response when successful; otherwise <c>null</c>.</returns>
    Task<InitializationResponseDto?> InitializeAdminAsync(InitializationRequestDto request);

    /// <summary>
    /// Initializes the user panel by creating the first user, customer company, and primary contact person.
    /// </summary>
    /// <param name="request">Initialization payload for the first end-user account.</param>
    /// <returns>Initialization response when successful; otherwise <c>null</c>.</returns>
    Task<UserPanelInitializationResponseDto?> InitializeUserPanelAsync(UserPanelInitializationRequestDto request);

    /// <summary>
    /// Backward-compatible alias for administrator initialization.
    /// </summary>
    /// <param name="request">Initialization payload for the first administrator.</param>
    /// <returns>Initialization response when successful; otherwise <c>null</c>.</returns>
    Task<InitializationResponseDto?> InitializeAsync(InitializationRequestDto request);

    /// <summary>
    /// Ensures required code-table values exist.
    /// </summary>
    /// <returns>Code-table update result.</returns>
    Task<CodeTablesResponseDto> CheckAndUpdateCodeTablesAsync();
}
