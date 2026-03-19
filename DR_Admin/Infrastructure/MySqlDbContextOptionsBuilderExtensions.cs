using Microsoft.EntityFrameworkCore.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Microsoft.EntityFrameworkCore;

public static class MySqlDbContextOptionsBuilderExtensionsCompat
{
    public static DbContextOptionsBuilder UseMySQL(
        this DbContextOptionsBuilder optionsBuilder,
        string connectionString,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        return optionsBuilder.UseMySql(connectionString, serverVersion, mySqlOptionsAction);
    }

    public static DbContextOptionsBuilder<TContext> UseMySQL<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder,
        string connectionString,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
        where TContext : DbContext
    {
        return (DbContextOptionsBuilder<TContext>)((DbContextOptionsBuilder)optionsBuilder)
            .UseMySQL(connectionString, mySqlOptionsAction);
    }

    public static DbContextOptionsBuilder UseMariaDb(
        this DbContextOptionsBuilder optionsBuilder,
        string connectionString,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        return optionsBuilder.UseMySql(connectionString, serverVersion, mySqlOptionsAction);
    }

    public static DbContextOptionsBuilder<TContext> UseMariaDb<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder,
        string connectionString,
        Action<MySqlDbContextOptionsBuilder>? mySqlOptionsAction = null)
        where TContext : DbContext
    {
        return (DbContextOptionsBuilder<TContext>)((DbContextOptionsBuilder)optionsBuilder)
            .UseMariaDb(connectionString, mySqlOptionsAction);
    }
}
