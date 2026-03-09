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

    /// <summary>
    /// Exports the current admin user and MyCompany profile to a debug snapshot file.
    /// </summary>
    /// <param name="fileName">Optional snapshot file name. If omitted, a timestamped file name is generated.</param>
    /// <returns>Summary and payload for the exported debug snapshot.</returns>
    Task<AdminUserMyCompanyExportResultDto> ExportAdminUserAndMyCompanyAsync(string? fileName = null);

    /// <summary>
    /// Imports a previously exported admin user and MyCompany profile snapshot from a debug file.
    /// </summary>
    /// <param name="request">Import request containing snapshot file details.</param>
    /// <returns>Summary of the import operation.</returns>
    Task<AdminUserMyCompanyImportResultDto> ImportAdminUserAndMyCompanyAsync(AdminUserMyCompanyImportRequestDto request);

    /// <summary>
    /// Imports a customer user snapshot from a debug file for shop initialization.
    /// </summary>
    /// <param name="request">Import request containing snapshot file details.</param>
    /// <returns>Summary of the import operation.</returns>
    Task<AdminUserMyCompanyImportResultDto> ImportCustomerUserSnapshotAsync(AdminUserMyCompanyImportRequestDto request);
}

/// <summary>
/// Request payload for importing admin user and MyCompany snapshot data.
/// </summary>
public class AdminUserMyCompanyImportRequestDto
{
    /// <summary>
    /// Snapshot file name located in the debug snapshot directory.
    /// </summary>
    public string FileName { get; set; } = string.Empty;
}

/// <summary>
/// Result payload for exporting admin user and MyCompany snapshot data.
/// </summary>
public class AdminUserMyCompanyExportResultDto
{
    /// <summary>
    /// Indicates whether the export operation completed successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Full path to the generated snapshot file.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// File name of the generated snapshot file.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Exported admin user and MyCompany snapshot payload.
    /// </summary>
    public AdminUserMyCompanySnapshotDto Snapshot { get; set; } = new();

    /// <summary>
    /// Error message if export failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Result payload for importing admin user and MyCompany snapshot data.
/// </summary>
public class AdminUserMyCompanyImportResultDto
{
    /// <summary>
    /// Indicates whether the import operation completed successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Full path of the imported snapshot file.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether an admin user record was created during import.
    /// </summary>
    public bool AdminUserCreated { get; set; }

    /// <summary>
    /// Indicates whether an existing admin user record was updated during import.
    /// </summary>
    public bool AdminUserUpdated { get; set; }

    /// <summary>
    /// Indicates whether a MyCompany record was created during import.
    /// </summary>
    public bool MyCompanyCreated { get; set; }

    /// <summary>
    /// Indicates whether an existing MyCompany record was updated during import.
    /// </summary>
    public bool MyCompanyUpdated { get; set; }

    /// <summary>
    /// Indicates whether a primary contact person record was created during import.
    /// </summary>
    public bool PrimaryContactPersonCreated { get; set; }

    /// <summary>
    /// Indicates whether an existing primary contact person record was updated during import.
    /// </summary>
    public bool PrimaryContactPersonUpdated { get; set; }

    /// <summary>
    /// Error message if import failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Snapshot payload containing admin user and MyCompany data.
/// </summary>
public class AdminUserMyCompanySnapshotDto
{
    /// <summary>
    /// UTC timestamp when snapshot was created.
    /// </summary>
    public DateTime ExportedAtUtc { get; set; }

    /// <summary>
    /// Admin user snapshot data.
    /// </summary>
    public AdminUserSnapshotDto AdminUser { get; set; } = new();

    /// <summary>
    /// Optional MyCompany snapshot data.
    /// </summary>
    public MyCompanySnapshotDto? MyCompany { get; set; }

    /// <summary>
    /// Optional primary contact person snapshot data.
    /// </summary>
    public PrimaryContactPersonSnapshotDto? PrimaryContactPerson { get; set; }
}

/// <summary>
/// Snapshot payload for primary contact person data.
/// </summary>
public class PrimaryContactPersonSnapshotDto
{
    /// <summary>
    /// Gets or sets the first name of the primary contact person.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name of the primary contact person.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the primary contact person.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone number of the primary contact person.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional notes for the primary contact person.
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Snapshot payload for admin user data.
/// </summary>
public class AdminUserSnapshotDto
{
    /// <summary>
    /// Username of the admin user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the admin user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password hash of the admin user.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the admin user is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Optional UTC timestamp of email confirmation.
    /// </summary>
    public DateTime? EmailConfirmed { get; set; }

    /// <summary>
    /// Indicates whether email-based two-factor authentication is enabled.
    /// </summary>
    public bool IsMailTwoFactorEnabled { get; set; }

    /// <summary>
    /// Indicates whether authenticator-based two-factor authentication is enabled.
    /// </summary>
    public bool IsAuthenticatorTwoFactorEnabled { get; set; }

    /// <summary>
    /// Shared authenticator key if configured.
    /// </summary>
    public string? AuthenticatorKey { get; set; }
}

/// <summary>
/// Snapshot payload for MyCompany data.
/// </summary>
public class MyCompanySnapshotDto
{
    /// <summary>
    /// Gets or sets the display name of the company.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the legal registered company name.
    /// </summary>
    public string? LegalName { get; set; }

    /// <summary>
    /// Gets or sets the company email used for communication.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the company phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the first address line.
    /// </summary>
    public string? AddressLine1 { get; set; }

    /// <summary>
    /// Gets or sets the second address line.
    /// </summary>
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the state or region.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the country code.
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// Gets or sets organization number.
    /// </summary>
    public string? OrganizationNumber { get; set; }

    /// <summary>
    /// Gets or sets tax identifier.
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// Gets or sets VAT number.
    /// </summary>
    public string? VatNumber { get; set; }

    /// <summary>
    /// Gets or sets invoice email.
    /// </summary>
    public string? InvoiceEmail { get; set; }

    /// <summary>
    /// Gets or sets website URL.
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Gets or sets logo URL.
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Gets or sets letterhead footer text.
    /// </summary>
    public string? LetterheadFooter { get; set; }
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
