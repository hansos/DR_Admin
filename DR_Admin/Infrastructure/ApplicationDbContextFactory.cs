using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ISPAdmin.Data;
using ISPAdmin.Infrastructure.Settings;

namespace ISPAdmin.Infrastructure;

/// <summary>
/// Design-time factory for creating ApplicationDbContext instances during migrations
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var dbSettings = configuration.GetSection("DbSettings").Get<DbSettings>() ?? new DbSettings();
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=DR_Admin_DesignTime.db";

        var databaseType = dbSettings.DatabaseType;
        var databaseTypeArg = args.FirstOrDefault(a => a.StartsWith("--dbtype=", StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(databaseTypeArg))
        {
            databaseType = databaseTypeArg.Split('=', 2)[1];
        }

        if (string.IsNullOrWhiteSpace(databaseType))
        {
            databaseType = "SQLITE";
        }

        optionsBuilder.ConfigureDatabase(connectionString, databaseType);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
