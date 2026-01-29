using ISPAdmin.Data;
using ISPAdmin.Infrastructure;
using ISPAdmin.Infrastructure.Settings;
using ISPAdmin.Services;
using ISPAdmin.BackgroundServices;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Threading.Channels;

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

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Admin only policy
    options.AddPolicy("Admin.Only", policy =>
        policy.RequireRole("Admin"));
    
    // Read policies (view/list operations)
    options.AddPolicy("Customer.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("User.Read", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("Role.Read", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("BillingCycle.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("ContactPerson.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("ControlPanelType.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("Country.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("Coupon.Read", policy =>
        policy.RequireRole("Admin", "Sales"));
    
    options.AddPolicy("Currency.Read", policy =>
        policy.RequireRole("Admin", "Finance"));
    
    options.AddPolicy("CustomerCredit.Read", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("CustomerPaymentMethod.Read", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("CustomerStatus.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("DnsRecord.Read", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("DnsRecord.ReadOwn", policy =>
        policy.RequireRole("Admin", "Support", "Customer"));
    
    // Write policies (create/update operations)
    options.AddPolicy("Customer.Write", policy =>
        policy.RequireRole("Admin", "Sales"));
    
    options.AddPolicy("User.Write", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("Role.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("BillingCycle.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("ContactPerson.Write", policy =>
        policy.RequireRole("Admin", "Sales"));
    
    options.AddPolicy("ControlPanelType.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Country.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Coupon.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Currency.Write", policy =>
        policy.RequireRole("Admin", "Finance"));
    
    options.AddPolicy("CustomerCredit.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("CustomerPaymentMethod.Write", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("CustomerStatus.Write", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("DnsRecord.Write", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("DnsRecord.WriteOwn", policy =>
        policy.RequireRole("Admin", "Support", "Customer"));
    
    // Delete policies
    options.AddPolicy("Customer.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("User.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Role.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("BillingCycle.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("ContactPerson.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("ControlPanelType.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Country.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Coupon.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Currency.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("CustomerCredit.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("CustomerPaymentMethod.Delete", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("CustomerStatus.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("DnsRecord.Delete", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("ServerIpAddress.Read", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("ServerIpAddress.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("ServerIpAddress.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Order.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("Order.Write", policy =>
        policy.RequireRole("Admin", "Sales"));
    
    options.AddPolicy("Order.Delete", policy =>
        policy.RequireRole("Admin"));
    
    // DNS-related policies
    options.AddPolicy("DnsRecord.Read", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("DnsRecord.ReadOwn", policy =>
        policy.RequireRole("Admin", "Support", "Customer"));
    
    options.AddPolicy("DnsRecord.Write", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("DnsRecord.WriteOwn", policy =>
        policy.RequireRole("Admin", "Support", "Customer"));
    
    options.AddPolicy("DnsRecord.Delete", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("DnsRecordType.Read", policy =>
        policy.RequireRole("Admin", "Support", "Customer"));
    
    options.AddPolicy("DnsRecordType.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("DnsRecordType.Delete", policy =>
        policy.RequireRole("Admin"));
    
    // Server-related policies
    options.AddPolicy("Server.Read", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("Server.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Server.Delete", policy =>
        policy.RequireRole("Admin"));
    
    // Service-related policies
    options.AddPolicy("Service.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("Service.Write", policy =>
        policy.RequireRole("Admin", "Sales"));
    
    options.AddPolicy("Service.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("ServiceType.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("ServiceType.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("ServiceType.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Invoice.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("Invoice.Write", policy =>
        policy.RequireRole("Admin", "Sales"));
    
    options.AddPolicy("Invoice.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Tld.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales", "Customer"));
    
    options.AddPolicy("Tld.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Tld.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Registrar.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales", "Customer"));
    
    options.AddPolicy("Registrar.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Registrar.Delete", policy =>
        policy.RequireRole("Admin"));
    
    // Additional entity policies
    options.AddPolicy("HostingPackage.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("HostingPackage.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("HostingPackage.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("PostalCode.Read", policy =>
        policy.RequireRole("Admin", "Support", "Customer"));
    
    options.AddPolicy("PostalCode.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("PostalCode.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("PaymentGateway.Read", policy =>
        policy.RequireRole("Admin", "Support", "Customer"));
    
    options.AddPolicy("PaymentGateway.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("PaymentGateway.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("SalesAgent.Read", policy =>
        policy.RequireRole("Admin", "Sales"));
    
    options.AddPolicy("SalesAgent.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("SalesAgent.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("ResellerCompany.Read", policy =>
        policy.RequireRole("Admin", "Sales"));
    
    options.AddPolicy("ResellerCompany.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("ResellerCompany.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("DnsZonePackage.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("DnsZonePackage.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("DnsZonePackage.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("DocumentTemplate.Read", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("DocumentTemplate.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("DocumentTemplate.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("SentEmail.Read", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("Unit.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("Unit.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Unit.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("InvoiceLine.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("InvoiceLine.Write", policy =>
        policy.RequireRole("Admin", "Sales"));
    
    options.AddPolicy("InvoiceLine.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("ServerControlPanel.Read", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("ServerControlPanel.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("ServerControlPanel.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("DnsZonePackageRecord.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("DnsZonePackageRecord.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("DnsZonePackageRecord.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("RegistrarTld.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales", "Customer"));
    
    options.AddPolicy("RegistrarTld.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("RegistrarTld.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("EmailQueue.Write", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("EmailQueue.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("Quote.Read", policy =>
        policy.RequireRole("Admin", "Sales", "Support"));
    
    options.AddPolicy("Quote.Write", policy =>
        policy.RequireRole("Admin", "Sales"));
    
    options.AddPolicy("Quote.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Refund.Read", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("Refund.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Refund.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Subscription.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("Subscription.Write", policy =>
        policy.RequireRole("Admin", "Sales"));
    
    options.AddPolicy("Subscription.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("SubscriptionBillingHistory.Read", policy =>
        policy.RequireRole("Admin", "Support", "Sales"));
    
    options.AddPolicy("TaxRule.Read", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("TaxRule.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("TaxRule.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Token.Read", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Token.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("Token.Delete", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("ExchangeRateDownloadLog.Read", policy =>
        policy.RequireRole("Admin", "Finance"));
    
    options.AddPolicy("PaymentIntent.Read", policy =>
        policy.RequireRole("Admin", "Support"));
    
    options.AddPolicy("PaymentIntent.Write", policy =>
        policy.RequireRole("Admin", "Support", "Customer"));
    
    options.AddPolicy("PaymentIntent.Delete", policy =>
        policy.RequireRole("Admin"));
});

// Add services to the container.
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IInitializationService, InitializationService>();
builder.Services.AddTransient<IMyAccountService, MyAccountService>();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<ICustomerStatusService, CustomerStatusService>();
builder.Services.AddTransient<IContactPersonService, ContactPersonService>();
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
builder.Services.AddTransient<IDnsRecordTypeService, DnsRecordTypeService>();
builder.Services.AddTransient<IDnsRecordService, DnsRecordService>();
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
builder.Services.AddTransient<ICurrencyService, CurrencyService>();
builder.Services.AddTransient<IExchangeRateDownloadLogService, ExchangeRateDownloadLogService>();
builder.Services.AddTransient<ISystemService, SystemService>();

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

// Domain Registration Library - Registrar Settings
var registrarSettings = builder.Configuration.GetSection("RegistrarSettings").Get<DomainRegistrationLib.Infrastructure.Settings.RegistrarSettings>()
    ?? new DomainRegistrationLib.Infrastructure.Settings.RegistrarSettings();
builder.Services.AddSingleton(registrarSettings);

// Domain Registration Library - Registrar Factory
builder.Services.AddSingleton<DomainRegistrationLib.Factories.DomainRegistrarFactory>();

// Exchange Rate Library - Settings and Factory
var exchangeRateSettings = builder.Configuration.GetSection("ExchangeRate").Get<ExchangeRateLib.Infrastructure.Settings.ExchangeRateSettings>()
    ?? new ExchangeRateLib.Infrastructure.Settings.ExchangeRateSettings();
builder.Services.AddSingleton(exchangeRateSettings);
builder.Services.AddSingleton<ExchangeRateLib.Factories.ExchangeRateFactory>();

// Database Backup Settings
var databaseBackupSettings = builder.Configuration.GetSection("DatabaseBackup").Get<ISPAdmin.Infrastructure.Settings.DatabaseBackupSettings>()
    ?? new ISPAdmin.Infrastructure.Settings.DatabaseBackupSettings();
builder.Services.AddSingleton(databaseBackupSettings);

// Domain Lifecycle Workflows - Domain Services
builder.Services.AddTransient<ISPAdmin.Domain.Services.IDomainEventPublisher, ISPAdmin.Domain.Services.DomainEventPublisher>();

// Domain Lifecycle Workflows - Event Handlers
builder.Services.AddTransient<ISPAdmin.Domain.Services.IDomainEventHandler<ISPAdmin.Domain.Events.DomainEvents.DomainRegisteredEvent>, ISPAdmin.Domain.EventHandlers.DomainRegisteredEventHandler>();
builder.Services.AddTransient<ISPAdmin.Domain.Services.IDomainEventHandler<ISPAdmin.Domain.Events.DomainEvents.DomainExpiredEvent>, ISPAdmin.Domain.EventHandlers.DomainExpiredEventHandler>();
builder.Services.AddTransient<ISPAdmin.Domain.Services.IDomainEventHandler<ISPAdmin.Domain.Events.OrderEvents.OrderActivatedEvent>, ISPAdmin.Domain.EventHandlers.OrderActivatedEventHandler>();
builder.Services.AddTransient<ISPAdmin.Domain.Services.IDomainEventHandler<ISPAdmin.Domain.Events.InvoiceEvents.InvoicePaidEvent>, ISPAdmin.Domain.EventHandlers.InvoicePaidEventHandler>();


// Domain Lifecycle Workflows - Workflow Orchestrators
builder.Services.AddTransient<ISPAdmin.Domain.Workflows.IDomainRegistrationWorkflow, ISPAdmin.Domain.Workflows.DomainRegistrationWorkflow>();
builder.Services.AddTransient<ISPAdmin.Domain.Workflows.IDomainRenewalWorkflow, ISPAdmin.Domain.Workflows.DomainRenewalWorkflow>();
builder.Services.AddTransient<ISPAdmin.Domain.Workflows.IOrderProvisioningWorkflow, ISPAdmin.Domain.Workflows.OrderProvisioningWorkflow>();

// Register Background Services
builder.Services.AddHostedService<EmailQueueBackgroundService>();
builder.Services.AddHostedService<ISPAdmin.BackgroundServices.DomainExpirationMonitorService>();
builder.Services.AddHostedService<ISPAdmin.BackgroundServices.OutboxProcessorService>();
builder.Services.AddHostedService<ISPAdmin.BackgroundServices.ExchangeRateUpdateService>();
builder.Services.AddHostedService<ISPAdmin.BackgroundServices.DatabaseBackupService>();

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
    
    // Include XML comments for Swagger documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
    
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

