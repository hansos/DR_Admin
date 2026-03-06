using System.Diagnostics;
using EmailSenderLib.Factories;
using EmailSenderLib.Infrastructure.Settings;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
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
    private readonly EmailSenderFactory _emailSenderFactory;
    private readonly EmailSettings _emailSettings;
    private static readonly Serilog.ILogger _log = Log.ForContext<SystemService>();

    public SystemService(
        ApplicationDbContext context,
        AppSettings appSettings,
        EmailSenderFactory emailSenderFactory,
        EmailSettings emailSettings)
    {
        _context = context;
        _appSettings = appSettings;
        _emailSenderFactory = emailSenderFactory;
        _emailSettings = emailSettings;
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
        }

        await _context.SaveChangesAsync();
        return customers.Count;
    }

    private async Task<int> NormalizeDomainsAsync()
    {
        var domains = await _context.RegisteredDomains.ToListAsync();
        
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
    /// Sends test emails with both plain text and HTML bodies.
    /// </summary>
    /// <param name="request">Test email request containing sender and receiver addresses.</param>
    /// <returns>Detailed test email execution report.</returns>
    public async Task<TestEmailResultDto> SendTestEmailAsync(TestEmailRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.SenderEmail))
        {
            throw new ArgumentException("SenderEmail is required", nameof(request.SenderEmail));
        }

        if (string.IsNullOrWhiteSpace(request.ReceiverEmail))
        {
            throw new ArgumentException("ReceiverEmail is required", nameof(request.ReceiverEmail));
        }

        var startedAt = DateTime.UtcNow;
        var result = new TestEmailResultDto
        {
            StartedAtUtc = startedAt,
            RequestedSenderEmail = request.SenderEmail.Trim(),
            ReceiverEmail = request.ReceiverEmail.Trim(),
            PreferredPluginKey = _emailSettings.Selection.DefaultPluginKey
                ?? _emailSettings.Provider
                ?? string.Empty
        };

        var emailSender = _emailSenderFactory.CreateEmailSender();
        result.SenderImplementation = emailSender.GetType().Name;

        var traceId = Guid.NewGuid().ToString("N");
        var baseSubject = $"DR Admin Test Email [{traceId}]";

        var textBody =
$"DR Admin test email (TEXT).\n" +
$"Requested sender: {result.RequestedSenderEmail}\n" +
$"Receiver: {result.ReceiverEmail}\n" +
$"Timestamp (UTC): {DateTime.UtcNow:O}\n";

        var htmlBody =
$"<html><body>" +
$"<h3>DR Admin test email (HTML)</h3>" +
$"<p><strong>Requested sender:</strong> {System.Net.WebUtility.HtmlEncode(result.RequestedSenderEmail)}</p>" +
$"<p><strong>Receiver:</strong> {System.Net.WebUtility.HtmlEncode(result.ReceiverEmail)}</p>" +
$"<p><strong>Timestamp (UTC):</strong> {DateTime.UtcNow:O}</p>" +
$"</body></html>";

        try
        {
            await emailSender.SendEmailAsync(result.ReceiverEmail, $"{baseSubject} [TEXT]", textBody, false);
            result.TextEmailSent = true;
        }
        catch (Exception ex)
        {
            result.TextEmailSent = false;
            result.TextEmailError = ex.ToString();
            _log.Error(ex, "Error sending TEXT test email to {Receiver}", result.ReceiverEmail);
        }

        try
        {
            await emailSender.SendEmailAsync(result.ReceiverEmail, $"{baseSubject} [HTML]", htmlBody, true);
            result.HtmlEmailSent = true;
        }
        catch (Exception ex)
        {
            result.HtmlEmailSent = false;
            result.HtmlEmailError = ex.ToString();
            _log.Error(ex, "Error sending HTML test email to {Receiver}", result.ReceiverEmail);
        }

        result.Success = result.TextEmailSent && result.HtmlEmailSent;
        result.CompletedAtUtc = DateTime.UtcNow;
        result.Note = "Requested sender is included in test content. Actual sender address is controlled by selected email provider configuration.";

        return result;
    }

    /// <summary>
    /// Seeds core test data into selected tables when those tables are empty.
    /// </summary>
    /// <returns>Summary of seeded records grouped by table.</returns>
    public async Task<SeedTestDataResultDto> SeedTestDataAsync()
    {
        var result = new SeedTestDataResultDto();

        try
        {
            _log.Information("Starting test data seeding for core catalog tables");

            if (!await _context.Tlds.AnyAsync())
            {
                _context.Tlds.AddRange(
                    new Tld { Extension = "com", Description = "Commercial", IsActive = true, DefaultRegistrationYears = 1, MaxRegistrationYears = 10 },
                    new Tld { Extension = "net", Description = "Network", IsActive = true, DefaultRegistrationYears = 1, MaxRegistrationYears = 10 },
                    new Tld { Extension = "org", Description = "Organization", IsActive = true, DefaultRegistrationYears = 1, MaxRegistrationYears = 10 });
                result.InsertedByTable["Tlds"] = 3;
            }

            if (!await _context.ServerTypes.AnyAsync())
            {
                _context.ServerTypes.AddRange(
                    new ServerType { Name = "shared", DisplayName = "Shared", Description = "Shared hosting server", IsActive = true },
                    new ServerType { Name = "vps", DisplayName = "VPS", Description = "Virtual private server", IsActive = true },
                    new ServerType { Name = "dedicated", DisplayName = "Dedicated", Description = "Dedicated physical server", IsActive = true });
                result.InsertedByTable["ServerTypes"] = 3;
            }

            if (!await _context.OperatingSystems.AnyAsync())
            {
                _context.OperatingSystems.AddRange(
                    new ISPAdmin.Data.Entities.OperatingSystem { Name = "ubuntu-22-04", DisplayName = "Ubuntu 22.04 LTS", Version = "22.04", Description = "Ubuntu Linux", IsActive = true },
                    new ISPAdmin.Data.Entities.OperatingSystem { Name = "debian-12", DisplayName = "Debian 12", Version = "12", Description = "Debian Linux", IsActive = true },
                    new ISPAdmin.Data.Entities.OperatingSystem { Name = "windows-server-2022", DisplayName = "Windows Server 2022", Version = "2022", Description = "Microsoft Windows Server", IsActive = true });
                result.InsertedByTable["OperatingSystems"] = 3;
            }

            if (!await _context.HostProviders.AnyAsync())
            {
                _context.HostProviders.Add(
                    new HostProvider
                    {
                        Name = "namecheap",
                        DisplayName = "NameCheap",
                        Description = "NameCheap infrastructure provider",
                        WebsiteUrl = "https://www.namecheap.com",
                        IsActive = true
                    });
                result.InsertedByTable["HostProviders"] = 1;
            }

            if (!await _context.HostingPackages.AnyAsync())
            {
                _context.HostingPackages.AddRange(
                    new HostingPackage
                    {
                        Name = "Starter Hosting",
                        Description = "Entry level hosting package",
                        DiskSpaceMB = 10240,
                        BandwidthMB = 102400,
                        EmailAccounts = 10,
                        Databases = 5,
                        Domains = 1,
                        Subdomains = 5,
                        FtpAccounts = 5,
                        SslSupport = true,
                        BackupSupport = false,
                        DedicatedIp = false,
                        MonthlyPrice = 4.99m,
                        YearlyPrice = 49.99m,
                        IsActive = true
                    },
                    new HostingPackage
                    {
                        Name = "Business Hosting",
                        Description = "Business hosting package",
                        DiskSpaceMB = 51200,
                        BandwidthMB = 512000,
                        EmailAccounts = 50,
                        Databases = 20,
                        Domains = 10,
                        Subdomains = 25,
                        FtpAccounts = 20,
                        SslSupport = true,
                        BackupSupport = true,
                        DedicatedIp = false,
                        MonthlyPrice = 12.99m,
                        YearlyPrice = 129.99m,
                        IsActive = true
                    },
                    new HostingPackage
                    {
                        Name = "Pro Hosting",
                        Description = "Advanced hosting package",
                        DiskSpaceMB = 102400,
                        BandwidthMB = 1048576,
                        EmailAccounts = 200,
                        Databases = 100,
                        Domains = 50,
                        Subdomains = 100,
                        FtpAccounts = 100,
                        SslSupport = true,
                        BackupSupport = true,
                        DedicatedIp = true,
                        MonthlyPrice = 24.99m,
                        YearlyPrice = 249.99m,
                        IsActive = true
                    });
                result.InsertedByTable["HostingPackages"] = 3;
            }

            if (!await _context.ControlPanelTypes.AnyAsync())
            {
                _context.ControlPanelTypes.AddRange(
                    new ControlPanelType { Name = "cpanel", DisplayName = "cPanel", Description = "cPanel hosting control panel", IsActive = true },
                    new ControlPanelType { Name = "plesk", DisplayName = "Plesk", Description = "Plesk hosting control panel", IsActive = true },
                    new ControlPanelType { Name = "directadmin", DisplayName = "DirectAdmin", Description = "DirectAdmin hosting control panel", IsActive = true });
                result.InsertedByTable["ControlPanelTypes"] = 3;
            }

            if (!await _context.DnsZonePackages.AnyAsync())
            {
                _context.DnsZonePackages.AddRange(
                    new DnsZonePackage
                    {
                        Name = "Basic DNS Template",
                        Description = "Standard DNS records template",
                        IsActive = true,
                        IsDefault = true,
                        SortOrder = 1
                    },
                    new DnsZonePackage
                    {
                        Name = "Business DNS Template",
                        Description = "DNS template for business hosting",
                        IsActive = true,
                        IsDefault = false,
                        SortOrder = 2
                    });
                result.InsertedByTable["DnsZonePackages"] = 2;
            }

            ServiceType additionalServiceType;
            if (!await _context.ServiceTypes.AnyAsync())
            {
                additionalServiceType = new ServiceType
                {
                    Name = "Additional Service",
                    Description = "Service type for add-on products"
                };

                _context.ServiceTypes.AddRange(
                    new ServiceType { Name = "Hosting", Description = "Hosting services" },
                    new ServiceType { Name = "Domain", Description = "Domain services" },
                    additionalServiceType);

                result.InsertedByTable["ServiceTypes"] = 3;
            }
            else
            {
                additionalServiceType = await _context.ServiceTypes
                    .OrderBy(x => x.Id)
                    .FirstAsync();
            }

            BillingCycle monthlyBillingCycle;
            if (!await _context.BillingCycles.AnyAsync())
            {
                monthlyBillingCycle = new BillingCycle
                {
                    Code = "MONTHLY",
                    Name = "Monthly",
                    DurationInDays = 30,
                    Description = "Monthly recurring billing",
                    SortOrder = 1
                };

                _context.BillingCycles.AddRange(
                    monthlyBillingCycle,
                    new BillingCycle
                    {
                        Code = "YEARLY",
                        Name = "Yearly",
                        DurationInDays = 365,
                        Description = "Yearly recurring billing",
                        SortOrder = 2
                    });

                result.InsertedByTable["BillingCycles"] = 2;
            }
            else
            {
                monthlyBillingCycle = await _context.BillingCycles
                    .OrderBy(x => x.Id)
                    .FirstAsync();
            }

            if (!await _context.Services.AnyAsync())
            {
                _context.Services.AddRange(
                    new Service
                    {
                        Name = "SSL Certificate",
                        Description = "Domain validated SSL certificate",
                        ServiceType = additionalServiceType,
                        BillingCycle = monthlyBillingCycle,
                        Price = 3.99m,
                        SetupFee = 0m,
                        IsActive = true,
                        IsFeatured = true,
                        Sku = "ADDON-SSL",
                        MinQuantity = 1,
                        SortOrder = 1,
                        SpecificationsJson = "{}"
                    },
                    new Service
                    {
                        Name = "Daily Backups",
                        Description = "Automated daily backup add-on",
                        ServiceType = additionalServiceType,
                        BillingCycle = monthlyBillingCycle,
                        Price = 2.99m,
                        SetupFee = 0m,
                        IsActive = true,
                        IsFeatured = false,
                        Sku = "ADDON-BACKUP",
                        MinQuantity = 1,
                        SortOrder = 2,
                        SpecificationsJson = "{}"
                    },
                    new Service
                    {
                        Name = "Priority Support",
                        Description = "Priority technical support add-on",
                        ServiceType = additionalServiceType,
                        BillingCycle = monthlyBillingCycle,
                        Price = 5.99m,
                        SetupFee = 0m,
                        IsActive = true,
                        IsFeatured = false,
                        Sku = "ADDON-SUPPORT",
                        MinQuantity = 1,
                        SortOrder = 3,
                        SpecificationsJson = "{}"
                    });
                result.InsertedByTable["Services"] = 3;
            }

            if (!await _context.Registrars.AnyAsync())
            {
                _context.Registrars.AddRange(
                    new Registrar
                    {
                        Name = "AWS Route 53",
                        Code = "aws",
                        IsActive = true,
                        IsDefault=true,
                        ContactEmail = "support@amazon.com",
                        Website = "https://aws.amazon.com/route53/",
                        Notes = "Registry API provider via AWS"
                    },
                    new Registrar
                    {
                        Name = "Namecheap API",
                        Code = "namecheap",
                        IsActive = true,
                        ContactEmail = "support@namecheap.com",
                        Website = "https://www.namecheap.com/support/api/",
                        Notes = "Namecheap domain registry API"
                    },
                    new Registrar
                    {
                        Name = "Cloudflare Registrar API",
                        Code = "cloudflare",
                        IsActive = true,
                        ContactEmail = "support@cloudflare.com",
                        Website = "https://www.cloudflare.com",
                        Notes = "Cloudflare registrar integration"
                    });
                result.InsertedByTable["Registrars"] = 3;
            }

            if (!await _context.PaymentGateways.AnyAsync())
            {
                _context.PaymentGateways.AddRange(
                    new PaymentGateway
                    {
                        Name = "Stripe",
                        ProviderCode = "stripe",
                        PaymentInstrument = "CreditCard",
                        IsActive = true,
                        IsDefault = true,
                        ApiKey = "test_stripe_api_key",
                        ApiSecret = "test_stripe_api_secret",
                        ConfigurationJson = "{}",
                        UseSandbox = true,
                        Description = "Stripe payment provider",
                        SupportedCurrencies = "USD,EUR,GBP"
                    },
                    new PaymentGateway
                    {
                        Name = "PayPal",
                        ProviderCode = "paypal",
                        PaymentInstrument = "PayPal",
                        IsActive = true,
                        IsDefault = false,
                        ApiKey = "test_paypal_api_key",
                        ApiSecret = "test_paypal_api_secret",
                        ConfigurationJson = "{}",
                        UseSandbox = true,
                        Description = "PayPal payment provider",
                        SupportedCurrencies = "USD,EUR,GBP"
                    });
                result.InsertedByTable["PaymentGateways"] = 2;
            }

            await _context.SaveChangesAsync();

            result.Success = true;
            result.Message = result.InsertedByTable.Count == 0
                ? "No data inserted. Target tables already contained records."
                : "Test data inserted successfully.";

            _log.Information("Completed test data seeding. Inserted tables: {TableCount}", result.InsertedByTable.Count);
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while seeding test data");
            result.Success = false;
            result.Message = ex.Message;
            return result;
        }
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
