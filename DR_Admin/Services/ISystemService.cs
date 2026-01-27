namespace ISPAdmin.Services;

/// <summary>
/// Service interface for system-level operations including data normalization
/// </summary>
public interface ISystemService
{
    /// <summary>
    /// Normalizes all records in the database by updating normalized fields
    /// </summary>
    /// <returns>Summary of normalization results</returns>
    Task<NormalizationResultDto> NormalizeAllRecordsAsync();
}

/// <summary>
/// Result of the normalization operation
/// </summary>
public class NormalizationResultDto
{
    /// <summary>
    /// Total number of records processed
    /// </summary>
    public int TotalRecordsProcessed { get; set; }

    /// <summary>
    /// Breakdown of records processed by entity type
    /// </summary>
    public Dictionary<string, int> RecordsByEntity { get; set; } = new();

    /// <summary>
    /// Time taken to complete the normalization
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Whether the operation completed successfully
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}
