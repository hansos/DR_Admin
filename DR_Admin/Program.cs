using ISPAdmin.Data;
using ISPAdmin.Infrastructure;
using ISPAdmin.Infrastructure.Settings;
using ISPAdmin.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog from appsettings
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

var log = Log.ForContext<Program>();

builder.Host.UseSerilog();

// Register Serilog logger for DI
builder.Services.AddSingleton(Log.Logger);

// Configure AppSettings as singleton
var appSettings = new AppSettings
{
    DefaultConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty,
    DbSettings = builder.Configuration.GetSection("DbSettings").Get<DbSettings>() ?? new DbSettings()
};
builder.Services.AddSingleton(appSettings);

// Configure DbContext based on database type from settings
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.ConfigureDatabase(
        appSettings.DefaultConnection,
        appSettings.DbSettings.DatabaseType);
});

// Configure Authentication with JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Add services to the container.
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IServiceService, ServiceService>();
builder.Services.AddTransient<IServiceTypeService, ServiceTypeService>();
builder.Services.AddTransient<IBillingCycleService, BillingCycleService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IInvoiceService, InvoiceService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Apply database migrations on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        log.Information("Applying database migrations...");
        context.Database.Migrate();
        log.Information("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        log.Error(ex, "An error occurred while migrating the database");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Add this BEFORE UseAuthorization
app.UseAuthorization();

app.MapControllers();

try
{
    log.Information("Starting web application");
    log.Information("Using database type: {DatabaseType}", appSettings.DbSettings.DatabaseType);
    app.Run();
}
catch (Exception ex)
{
    log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

