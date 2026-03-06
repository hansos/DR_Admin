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

    /// <summary>
    /// Sends test emails with both plain text and HTML bodies.
    /// </summary>
    /// <param name="request">Test email request containing sender and receiver addresses.</param>
    /// <returns>Detailed test email execution report.</returns>
    Task<TestEmailResultDto> SendTestEmailAsync(TestEmailRequestDto request);

    /// <summary>
    /// Seeds core test data into selected tables when those tables are empty.
    /// </summary>
    /// <returns>Summary of seeded records grouped by table.</returns>
    Task<SeedTestDataResultDto> SeedTestDataAsync();
}

/// <summary>
/// Result of the test data seeding operation.
/// </summary>
public class SeedTestDataResultDto
{
    /// <summary>
    /// Indicates whether the seeding operation completed successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Describes the outcome of the operation.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Number of records inserted per table.
    /// </summary>
    public Dictionary<string, int> InsertedByTable { get; set; } = new();
}

/// <summary>
/// Request payload for sending test emails.
/// </summary>
public class TestEmailRequestDto
{
    /// <summary>
    /// Sender email address for the test context.
    /// </summary>
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// Receiver email address.
    /// </summary>
    public string ReceiverEmail { get; set; } = string.Empty;
}

/// <summary>
/// Detailed result report for test email sending.
/// </summary>
public class TestEmailResultDto
{
    /// <summary>
    /// Indicates if both text and html emails were sent successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// UTC timestamp when execution started.
    /// </summary>
    public DateTime StartedAtUtc { get; set; }

    /// <summary>
    /// UTC timestamp when execution completed.
    /// </summary>
    public DateTime CompletedAtUtc { get; set; }

    /// <summary>
    /// Requested sender email from the API payload.
    /// </summary>
    public string RequestedSenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// Receiver email from the API payload.
    /// </summary>
    public string ReceiverEmail { get; set; } = string.Empty;

    /// <summary>
    /// Configured preferred plugin key.
    /// </summary>
    public string PreferredPluginKey { get; set; } = string.Empty;

    /// <summary>
    /// Concrete sender implementation type.
    /// </summary>
    public string SenderImplementation { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether text email send succeeded.
    /// </summary>
    public bool TextEmailSent { get; set; }

    /// <summary>
    /// Indicates whether html email send succeeded.
    /// </summary>
    public bool HtmlEmailSent { get; set; }

    /// <summary>
    /// Error message for text email send if it failed.
    /// </summary>
    public string? TextEmailError { get; set; }

    /// <summary>
    /// Error message for html email send if it failed.
    /// </summary>
    public string? HtmlEmailError { get; set; }

    /// <summary>
    /// Informational note included in the report.
    /// </summary>
    public string Note { get; set; } = string.Empty;
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
