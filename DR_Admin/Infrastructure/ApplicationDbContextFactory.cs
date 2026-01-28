using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ISPAdmin.Data;

namespace ISPAdmin.Infrastructure;

/// <summary>
/// Design-time factory for creating ApplicationDbContext instances during migrations
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Use SQLite for design-time (migrations)
        // This matches the Development configuration
        optionsBuilder.UseSqlite("Data Source=DR_Admin_DesignTime.db");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
