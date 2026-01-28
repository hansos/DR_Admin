using System.Diagnostics;
using ISPAdmin.Data;
using ISPAdmin.Utilities;
using ISPAdmin.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Npgsql;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for system-level operations including data normalization
/// </summary>
public class SystemService : ISystemService
{
    private readonly ApplicationDbContext _context;
    private readonly AppSettings _appSettings;
    private static readonly Serilog.ILogger _log = Log.ForContext<SystemService>();

    public SystemService(ApplicationDbContext context, AppSettings appSettings)
    {
        _context = context;
        _appSettings = appSettings;
    }

    /// <summary>
    /// Normalizes all records in the database by updating normalized fields
    /// </summary>
    public async Task<NormalizationResultDto> NormalizeAllRecordsAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new NormalizationResultDto
        {
            RecordsByEntity = new Dictionary<string, int>()
        };

        try
        {
            _log.Information("Starting normalization of all records");

            // Normalize Countries
            var countryCount = await NormalizeCountriesAsync();
            result.RecordsByEntity["Country"] = countryCount;
            _log.Information("Normalized {Count} countries", countryCount);

            // Normalize Coupons
            var couponCount = await NormalizeCouponsAsync();
            result.RecordsByEntity["Coupon"] = couponCount;
            _log.Information("Normalized {Count} coupons", couponCount);

            // Normalize Customers
            var customerCount = await NormalizeCustomersAsync();
            result.RecordsByEntity["Customer"] = customerCount;
            _log.Information("Normalized {Count} customers", customerCount);

            // Normalize Domains
            var domainCount = await NormalizeDomainsAsync();
            result.RecordsByEntity["Domain"] = domainCount;
            _log.Information("Normalized {Count} domains", domainCount);

            // Normalize HostingPackages
            var hostingPackageCount = await NormalizeHostingPackagesAsync();
            result.RecordsByEntity["HostingPackage"] = hostingPackageCount;
            _log.Information("Normalized {Count} hosting packages", hostingPackageCount);

            // Normalize PaymentGateways
            var paymentGatewayCount = await NormalizePaymentGatewaysAsync();
            result.RecordsByEntity["PaymentGateway"] = paymentGatewayCount;
            _log.Information("Normalized {Count} payment gateways", paymentGatewayCount);

            // Normalize PostalCodes
            var postalCodeCount = await NormalizePostalCodesAsync();
            result.RecordsByEntity["PostalCode"] = postalCodeCount;
            _log.Information("Normalized {Count} postal codes", postalCodeCount);

            // Normalize Registrars
            var registrarCount = await NormalizeRegistrarsAsync();
            result.RecordsByEntity["Registrar"] = registrarCount;
            _log.Information("Normalized {Count} registrars", registrarCount);

            // Normalize SalesAgents
            var salesAgentCount = await NormalizeSalesAgentsAsync();
            result.RecordsByEntity["SalesAgent"] = salesAgentCount;
            _log.Information("Normalized {Count} sales agents", salesAgentCount);

            // Normalize Users
            var userCount = await NormalizeUsersAsync();
            result.RecordsByEntity["User"] = userCount;
            _log.Information("Normalized {Count} users", userCount);

            result.TotalRecordsProcessed = result.RecordsByEntity.Values.Sum();
            result.Success = true;

            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;

            _log.Information("Successfully normalized {TotalCount} records in {Duration}ms", 
                result.TotalRecordsProcessed, result.Duration.TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
            result.Success = false;
            result.ErrorMessage = ex.Message;

            _log.Error(ex, "Error occurred during normalization after {Duration}ms", result.Duration.TotalMilliseconds);
            return result;
        }
    }

    private async Task<int> NormalizeCountriesAsync()
    {
        var countries = await _context.Countries.ToListAsync();
        
        foreach (var country in countries)
        {
            country.NormalizedEnglishName = NormalizationHelper.Normalize(country.EnglishName) ?? string.Empty;
            country.NormalizedLocalName = NormalizationHelper.Normalize(country.LocalName) ?? string.Empty;
        }

        await _context.SaveChangesAsync();
        return countries.Count;
    }

    private async Task<int> NormalizeCouponsAsync()
    {
        var coupons = await _context.Coupons.ToListAsync();
        
        foreach (var coupon in coupons)
        {
            coupon.NormalizedName = NormalizationHelper.Normalize(coupon.Name) ?? string.Empty;
        }

        await _context.SaveChangesAsync();
        return coupons.Count;
    }

    private async Task<int> NormalizeCustomersAsync()
    {
        var customers = await _context.Customers.ToListAsync();
        
        foreach (var customer in customers)
        {
            customer.NormalizedName = NormalizationHelper.Normalize(customer.Name) ?? string.Empty;
            customer.NormalizedCustomerName = NormalizationHelper.Normalize(customer.CustomerName);
            customer.NormalizedContactPerson = NormalizationHelper.Normalize(customer.ContactPerson);
        }

        await _context.SaveChangesAsync();
        return customers.Count;
    }

    private async Task<int> NormalizeDomainsAsync()
    {
        var domains = await _context.Domains.ToListAsync();
        
        foreach (var domain in domains)
        {
            domain.NormalizedName = NormalizationHelper.Normalize(domain.Name) ?? string.Empty;
        }

        await _context.SaveChangesAsync();
        return domains.Count;
    }

    private async Task<int> NormalizeHostingPackagesAsync()
    {
        var hostingPackages = await _context.HostingPackages.ToListAsync();
        
        foreach (var package in hostingPackages)
        {
            package.NormalizedName = NormalizationHelper.Normalize(package.Name) ?? string.Empty;
        }

        await _context.SaveChangesAsync();
        return hostingPackages.Count;
    }

    private async Task<int> NormalizePaymentGatewaysAsync()
    {
        var paymentGateways = await _context.PaymentGateways.ToListAsync();
        
        foreach (var gateway in paymentGateways)
        {
            gateway.NormalizedName = NormalizationHelper.Normalize(gateway.Name) ?? string.Empty;
        }

        await _context.SaveChangesAsync();
        return paymentGateways.Count;
    }

    private async Task<int> NormalizePostalCodesAsync()
    {
        var postalCodes = await _context.PostalCodes.ToListAsync();
        
        foreach (var postalCode in postalCodes)
        {
            postalCode.NormalizedCode = NormalizationHelper.Normalize(postalCode.Code) ?? string.Empty;
            postalCode.NormalizedCountryCode = NormalizationHelper.Normalize(postalCode.CountryCode) ?? string.Empty;
            postalCode.NormalizedCity = NormalizationHelper.Normalize(postalCode.City) ?? string.Empty;
            postalCode.NormalizedState = NormalizationHelper.Normalize(postalCode.State);
            postalCode.NormalizedRegion = NormalizationHelper.Normalize(postalCode.Region);
            postalCode.NormalizedDistrict = NormalizationHelper.Normalize(postalCode.District);
        }

        await _context.SaveChangesAsync();
        return postalCodes.Count;
    }

    private async Task<int> NormalizeRegistrarsAsync()
    {
        var registrars = await _context.Registrars.ToListAsync();
        
        foreach (var registrar in registrars)
        {
            registrar.NormalizedName = NormalizationHelper.Normalize(registrar.Name) ?? string.Empty;
        }

        await _context.SaveChangesAsync();
        return registrars.Count;
    }

    private async Task<int> NormalizeSalesAgentsAsync()
    {
        var salesAgents = await _context.SalesAgents.ToListAsync();
        
        foreach (var agent in salesAgents)
        {
            agent.NormalizedFirstName = NormalizationHelper.Normalize(agent.FirstName) ?? string.Empty;
            agent.NormalizedLastName = NormalizationHelper.Normalize(agent.LastName) ?? string.Empty;
        }

        await _context.SaveChangesAsync();
        return salesAgents.Count;
    }

    private async Task<int> NormalizeUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        
        foreach (var user in users)
        {
            user.NormalizedUsername = NormalizationHelper.Normalize(user.Username) ?? string.Empty;
        }

        await _context.SaveChangesAsync();
        return users.Count;
    }

    /// <summary>
    /// Creates a backup of the database
    /// </summary>
    public async Task<BackupResultDto> CreateBackupAsync(string? backupFileName = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new BackupResultDto
        {
            BackupTimestamp = DateTime.UtcNow
        };

        try
        {
            _log.Information("Starting database backup");

            var databaseType = _appSettings.DbSettings.DatabaseType.ToUpperInvariant();
            var connectionString = _appSettings.DefaultConnection;

            // Create backups directory if it doesn't exist
            var backupsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Backups");
            Directory.CreateDirectory(backupsDirectory);

            // Generate backup filename
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var fileName = string.IsNullOrWhiteSpace(backupFileName) 
                ? $"backup_{timestamp}" 
                : $"{backupFileName}_{timestamp}";

            switch (databaseType)
            {
                case "SQLITE":
                case "LITESQL":
                    result = await BackupSqliteAsync(connectionString, backupsDirectory, fileName, result);
                    break;

                case "MSSQL":
                case "SQLSERVER":
                    result = await BackupSqlServerAsync(connectionString, backupsDirectory, fileName, result);
                    break;

                case "POSTGRE":
                case "POSTGRESQL":
                    result = await BackupPostgreSqlAsync(connectionString, backupsDirectory, fileName, result);
                    break;

                default:
                    throw new NotSupportedException($"Backup not supported for database type: {databaseType}");
            }

            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;

            if (result.Success)
            {
                _log.Information("Successfully created backup at {BackupPath} in {Duration}ms", 
                    result.BackupFilePath, result.Duration.TotalMilliseconds);
            }

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
            result.Success = false;
            result.ErrorMessage = ex.Message;

            _log.Error(ex, "Error occurred during backup after {Duration}ms", result.Duration.TotalMilliseconds);
            return result;
        }
    }

    /// <summary>
    /// Restores the database from a backup file
    /// </summary>
    public async Task<RestoreResultDto> RestoreFromBackupAsync(string backupFilePath)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new RestoreResultDto
        {
            RestoreTimestamp = DateTime.UtcNow,
            RestoredFromFilePath = backupFilePath
        };

        try
        {
            _log.Information("Starting database restore from {BackupPath}", backupFilePath);

            if (!File.Exists(backupFilePath))
            {
                throw new FileNotFoundException($"Backup file not found: {backupFilePath}");
            }

            var databaseType = _appSettings.DbSettings.DatabaseType.ToUpperInvariant();
            var connectionString = _appSettings.DefaultConnection;

            switch (databaseType)
            {
                case "SQLITE":
                case "LITESQL":
                    result = await RestoreSqliteAsync(connectionString, backupFilePath, result);
                    break;

                case "MSSQL":
                case "SQLSERVER":
                    result = await RestoreSqlServerAsync(connectionString, backupFilePath, result);
                    break;

                case "POSTGRE":
                case "POSTGRESQL":
                    result = await RestorePostgreSqlAsync(connectionString, backupFilePath, result);
                    break;

                default:
                    throw new NotSupportedException($"Restore not supported for database type: {databaseType}");
            }

            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;

            if (result.Success)
            {
                _log.Information("Successfully restored database from {BackupPath} in {Duration}ms", 
                    backupFilePath, result.Duration.TotalMilliseconds);
            }

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
            result.Success = false;
            result.ErrorMessage = ex.Message;

            _log.Error(ex, "Error occurred during restore after {Duration}ms", result.Duration.TotalMilliseconds);
            return result;
        }
    }

    private async Task<BackupResultDto> BackupSqliteAsync(string connectionString, string backupsDirectory, string fileName, BackupResultDto result)
    {
        // Extract database file path from connection string
        var dbPath = ExtractSqliteDbPath(connectionString);
        
        if (string.IsNullOrEmpty(dbPath) || !File.Exists(dbPath))
        {
            throw new InvalidOperationException($"SQLite database file not found: {dbPath}");
        }

        var backupPath = Path.Combine(backupsDirectory, $"{fileName}.db");

        // Close all connections before backup
        await _context.Database.CloseConnectionAsync();

        // Copy the database file
        File.Copy(dbPath, backupPath, overwrite: true);

        var fileInfo = new FileInfo(backupPath);
        result.Success = true;
        result.BackupFilePath = backupPath;
        result.BackupFileSizeBytes = fileInfo.Length;

        return result;
    }

    private async Task<BackupResultDto> BackupSqlServerAsync(string connectionString, string backupsDirectory, string fileName, BackupResultDto result)
    {
        var backupPath = Path.Combine(backupsDirectory, $"{fileName}.bak");

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;

        var backupCommand = $@"
            BACKUP DATABASE [{databaseName}]
            TO DISK = @BackupPath
            WITH FORMAT, INIT, COMPRESSION,
            NAME = N'{fileName}',
            SKIP, NOREWIND, NOUNLOAD, STATS = 10";

        using var command = new SqlCommand(backupCommand, connection);
        command.CommandTimeout = 300; // 5 minutes
        command.Parameters.AddWithValue("@BackupPath", backupPath);

        await command.ExecuteNonQueryAsync();

        var fileInfo = new FileInfo(backupPath);
        result.Success = true;
        result.BackupFilePath = backupPath;
        result.BackupFileSizeBytes = fileInfo.Length;

        return result;
    }

    private async Task<BackupResultDto> BackupPostgreSqlAsync(string connectionString, string backupsDirectory, string fileName, BackupResultDto result)
    {
        var backupPath = Path.Combine(backupsDirectory, $"{fileName}.backup");

        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;
        var host = builder.Host;
        var port = builder.Port;
        var username = builder.Username;
        var password = builder.Password;

        // Use pg_dump for PostgreSQL backup
        var pgDumpPath = "pg_dump"; // Assumes pg_dump is in PATH
        var arguments = $"-h {host} -p {port} -U {username} -F c -b -v -f \"{backupPath}\" {databaseName}";

        var processStartInfo = new ProcessStartInfo
        {
            FileName = pgDumpPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // Set PGPASSWORD environment variable for authentication
        processStartInfo.Environment["PGPASSWORD"] = password;

        using var process = Process.Start(processStartInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start pg_dump process");
        }

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync();
            throw new InvalidOperationException($"pg_dump failed: {error}");
        }

        var fileInfo = new FileInfo(backupPath);
        result.Success = true;
        result.BackupFilePath = backupPath;
        result.BackupFileSizeBytes = fileInfo.Length;

        return result;
    }

    private async Task<RestoreResultDto> RestoreSqliteAsync(string connectionString, string backupFilePath, RestoreResultDto result)
    {
        // Extract database file path from connection string
        var dbPath = ExtractSqliteDbPath(connectionString);
        
        if (string.IsNullOrEmpty(dbPath))
        {
            throw new InvalidOperationException("Could not extract database path from connection string");
        }

        // Close all connections before restore
        await _context.Database.CloseConnectionAsync();

        // Create a backup of current database before restoring
        if (File.Exists(dbPath))
        {
            var preRestoreBackup = $"{dbPath}.pre-restore-{DateTime.UtcNow:yyyyMMdd_HHmmss}.bak";
            File.Copy(dbPath, preRestoreBackup, overwrite: false);
            _log.Information("Created pre-restore backup at {BackupPath}", preRestoreBackup);
        }

        // Restore by copying the backup file
        File.Copy(backupFilePath, dbPath, overwrite: true);

        result.Success = true;
        return result;
    }

    private async Task<RestoreResultDto> RestoreSqlServerAsync(string connectionString, string backupFilePath, RestoreResultDto result)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;

        // Set database to single user mode to disconnect all users
        var setSingleUserCommand = $@"
            ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";

        using (var command = new SqlCommand(setSingleUserCommand, connection))
        {
            await command.ExecuteNonQueryAsync();
        }

        try
        {
            // Restore the database
            var restoreCommand = $@"
                RESTORE DATABASE [{databaseName}]
                FROM DISK = @BackupPath
                WITH REPLACE, STATS = 10";

            using (var command = new SqlCommand(restoreCommand, connection))
            {
                command.CommandTimeout = 300; // 5 minutes
                command.Parameters.AddWithValue("@BackupPath", backupFilePath);
                await command.ExecuteNonQueryAsync();
            }

            result.Success = true;
        }
        finally
        {
            // Set database back to multi-user mode
            var setMultiUserCommand = $@"
                ALTER DATABASE [{databaseName}] SET MULTI_USER";

            using (var command = new SqlCommand(setMultiUserCommand, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        return result;
    }

    private async Task<RestoreResultDto> RestorePostgreSqlAsync(string connectionString, string backupFilePath, RestoreResultDto result)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;
        var host = builder.Host;
        var port = builder.Port;
        var username = builder.Username;
        var password = builder.Password;

        // Use pg_restore for PostgreSQL restore
        var pgRestorePath = "pg_restore"; // Assumes pg_restore is in PATH
        var arguments = $"-h {host} -p {port} -U {username} -d {databaseName} -c -v \"{backupFilePath}\"";

        var processStartInfo = new ProcessStartInfo
        {
            FileName = pgRestorePath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // Set PGPASSWORD environment variable for authentication
        processStartInfo.Environment["PGPASSWORD"] = password;

        using var process = Process.Start(processStartInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start pg_restore process");
        }

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync();
            throw new InvalidOperationException($"pg_restore failed: {error}");
        }

        result.Success = true;
        return result;
    }

    private string ExtractSqliteDbPath(string connectionString)
    {
        // SQLite connection string format: "Data Source=path/to/database.db"
        var dataSourcePrefix = "Data Source=";
        var startIndex = connectionString.IndexOf(dataSourcePrefix, StringComparison.OrdinalIgnoreCase);
        
        if (startIndex == -1)
        {
            return string.Empty;
        }

        startIndex += dataSourcePrefix.Length;
        var endIndex = connectionString.IndexOf(';', startIndex);
        
        var dbPath = endIndex == -1 
            ? connectionString.Substring(startIndex).Trim() 
            : connectionString.Substring(startIndex, endIndex - startIndex).Trim();

        return dbPath;
    }
}
