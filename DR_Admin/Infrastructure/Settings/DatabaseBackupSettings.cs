namespace ISPAdmin.Infrastructure.Settings;

/// <summary>
/// Configuration settings for database backup operations
/// </summary>
public class DatabaseBackupSettings
{
    /// <summary>
    /// Directory path where database backups will be stored
    /// </summary>
    public string BackupLocation { get; set; } = string.Empty;

    /// <summary>
    /// Backup frequency in hours (e.g., 24 for daily backups)
    /// </summary>
    public int FrequencyInHours { get; set; } = 24;

    /// <summary>
    /// Maximum number of backup files to retain (older backups will be deleted)
    /// </summary>
    public int MaxBackupsToKeep { get; set; } = 7;

    /// <summary>
    /// Whether the backup service is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Whether to compress backup files (SQLite only)
    /// </summary>
    public bool CompressBackup { get; set; } = true;

    /// <summary>
    /// Whether to run a backup immediately on service startup
    /// </summary>
    public bool RunOnStartup { get; set; } = false;
}
