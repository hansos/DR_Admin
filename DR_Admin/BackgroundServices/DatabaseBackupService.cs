using ISPAdmin.Data;
using ISPAdmin.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO.Compression;

namespace ISPAdmin.BackgroundServices;

/// <summary>
/// Background service that performs automated database backups
/// </summary>
public class DatabaseBackupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DatabaseBackupSettings _backupSettings;
    private readonly AppSettings _appSettings;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<DatabaseBackupService>();

    public DatabaseBackupService(
        IServiceProvider serviceProvider,
        DatabaseBackupSettings backupSettings,
        AppSettings appSettings)
    {
        _serviceProvider = serviceProvider;
        _backupSettings = backupSettings;
        _appSettings = appSettings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_backupSettings.Enabled)
        {
            _log.Information("Database Backup Service is disabled");
            return;
        }

        _log.Information("Database Backup Service starting. Backup frequency: {Hours} hours, Location: {Location}, RunOnStartup: {RunOnStartup}",
            _backupSettings.FrequencyInHours, _backupSettings.BackupLocation, _backupSettings.RunOnStartup);

        // Wait a bit before starting to allow other services to initialize
        await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);

        // Run backup on startup if configured
        if (_backupSettings.RunOnStartup)
        {
            _log.Information("RunOnStartup is enabled, performing initial backup");
            try
            {
                await PerformBackupAsync(stoppingToken);
                _log.Information("Initial startup backup completed successfully");
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error during startup backup");
            }

            // Wait for the configured interval before next backup
            _log.Information("Waiting {Hours} hours before next scheduled backup", _backupSettings.FrequencyInHours);
            await Task.Delay(TimeSpan.FromHours(_backupSettings.FrequencyInHours), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformBackupAsync(stoppingToken);

                _log.Information("Database backup completed successfully. Next backup in {Hours} hours",
                    _backupSettings.FrequencyInHours);

                await Task.Delay(TimeSpan.FromHours(_backupSettings.FrequencyInHours), stoppingToken);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error during database backup");
                // Retry in 1 hour on error
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        _log.Information("Database Backup Service stopping");
    }

    private async Task PerformBackupAsync(CancellationToken cancellationToken)
    {
        _log.Information("Starting database backup");

        // Ensure backup directory exists
        if (string.IsNullOrWhiteSpace(_backupSettings.BackupLocation))
        {
            _log.Error("Backup location is not configured");
            return;
        }

        Directory.CreateDirectory(_backupSettings.BackupLocation);

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var databaseType = _appSettings.DbSettings.DatabaseType.ToUpper();

        try
        {
            if (databaseType == "SQLITE")
            {
                await BackupSqliteAsync(timestamp, cancellationToken);
            }
            else if (databaseType == "SQLSERVER")
            {
                await BackupSqlServerAsync(timestamp, cancellationToken);
            }
            else if (databaseType == "POSTGRESQL")
            {
                await BackupPostgreSqlAsync(timestamp, cancellationToken);
            }
            else if (databaseType == "MYSQL")
            {
                await BackupMySqlAsync(timestamp, cancellationToken);
            }
            else
            {
                _log.Warning("Unsupported database type for backup: {DatabaseType}", databaseType);
            }

            // Clean up old backups
            CleanupOldBackups();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Failed to perform database backup");
            throw;
        }
    }

    private async Task BackupSqliteAsync(string timestamp, CancellationToken cancellationToken)
    {
        var sourcePath = ExtractSqliteFilePath(_appSettings.DefaultConnection);
        if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
        {
            _log.Error("SQLite database file not found: {Path}", sourcePath);
            return;
        }

        var backupFileName = $"backup_{timestamp}.db";
        var backupPath = Path.Combine(_backupSettings.BackupLocation, backupFileName);

        _log.Information("Backing up SQLite database from {Source} to {Destination}", sourcePath, backupPath);

        // Use EF Core to perform a proper SQLite backup using the backup API
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken);

        try
        {
            // Use SQLite backup command
            var backupCommand = connection.CreateCommand();
            backupCommand.CommandText = $"VACUUM INTO '{backupPath}'";
            await backupCommand.ExecuteNonQueryAsync(cancellationToken);
            
            _log.Information("SQLite database backup created successfully");
        }
        finally
        {
            await connection.CloseAsync();
        }

        // Compress if enabled
        if (_backupSettings.CompressBackup)
        {
            var compressedPath = $"{backupPath}.gz";
            _log.Information("Compressing backup to {CompressedPath}", compressedPath);

            await using (var sourceStream = File.OpenRead(backupPath))
            await using (var destinationStream = File.Create(compressedPath))
            await using (var compressionStream = new GZipStream(destinationStream, CompressionMode.Compress))
            {
                await sourceStream.CopyToAsync(compressionStream, cancellationToken);
            }

            // Delete uncompressed backup
            File.Delete(backupPath);
            _log.Information("SQLite backup compressed successfully");
        }

        _log.Information("SQLite database backup completed");
    }

    private async Task BackupSqlServerAsync(string timestamp, CancellationToken cancellationToken)
    {
        var backupFileName = $"backup_{timestamp}.bak";
        var backupPath = Path.Combine(_backupSettings.BackupLocation, backupFileName);

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var databaseName = context.Database.GetDbConnection().Database;

        _log.Information("Backing up SQL Server database {DatabaseName} to {BackupPath}", databaseName, backupPath);

        var backupCommand = $"BACKUP DATABASE [{databaseName}] TO DISK = '{backupPath}' WITH FORMAT, INIT, SKIP, NOREWIND, NOUNLOAD, STATS = 10";

        await context.Database.ExecuteSqlRawAsync(backupCommand, cancellationToken);

        _log.Information("SQL Server database backup completed");
    }

    private async Task BackupPostgreSqlAsync(string timestamp, CancellationToken cancellationToken)
    {
        var backupFileName = $"backup_{timestamp}.dump";
        var backupPath = Path.Combine(_backupSettings.BackupLocation, backupFileName);

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var connectionString = context.Database.GetConnectionString();
        var databaseName = context.Database.GetDbConnection().Database;

        _log.Information("Backing up PostgreSQL database {DatabaseName} to {BackupPath}", databaseName, backupPath);

        // Note: This requires pg_dump to be available in the system PATH
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "pg_dump",
            Arguments = $"-F c -b -v -f \"{backupPath}\" {databaseName}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = System.Diagnostics.Process.Start(startInfo);
        if (process != null)
        {
            await process.WaitForExitAsync(cancellationToken);
            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync(cancellationToken);
                _log.Error("PostgreSQL backup failed: {Error}", error);
                throw new Exception($"PostgreSQL backup failed: {error}");
            }
        }

        _log.Information("PostgreSQL database backup completed");
    }

    private async Task BackupMySqlAsync(string timestamp, CancellationToken cancellationToken)
    {
        var backupFileName = $"backup_{timestamp}.sql";
        var backupPath = Path.Combine(_backupSettings.BackupLocation, backupFileName);

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var databaseName = context.Database.GetDbConnection().Database;

        _log.Information("Backing up MySQL database {DatabaseName} to {BackupPath}", databaseName, backupPath);

        // Note: This requires mysqldump to be available in the system PATH
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "mysqldump",
            Arguments = $"-u root -p {databaseName} > \"{backupPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = System.Diagnostics.Process.Start(startInfo);
        if (process != null)
        {
            await process.WaitForExitAsync(cancellationToken);
            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync(cancellationToken);
                _log.Error("MySQL backup failed: {Error}", error);
                throw new Exception($"MySQL backup failed: {error}");
            }
        }

        _log.Information("MySQL database backup completed");
    }

    private void CleanupOldBackups()
    {
        try
        {
            if (_backupSettings.MaxBackupsToKeep <= 0)
            {
                return;
            }

            _log.Information("Cleaning up old backups, keeping {Count} most recent backups", _backupSettings.MaxBackupsToKeep);

            var backupFiles = Directory.GetFiles(_backupSettings.BackupLocation, "backup_*.*")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTimeUtc)
                .ToList();

            var filesToDelete = backupFiles.Skip(_backupSettings.MaxBackupsToKeep);

            foreach (var file in filesToDelete)
            {
                _log.Information("Deleting old backup: {FileName}", file.Name);
                file.Delete();
            }

            _log.Information("Backup cleanup completed");
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during backup cleanup");
        }
    }

    private string ExtractSqliteFilePath(string connectionString)
    {
        // Handle various SQLite connection string formats
        var parts = connectionString.Split(';');
        foreach (var part in parts)
        {
            var trimmedPart = part.Trim();
            if (trimmedPart.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
            {
                return trimmedPart.Substring("Data Source=".Length).Trim();
            }
        }

        return string.Empty;
    }
}
