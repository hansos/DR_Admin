using Microsoft.EntityFrameworkCore;
using ISPAdmin.Data;
using ISPAdmin.Infrastructure.Settings;

namespace ISPAdmin.Infrastructure;

public static class DbContextConfiguration
{
    public static void ConfigureDatabase(
        this DbContextOptionsBuilder options,
        string connectionString,
        string databaseType)
    {
        switch (databaseType.ToUpperInvariant())
        {
            case "MSSQL":
            case "SQLSERVER":
                options.UseSqlServer(connectionString);
                break;

            case "POSTGRE":
            case "POSTGRESQL":
                options.UseNpgsql(connectionString);
                break;

            case "SQLITE":
            case "LITESQL":
                options.UseSqlite(connectionString);
                break;

            case "MYSQL":
                throw new NotSupportedException(
                    "MySQL is not currently supported with EF Core 10. " +
                    "The Pomelo.EntityFrameworkCore.MySql package is not yet compatible with EF Core 10. " +
                    "Supported database types are: MSSQL, POSTGRE, SQLITE");

            default:
                throw new InvalidOperationException(
                    $"Unsupported database type: {databaseType}. " +
                    "Supported types are: MSSQL, POSTGRE, SQLITE");
        }
    }
}
