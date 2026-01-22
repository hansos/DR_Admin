using ISPAdmin.Data;
using ISPAdmin.Infrastructure;
using ISPAdmin.Infrastructure.Settings;
using ISPAdmin.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.Name
    };
});

// Add services to the container.
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IMyAccountService, MyAccountService>();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IServiceService, ServiceService>();
builder.Services.AddTransient<IServiceTypeService, ServiceTypeService>();
builder.Services.AddTransient<IBillingCycleService, BillingCycleService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IInvoiceService, InvoiceService>();

// Configure CORS from appsettings
var corsSettings = builder.Configuration.GetSection("CorsSettings");
var policyName = corsSettings["PolicyName"] ?? "AllowSpecificOrigins";
var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var allowedMethods = corsSettings.GetSection("AllowedMethods").Get<string[]>() ?? Array.Empty<string>();
var allowedHeaders = corsSettings.GetSection("AllowedHeaders").Get<string[]>() ?? Array.Empty<string>();
var allowCredentials = corsSettings.GetValue<bool>("AllowCredentials");
var maxAge = corsSettings.GetValue<int>("MaxAge");

builder.Services.AddCors(options =>
{
    options.AddPolicy(policyName, policy =>
    {
        if (allowedOrigins.Length > 0 && allowedOrigins[0] != "*")
        {
            policy.WithOrigins(allowedOrigins);
        }
        else
        {
            policy.AllowAnyOrigin();
        }

        if (allowedMethods.Length > 0 && allowedMethods[0] != "*")
        {
            policy.WithMethods(allowedMethods);
        }
        else
        {
            policy.AllowAnyMethod();
        }

        if (allowedHeaders.Length > 0 && allowedHeaders[0] != "*")
        {
            policy.WithHeaders(allowedHeaders);
        }
        else
        {
            policy.AllowAnyHeader();
        }

        if (allowCredentials)
        {
            policy.AllowCredentials();
        }

        if (maxAge > 0)
        {
            policy.SetPreflightMaxAge(TimeSpan.FromSeconds(maxAge));
        }
    });
});

builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DR Admin API",
        Version = "v1",
        Description = "ISP Admin API"
    });
    
    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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

        // Synchronize roles from controller attributes
        log.Information("Synchronizing roles from controller attributes...");
        var roleService = services.GetRequiredService<IRoleService>();
        var controllerRoles = RoleInitializationService.GetAllRolesFromControllers();
        
        log.Information("Found {Count} unique roles in controllers: {Roles}", 
            controllerRoles.Count, 
            string.Join(", ", controllerRoles));

        foreach (var roleName in controllerRoles)
        {
            await roleService.EnsureRoleExistsAsync(roleName);
        }

        log.Information("Role synchronization completed successfully");
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
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "DR Admin API v1");
    });
}

app.UseHttpsRedirection();

// Enable CORS - must be before UseAuthentication and UseAuthorization
var corsPolicy = builder.Configuration.GetSection("CorsSettings")["PolicyName"] ?? "AllowSpecificOrigins";
app.UseCors(corsPolicy);

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

