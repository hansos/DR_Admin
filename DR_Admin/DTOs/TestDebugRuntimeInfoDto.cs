namespace ISPAdmin.DTOs;

/// <summary>
/// Provides debug-only runtime details used by the reseller debug help page.
/// </summary>
public class TestDebugRuntimeInfoDto
{
    /// <summary>
    /// Human-readable description of how the primary application database is configured.
    /// </summary>
    public string DatabaseConnectionDescription { get; set; } = string.Empty;

    /// <summary>
    /// Resolved path to the simulator registrar storage file.
    /// </summary>
    public string SimulatorRegistrarDatabasePath { get; set; } = string.Empty;

    /// <summary>
    /// Resolved path to the customer user import snapshot file.
    /// </summary>
    public string UserJsonImportFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Resolved path to the admin user import snapshot file.
    /// </summary>
    public string AdminJsonImportFilePath { get; set; } = string.Empty;
}
