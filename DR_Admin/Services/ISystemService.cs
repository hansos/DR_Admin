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

    /// <summary>
    /// Creates a backup of the database
    /// </summary>
    /// <param name="backupFileName">Optional custom backup file name (without extension)</param>
    /// <returns>Summary of backup results including file path</returns>
    Task<BackupResultDto> CreateBackupAsync(string? backupFileName = null);

    /// <summary>
    /// Restores the database from a backup file
    /// </summary>
    /// <param name="backupFilePath">Full path to the backup file to restore</param>
    /// <returns>Summary of restore results</returns>
    Task<RestoreResultDto> RestoreFromBackupAsync(string backupFilePath);
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

/// <summary>
/// Result of the backup operation
/// </summary>
public class BackupResultDto
{
    /// <summary>
    /// Whether the backup completed successfully
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Full path to the backup file
    /// </summary>
    public string? BackupFilePath { get; set; }

    /// <summary>
    /// Size of the backup file in bytes
    /// </summary>
    public long BackupFileSizeBytes { get; set; }

    /// <summary>
    /// Time taken to complete the backup
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Timestamp when the backup was created
    /// </summary>
    public DateTime BackupTimestamp { get; set; }
}

/// <summary>
/// Result of the restore operation
/// </summary>
public class RestoreResultDto
{
    /// <summary>
    /// Whether the restore completed successfully
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Path to the backup file that was restored
    /// </summary>
    public string? RestoredFromFilePath { get; set; }

    /// <summary>
    /// Time taken to complete the restore
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Timestamp when the restore was performed
    /// </summary>
    public DateTime RestoreTimestamp { get; set; }
}
