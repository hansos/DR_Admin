using ISPAdmin.Data;
using ISPAdmin.Infrastructure;
using ISPAdmin.Infrastructure.Settings;
using ISPAdmin.Services;
using ISPAdmin.BackgroundServices;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Channels;
using ISPAdmin.Workflow.Domain.EventHandlers;
using ISPAdmin.Workflow.Domain.Services;
using ISPAdmin.Workflow.Domain.Workflows;
using ISPAdmin.Workflow.Domain.Events.DomainEvents;
using ISPAdmin.Workflow.Domain.Events.InvoiceEvents;
using ISPAdmin.Workflow.Domain.Events.OrderEvents;

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
    
    // Temporarily suppress pending model changes warning during development
    options.ConfigureWarnings(warnings => 
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// Register Email Queue Channel as singleton
builder.Services.AddSingleton(Channel.CreateUnbounded<int>(new UnboundedChannelOptions
{
    SingleReader = false, // Allow multiple background service instances if scaled
    SingleWriter = false
}));

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

// Register DB Role to Claims Transformation
builder.Services.AddScoped<Microsoft.AspNetCore.Authentication.IClaimsTransformation, ISPAdmin.Infrastructure.DbRoleClaimsTransformation>();

// Register Role Permission Service
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ISPAdmin.Infrastructure.IRolePermissionService, ISPAdmin.Infrastructure.RolePermissionService>();

// Register Role Sync Service for runtime role updates
builder.Services.AddScoped<ISPAdmin.Infrastructure.IRoleSyncService, ISPAdmin.Infrastructure.RoleSyncService>();

// Register Authorization Handlers
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, ISPAdmin.Infrastructure.Authorization.ResourcePermissionHandler>();
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, ISPAdmin.Infrastructure.Authorization.ResourceOwnerHandler>();

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.ConfigureAuthorizationPolicies();
});

// Add services to the container.
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IInitializationService, InitializationService>();
builder.Services.AddTransient<IMyAccountService, MyAccountService>();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<ICustomerStatusService, CustomerStatusService>();
builder.Services.AddTransient<ICustomerAddressService, CustomerAddressService>();
builder.Services.AddTransient<IAddressTypeService, AddressTypeService>();
builder.Services.AddTransient<IContactPersonService, ContactPersonService>();
builder.Services.AddTransient<IRegistrarMailAddressService, RegistrarMailAddressService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IServiceService, ServiceService>();
builder.Services.AddTransient<IServiceTypeService, ServiceTypeService>();
builder.Services.AddTransient<IBillingCycleService, BillingCycleService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IInvoiceService, InvoiceService>();
builder.Services.AddTransient<IInvoiceLineService, InvoiceLineService>();
builder.Services.AddTransient<IUnitService, UnitService>();
builder.Services.AddTransient<ICountryService, CountryService>();
builder.Services.AddTransient<IPostalCodeService, PostalCodeService>();
builder.Services.AddTransient<ITldService, TldService>();
builder.Services.AddTransient<IRegistrarService, RegistrarService>();
builder.Services.AddTransient<IRegistrarTldService, RegistrarTldService>();
builder.Services.AddTransient<IDomainContactService, DomainContactService>();
builder.Services.AddTransient<IDnsRecordTypeService, DnsRecordTypeService>();
builder.Services.AddTransient<IDnsRecordService, DnsRecordService>();
builder.Services.AddTransient<INameServerService, NameServerService>();
builder.Services.AddTransient<IDnsZonePackageService, DnsZonePackageService>();
builder.Services.AddTransient<IDnsZonePackageRecordService, DnsZonePackageRecordService>();
builder.Services.AddTransient<IServerService, ServerService>();
builder.Services.AddTransient<IServerIpAddressService, ServerIpAddressService>();
builder.Services.AddTransient<IControlPanelTypeService, ControlPanelTypeService>();
builder.Services.AddTransient<IServerControlPanelService, ServerControlPanelService>();
builder.Services.AddTransient<IHostingPackageService, HostingPackageService>();
builder.Services.AddTransient<IResellerCompanyService, ResellerCompanyService>();
builder.Services.AddTransient<ISalesAgentService, SalesAgentService>();
builder.Services.AddTransient<ISentEmailService, SentEmailService>();
builder.Services.AddTransient<IPaymentGatewayService, PaymentGatewayService>();
builder.Services.AddTransient<IEmailQueueService, EmailQueueService>();
builder.Services.AddTransient<IDocumentTemplateService, DocumentTemplateService>();
builder.Services.AddTransient<IReportTemplateService, ReportTemplateService>();
builder.Services.AddTransient<ICurrencyService, CurrencyService>();
builder.Services.AddTransient<IExchangeRateDownloadLogService, ExchangeRateDownloadLogService>();
builder.Services.AddTransient<ISystemService, SystemService>();
builder.Services.AddTransient<IRegisteredDomainService, DomainRegistrationService>();
builder.Services.AddTransient<IDomainManagerService, DomainManagerService>();

// Hosting Management Services
builder.Services.AddScoped<IHostingManagerService, HostingManagerService>();
builder.Services.AddScoped<IHostingSyncService, HostingSyncService>();
builder.Services.AddScoped<IHostingDomainService, HostingDomainService>();
builder.Services.AddScoped<IHostingEmailService, HostingEmailService>();
builder.Services.AddScoped<IHostingDatabaseService, HostingDatabaseService>();
builder.Services.AddScoped<IHostingFtpService, HostingFtpService>();

// Sales and Payment Flow services
builder.Services.AddTransient<ICouponService, CouponService>();
builder.Services.AddTransient<ITaxService, TaxService>();
builder.Services.AddTransient<IQuoteService, QuoteService>();
builder.Services.AddTransient<ICreditService, CreditService>();
builder.Services.AddTransient<IPaymentIntentService, PaymentIntentService>();
builder.Services.AddTransient<IRefundService, RefundService>();
builder.Services.AddTransient<ICustomerPaymentMethodService, CustomerPaymentMethodService>();
builder.Services.AddTransient<ISubscriptionService, SubscriptionService>();
builder.Services.AddTransient<ISubscriptionBillingHistoryService, SubscriptionBillingHistoryService>();

// Financial Tracking services
builder.Services.AddTransient<ICustomerTaxProfileService, CustomerTaxProfileService>();
builder.Services.AddTransient<IVendorCostService, VendorCostService>();
builder.Services.AddTransient<IVendorPayoutService, VendorPayoutService>();
builder.Services.AddTransient<IVendorTaxProfileService, VendorTaxProfileService>();
builder.Services.AddTransient<IRefundLossAuditService, RefundLossAuditService>();

// Payment Processing services
builder.Services.AddTransient<IPaymentProcessingService, PaymentProcessingService>();
builder.Services.AddTransient<IPaymentNotificationService, PaymentNotificationService>();

// Domain Registration Library - Registrar Settings
var registrarSettings = builder.Configuration.GetSection("RegistrarSettings").Get<DomainRegistrationLib.Infrastructure.Settings.RegistrarSettings>()
    ?? new DomainRegistrationLib.Infrastructure.Settings.RegistrarSettings();
builder.Services.AddSingleton(registrarSettings);

// Domain Registration Library - Registrar Factory
builder.Services.AddSingleton<DomainRegistrationLib.Factories.DomainRegistrarFactory>();

// Register Domain Merge Helper
builder.Services.AddScoped<ISPAdmin.Services.Helpers.DomainMergeHelper>();

// Exchange Rate Library - Settings and Factory
var exchangeRateSettings = builder.Configuration.GetSection("ExchangeRate").Get<ExchangeRateLib.Infrastructure.Settings.ExchangeRateSettings>()
    ?? new ExchangeRateLib.Infrastructure.Settings.ExchangeRateSettings();
builder.Services.AddSingleton(exchangeRateSettings);
builder.Services.AddSingleton<ExchangeRateLib.Factories.ExchangeRateFactory>();

// Database Backup Settings
var databaseBackupSettings = builder.Configuration.GetSection("DatabaseBackup").Get<ISPAdmin.Infrastructure.Settings.DatabaseBackupSettings>()
    ?? new ISPAdmin.Infrastructure.Settings.DatabaseBackupSettings();
builder.Services.AddSingleton(databaseBackupSettings);

// Domain Registration Settings
builder.Services.Configure<ISPAdmin.Infrastructure.Settings.DomainRegistrationSettings>(
    builder.Configuration.GetSection("DomainRegistration"));

// TLD Pricing Settings and Services
builder.Services.Configure<ISPAdmin.Infrastructure.Settings.TldPricingSettings>(
    builder.Configuration.GetSection(ISPAdmin.Infrastructure.Settings.TldPricingSettings.SectionName));
builder.Services.AddScoped<ISPAdmin.Services.Helpers.MarginValidator>();
builder.Services.AddScoped<ISPAdmin.Services.Helpers.FuturePricingManager>();
builder.Services.AddTransient<ITldPricingService, TldPricingService>();

// Domain Lifecycle Workflows - Domain Services
builder.Services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();

// Domain Lifecycle Workflows - Event Handlers
builder.Services.AddTransient<IDomainEventHandler<DomainRegisteredEvent>, DomainRegisteredEventHandler>();
builder.Services.AddTransient<IDomainEventHandler<DomainExpiredEvent>, DomainExpiredEventHandler>();
builder.Services.AddTransient<IDomainEventHandler<OrderActivatedEvent>, OrderActivatedEventHandler>();
builder.Services.AddTransient<IDomainEventHandler<InvoicePaidEvent>, InvoicePaidEventHandler>();


// Domain Lifecycle Workflows - Workflow Orchestrators
builder.Services.AddTransient<IDomainRegistrationWorkflow, DomainRegistrationWorkflow>();
builder.Services.AddTransient<IDomainRenewalWorkflow, DomainRenewalWorkflow>();
builder.Services.AddTransient<IOrderProvisioningWorkflow, OrderProvisioningWorkflow>();

// Register Background Services
builder.Services.AddHostedService<EmailQueueBackgroundService>();
builder.Services.AddHostedService<DomainExpirationMonitorService>();
builder.Services.AddHostedService<OutboxProcessorService>();
builder.Services.AddHostedService<ExchangeRateUpdateService>();
builder.Services.AddHostedService<DatabaseBackupService>();

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
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "DR Admin API",
        Version = "v1",
        Description = "ISP Admin API"
    });
    
    // Include XML comments for Swagger documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
    
    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    
    options.AddSecurityRequirement((document) => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });
});

var app = builder.Build();

// Apply database migrations on startup (skip in testing environment)
if (app.Environment.EnvironmentName != "Testing")
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            
            // Check for pending migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            var pendingCount = pendingMigrations.Count();
            
            if (pendingCount > 0)
            {
                log.Information("Applying {Count} pending database migration(s)...", pendingCount);
                await context.Database.MigrateAsync();
                log.Information("Database migrations applied successfully");
            }
            else
            {
                log.Information("Database is up to date. No pending migrations");
            }

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
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Show detailed exceptions in development to help diagnose Swagger generation errors
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "DR Admin API v1");
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
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

