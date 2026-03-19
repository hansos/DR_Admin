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
                options.UseMySQL(connectionString);
                break;

            case "MARIADB":
            case "MARIA":
                options.UseMariaDb(connectionString);
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported database type: {databaseType}. " +
                    "Supported types are: MSSQL, POSTGRE, SQLITE, MYSQL, MARIADB");
        }
    }
}
