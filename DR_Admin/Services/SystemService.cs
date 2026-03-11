using System.Diagnostics;
using EmailSenderLib.Factories;
using EmailSenderLib.Infrastructure.Settings;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Infrastructure;
using ISPAdmin.Utilities;
using ISPAdmin.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Serilog;
using System.Text.Json;

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
    private readonly IHostEnvironment _hostEnvironment;
    private static readonly Serilog.ILogger _log = Log.ForContext<SystemService>();

    public SystemService(
        ApplicationDbContext context,
        AppSettings appSettings,
        EmailSenderFactory emailSenderFactory,
        EmailSettings emailSettings,
        IHostEnvironment hostEnvironment)
    {
        _context = context;
        _appSettings = appSettings;
        _emailSenderFactory = emailSenderFactory;
        _emailSettings = emailSettings;
        _hostEnvironment = hostEnvironment;
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
        var seededTlds = new List<Tld>();
        var seededServers = new List<Server>();

        try
        {
            _log.Information("Starting test data seeding for core catalog tables");

            if (!await _context.Countries.AnyAsync())
            {
                _context.Countries.AddRange(
                    new Country
                    {
                        Code = "NO",
                        Tld = "no",
                        Iso3 = "NOR",
                        Numeric = 578,
                        EnglishName = "Norway",
                        LocalName = "Norge",
                        IsActive = true,
                        NormalizedEnglishName = "norway",
                        NormalizedLocalName = "norge"
                    },
                    new Country
                    {
                        Code = "DK",
                        Tld = "dk",
                        Iso3 = "DNK",
                        Numeric = 208,
                        EnglishName = "Denmark",
                        LocalName = "Danmark",
                        IsActive = true,
                        NormalizedEnglishName = "denmark",
                        NormalizedLocalName = "danmark"
                    },
                    new Country
                    {
                        Code = "GB",
                        Tld = "uk",
                        Iso3 = "GBR",
                        Numeric = 826,
                        EnglishName = "United Kingdom",
                        LocalName = "United Kingdom",
                        IsActive = true,
                        NormalizedEnglishName = "united kingdom",
                        NormalizedLocalName = "united kingdom"
                    },
                    new Country
                    {
                        Code = "US",
                        Tld = "us",
                        Iso3 = "USA",
                        Numeric = 840,
                        EnglishName = "United States",
                        LocalName = "United States",
                        IsActive = true,
                        NormalizedEnglishName = "united states",
                        NormalizedLocalName = "united states"
                    });

                result.InsertedByTable["Countries"] = 4;
            }

            if (!await _context.CustomerStatuses.AnyAsync())
            {
                _context.CustomerStatuses.AddRange(
                    new CustomerStatus
                    {
                        Code = "ACTIVE",
                        Name = "Active",
                        Description = "Customer account is active and can place orders",
                        IsActive = true,
                        Color = "#198754",
                        Priority = 1,
                        IsSystem = true
                    },
                    new CustomerStatus
                    {
                        Code = "PENDING",
                        Name = "Pending",
                        Description = "Customer account is pending review or setup",
                        IsActive = true,
                        Color = "#ffc107",
                        Priority = 2,
                        IsSystem = true
                    },
                    new CustomerStatus
                    {
                        Code = "SUSPENDED",
                        Name = "Suspended",
                        Description = "Customer account is temporarily suspended",
                        IsActive = true,
                        Color = "#fd7e14",
                        Priority = 3,
                        IsSystem = true
                    },
                    new CustomerStatus
                    {
                        Code = "INACTIVE",
                        Name = "Inactive",
                        Description = "Customer account is inactive",
                        IsActive = true,
                        Color = "#6c757d",
                        Priority = 4,
                        IsSystem = true
                    });

                result.InsertedByTable["CustomerStatuses"] = 4;
            }

            if (!await _context.Currencies.AnyAsync())
            {
                _context.Currencies.AddRange(
                    new Currency
                    {
                        Code = "NOK",
                        Name = "Norwegian Krone",
                        Symbol = "kr",
                        IsActive = true,
                        IsDefault = false,
                        IsCustomerCurrency = true,
                        IsProviderCurrency = true,
                        SortOrder = 1
                    },
                    new Currency
                    {
                        Code = "EUR",
                        Name = "Euro",
                        Symbol = "€",
                        IsActive = true,
                        IsDefault = true,
                        IsCustomerCurrency = true,
                        IsProviderCurrency = true,
                        SortOrder = 2
                    },
                    new Currency
                    {
                        Code = "GBP",
                        Name = "British Pound",
                        Symbol = "£",
                        IsActive = true,
                        IsDefault = false,
                        IsCustomerCurrency = true,
                        IsProviderCurrency = true,
                        SortOrder = 3
                    },
                    new Currency
                    {
                        Code = "USD",
                        Name = "US Dollar",
                        Symbol = "$",
                        IsActive = true,
                        IsDefault = false,
                        IsCustomerCurrency = true,
                        IsProviderCurrency = true,
                        SortOrder = 4
                    });

                result.InsertedByTable["Currencies"] = 4;
            }

            if (!await _context.Tlds.AnyAsync())
            {
                seededTlds.AddRange(
                    new Tld { Extension = "com", Description = "Commercial", IsActive = true, DefaultRegistrationYears = 1, MaxRegistrationYears = 10 },
                    new Tld { Extension = "net", Description = "Network", IsActive = true, DefaultRegistrationYears = 1, MaxRegistrationYears = 10 },
                    new Tld { Extension = "org", Description = "Organization", IsActive = true, DefaultRegistrationYears = 1, MaxRegistrationYears = 10 });

                _context.Tlds.AddRange(seededTlds);
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

            if (!await _context.Servers.AnyAsync())
            {
                var serverTypes = _context.ServerTypes.Local.ToList();
                if (serverTypes.Count == 0)
                {
                    serverTypes = await _context.ServerTypes
                        .OrderBy(x => x.Id)
                        .ToListAsync();
                }

                var operatingSystems = _context.OperatingSystems.Local.ToList();
                if (operatingSystems.Count == 0)
                {
                    operatingSystems = await _context.OperatingSystems
                        .OrderBy(x => x.Id)
                        .ToListAsync();
                }

                var hostProvider = _context.HostProviders.Local.FirstOrDefault()
                    ?? await _context.HostProviders
                        .OrderBy(x => x.Id)
                        .FirstOrDefaultAsync();

                if (serverTypes.Count > 0 && operatingSystems.Count > 0)
                {
                    var primaryServerType = serverTypes[0];
                    var secondaryServerType = serverTypes.Count > 1 ? serverTypes[1] : serverTypes[0];
                    var primaryOperatingSystem = operatingSystems[0];
                    var secondaryOperatingSystem = operatingSystems.Count > 1 ? operatingSystems[1] : operatingSystems[0];

                    seededServers.AddRange(
                        new Server
                        {
                            Name = "web-01",
                            ServerType = primaryServerType,
                            HostProvider = hostProvider,
                            Location = "US-East",
                            OperatingSystem = primaryOperatingSystem,
                            Status = true,
                            CpuCores = 4,
                            RamMB = 8192,
                            DiskSpaceGB = 120,
                            Notes = "Seeded test web server"
                        },
                        new Server
                        {
                            Name = "app-01",
                            ServerType = secondaryServerType,
                            HostProvider = hostProvider,
                            Location = "EU-West",
                            OperatingSystem = secondaryOperatingSystem,
                            Status = true,
                            CpuCores = 8,
                            RamMB = 16384,
                            DiskSpaceGB = 240,
                            Notes = "Seeded test application server"
                        });

                    _context.Servers.AddRange(seededServers);
                    result.InsertedByTable["Servers"] = seededServers.Count;
                }
                else
                {
                    _log.Warning("Skipped server seeding because required ServerTypes or OperatingSystems are unavailable");
                }
            }

            if (!await _context.ServerIpAddresses.AnyAsync())
            {
                var serversForIpSeed = seededServers.Count > 0
                    ? seededServers
                    : await _context.Servers
                        .OrderBy(x => x.Id)
                        .Take(2)
                        .ToListAsync();

                if (serversForIpSeed.Count > 0)
                {
                    var seededIps = new List<ServerIpAddress>
                    {
                        new ServerIpAddress
                        {
                            Server = serversForIpSeed[0],
                            IpAddress = "192.0.2.10",
                            IpVersion = "IPv4",
                            IsPrimary = true,
                            Status = "Active",
                            Notes = "Primary IP for seeded server"
                        },
                        new ServerIpAddress
                        {
                            Server = serversForIpSeed[0],
                            IpAddress = "192.0.2.11",
                            IpVersion = "IPv4",
                            IsPrimary = false,
                            Status = "Reserved",
                            Notes = "Secondary IP for seeded server"
                        }
                    };

                    if (serversForIpSeed.Count > 1)
                    {
                        seededIps.Add(
                            new ServerIpAddress
                            {
                                Server = serversForIpSeed[1],
                                IpAddress = "2001:db8::10",
                                IpVersion = "IPv6",
                                IsPrimary = true,
                                Status = "Active",
                                Notes = "Primary IPv6 for seeded server"
                            });
                    }

                    _context.ServerIpAddresses.AddRange(seededIps);
                    result.InsertedByTable["ServerIpAddresses"] = seededIps.Count;
                }
                else
                {
                    _log.Warning("Skipped server IP address seeding because no servers are available");
                }
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

            if (seededTlds.Count > 0)
            {
                var awsRegistrar = _context.Registrars.Local
                    .FirstOrDefault(x => x.Code == "aws")
                    ?? await _context.Registrars.FirstOrDefaultAsync(x => x.Code == "aws");

                if (awsRegistrar == null)
                {
                    awsRegistrar = new Registrar
                    {
                        Name = "AWS Route 53",
                        Code = "aws",
                        IsActive = true,
                        IsDefault = true,
                        ContactEmail = "support@amazon.com",
                        Website = "https://aws.amazon.com/route53/",
                        Notes = "Registry API provider via AWS"
                    };

                    _context.Registrars.Add(awsRegistrar);
                    result.InsertedByTable["Registrars"] = result.InsertedByTable.GetValueOrDefault("Registrars") + 1;
                }

                var seededAtUtc = DateTime.UtcNow;

                _context.TldSalesPricing.AddRange(
                    seededTlds.Select(tld => new TldSalesPricing
                    {
                        Tld = tld,
                        EffectiveFrom = seededAtUtc,
                        RegistrationPrice = 12.99m,
                        RenewalPrice = 14.99m,
                        TransferPrice = 11.99m,
                        PrivacyPrice = 2.99m,
                        Currency = "USD",
                        IsPromotional = false,
                        IsActive = true,
                        Notes = "Seeded default sales pricing",
                        CreatedBy = "SystemService.SeedTestDataAsync"
                    }));
                result.InsertedByTable["TldSalesPricing"] = seededTlds.Count;

                _context.RegistrarTlds.AddRange(
                    seededTlds.Select(tld => new RegistrarTld
                    {
                        Registrar = awsRegistrar,
                        Tld = tld,
                        IsActive = true,
                        AutoRenew = true,
                        MinRegistrationYears = tld.DefaultRegistrationYears ?? 1,
                        MaxRegistrationYears = tld.MaxRegistrationYears,
                        Notes = "Seeded AWS registrar mapping"
                    }));
                result.InsertedByTable["RegistrarTlds"] = seededTlds.Count;
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

            if (!await _context.PaymentInstruments.AnyAsync())
            {
                var stripeGateway = _context.PaymentGateways.Local
                    .FirstOrDefault(x => x.ProviderCode == "stripe")
                    ?? await _context.PaymentGateways
                        .FirstOrDefaultAsync(x => x.ProviderCode == "stripe");

                _context.PaymentInstruments.AddRange(
                    new PaymentInstrument
                    {
                        Code = "CreditCard",
                        Name = "Credit Card",
                        NormalizedCode = "creditcard",
                        NormalizedName = "credit card",
                        Description = "Card payments",
                        IsActive = true,
                        DisplayOrder = 1,
                        DefaultGateway = stripeGateway
                    },
                    new PaymentInstrument
                    {
                        Code = "PayPal",
                        Name = "PayPal",
                        NormalizedCode = "paypal",
                        NormalizedName = "paypal",
                        Description = "PayPal payments",
                        IsActive = true,
                        DisplayOrder = 2
                    },
                    new PaymentInstrument
                    {
                        Code = "Cash",
                        Name = "Cash",
                        NormalizedCode = "cash",
                        NormalizedName = "cash",
                        Description = "Cash payments",
                        IsActive = true,
                        DisplayOrder = 3
                    });

                result.InsertedByTable["PaymentInstruments"] = 3;
            }

            var taxSeedBaseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var taxSeedCountries = new[] { "NO", "DK", "GB", "US" };

            var taxJurisdictionSeeds = new[]
            {
                new { Code = "NO", JurisdictionCode = "NO-NATIONAL", Name = "Norway VAT", CountryCode = "NO", StateCode = (string?)null, Authority = "Skatteetaten", TaxCurrencyCode = "NOK", RegistrationNumber = "NO123456789MVA", LegalEntity = "DR Admin Norway AS", TaxName = "MVA", TaxRate = 0.25m, TaxCategoryCode = "STANDARD", TaxCategoryName = "Standard VAT", TaxCategoryDescription = "Norway standard VAT", ReverseCharge = true, Priority = 100 },
                new { Code = "DK", JurisdictionCode = "DK-NATIONAL", Name = "Denmark VAT", CountryCode = "DK", StateCode = (string?)null, Authority = "Skattestyrelsen", TaxCurrencyCode = "DKK", RegistrationNumber = "DK12345678", LegalEntity = "DR Admin Denmark ApS", TaxName = "MOMS", TaxRate = 0.25m, TaxCategoryCode = "STANDARD", TaxCategoryName = "Standard VAT", TaxCategoryDescription = "Denmark standard VAT", ReverseCharge = true, Priority = 100 },
                new { Code = "GB", JurisdictionCode = "GB-NATIONAL", Name = "United Kingdom VAT", CountryCode = "GB", StateCode = (string?)null, Authority = "HM Revenue & Customs", TaxCurrencyCode = "GBP", RegistrationNumber = "GB123456789", LegalEntity = "DR Admin UK Ltd", TaxName = "VAT", TaxRate = 0.20m, TaxCategoryCode = "STANDARD", TaxCategoryName = "Standard VAT", TaxCategoryDescription = "United Kingdom standard VAT", ReverseCharge = true, Priority = 100 },
                new { Code = "US", JurisdictionCode = "US-NY", Name = "United States Sales Tax (NY)", CountryCode = "US", StateCode = "NY", Authority = "New York State Department of Taxation and Finance", TaxCurrencyCode = "USD", RegistrationNumber = "NY-987654321", LegalEntity = "DR Admin USA Inc", TaxName = "Sales Tax", TaxRate = 0.08875m, TaxCategoryCode = "STANDARD", TaxCategoryName = "Standard Sales Tax", TaxCategoryDescription = "New York standard sales tax", ReverseCharge = false, Priority = 90 }
            };

            var existingJurisdictions = await _context.TaxJurisdictions
                .Where(x => taxJurisdictionSeeds.Select(s => s.JurisdictionCode).Contains(x.Code))
                .ToListAsync();

            var jurisdictionsByCode = existingJurisdictions
                .ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

            var insertedTaxJurisdictions = 0;
            foreach (var seed in taxJurisdictionSeeds)
            {
                if (jurisdictionsByCode.ContainsKey(seed.JurisdictionCode))
                {
                    continue;
                }

                var jurisdiction = new TaxJurisdiction
                {
                    Code = seed.JurisdictionCode,
                    Name = seed.Name,
                    CountryCode = seed.CountryCode,
                    StateCode = seed.StateCode,
                    TaxAuthority = seed.Authority,
                    TaxCurrencyCode = seed.TaxCurrencyCode,
                    IsActive = true,
                    Notes = $"Seeded VAT/TAX jurisdiction for {seed.CountryCode}"
                };

                _context.TaxJurisdictions.Add(jurisdiction);
                jurisdictionsByCode[seed.JurisdictionCode] = jurisdiction;
                insertedTaxJurisdictions++;
            }

            if (insertedTaxJurisdictions > 0)
            {
                result.InsertedByTable["TaxJurisdictions"] = insertedTaxJurisdictions;
            }

            var existingCategories = await _context.TaxCategories
                .Where(x => taxSeedCountries.Contains(x.CountryCode))
                .ToListAsync();

            var insertedTaxCategories = 0;
            var categoriesByCountryStateCode = existingCategories.ToDictionary(
                x => $"{x.CountryCode}|{x.StateCode ?? string.Empty}|{x.Code}",
                x => x,
                StringComparer.OrdinalIgnoreCase);

            foreach (var seed in taxJurisdictionSeeds)
            {
                var categoryKey = $"{seed.CountryCode}|{seed.StateCode ?? string.Empty}|{seed.TaxCategoryCode}";
                if (categoriesByCountryStateCode.ContainsKey(categoryKey))
                {
                    continue;
                }

                var category = new TaxCategory
                {
                    CountryCode = seed.CountryCode,
                    StateCode = seed.StateCode,
                    Code = seed.TaxCategoryCode,
                    Name = seed.TaxCategoryName,
                    Description = seed.TaxCategoryDescription,
                    IsActive = true
                };

                _context.TaxCategories.Add(category);
                categoriesByCountryStateCode[categoryKey] = category;
                insertedTaxCategories++;
            }

            if (insertedTaxCategories > 0)
            {
                result.InsertedByTable["TaxCategories"] = insertedTaxCategories;
            }

            var existingRegistrations = await _context.TaxRegistrations
                .Include(x => x.TaxJurisdiction)
                .Where(x => taxJurisdictionSeeds.Select(s => s.RegistrationNumber).Contains(x.RegistrationNumber))
                .ToListAsync();

            var insertedTaxRegistrations = 0;
            foreach (var seed in taxJurisdictionSeeds)
            {
                var exists = existingRegistrations.Any(x =>
                    string.Equals(x.RegistrationNumber, seed.RegistrationNumber, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.TaxJurisdiction.Code, seed.JurisdictionCode, StringComparison.OrdinalIgnoreCase));

                if (exists)
                {
                    continue;
                }

                var jurisdiction = jurisdictionsByCode[seed.JurisdictionCode];
                _context.TaxRegistrations.Add(new TaxRegistration
                {
                    TaxJurisdiction = jurisdiction,
                    LegalEntityName = seed.LegalEntity,
                    RegistrationNumber = seed.RegistrationNumber,
                    EffectiveFrom = taxSeedBaseDate,
                    EffectiveUntil = null,
                    IsActive = true,
                    Notes = $"Seeded VAT/TAX registration for {seed.CountryCode}"
                });

                insertedTaxRegistrations++;
            }

            if (insertedTaxRegistrations > 0)
            {
                result.InsertedByTable["TaxRegistrations"] = insertedTaxRegistrations;
            }

            var existingRules = await _context.TaxRules
                .Where(x => taxSeedCountries.Contains(x.CountryCode) && x.TaxCategory == "STANDARD")
                .ToListAsync();

            var insertedTaxRules = 0;
            foreach (var seed in taxJurisdictionSeeds)
            {
                var alreadyExists = existingRules.Any(x =>
                    string.Equals(x.CountryCode, seed.CountryCode, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.StateCode ?? string.Empty, seed.StateCode ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.TaxCategory, seed.TaxCategoryCode, StringComparison.OrdinalIgnoreCase)
                    && x.IsActive);

                if (alreadyExists)
                {
                    continue;
                }

                var jurisdiction = jurisdictionsByCode[seed.JurisdictionCode];
                var categoryKey = $"{seed.CountryCode}|{seed.StateCode ?? string.Empty}|{seed.TaxCategoryCode}";
                var category = categoriesByCountryStateCode.GetValueOrDefault(categoryKey);

                _context.TaxRules.Add(new TaxRule
                {
                    TaxJurisdiction = jurisdiction,
                    TaxCategoryEntity = category,
                    CountryCode = seed.CountryCode,
                    StateCode = seed.StateCode,
                    TaxName = seed.TaxName,
                    TaxCategory = seed.TaxCategoryCode,
                    TaxRate = seed.TaxRate,
                    IsActive = true,
                    EffectiveFrom = taxSeedBaseDate,
                    EffectiveUntil = null,
                    AppliesToSetupFees = true,
                    AppliesToRecurring = true,
                    ReverseCharge = seed.ReverseCharge,
                    TaxAuthority = seed.Authority,
                    TaxRegistrationNumber = seed.RegistrationNumber,
                    Priority = seed.Priority,
                    InternalNotes = $"Seeded VAT/TAX rule for {seed.CountryCode}"
                });

                insertedTaxRules++;
            }

            if (insertedTaxRules > 0)
            {
                result.InsertedByTable["TaxRules"] = insertedTaxRules;
            }

            var evidenceSeeds = new[]
            {
                new { CountryCode = "NO", StateCode = (string?)null, IpAddress = "84.49.0.10", BuyerTaxId = "NO123456789MVA", BuyerTaxIdValidated = true, Provider = "BuiltInVatValidationProvider" },
                new { CountryCode = "DK", StateCode = (string?)null, IpAddress = "80.62.0.11", BuyerTaxId = "DK12345678", BuyerTaxIdValidated = true, Provider = "BuiltInVatValidationProvider" },
                new { CountryCode = "GB", StateCode = (string?)null, IpAddress = "51.140.0.12", BuyerTaxId = "GB123456789", BuyerTaxIdValidated = true, Provider = "StripeVatValidationProvider" },
                new { CountryCode = "US", StateCode = "NY", IpAddress = "23.45.0.13", BuyerTaxId = "NY-987654321", BuyerTaxIdValidated = false, Provider = "BuiltInVatValidationProvider" }
            };

            var existingEvidence = await _context.TaxDeterminationEvidences
                .Where(x => taxSeedCountries.Contains(x.BuyerCountryCode))
                .ToListAsync();

            var insertedEvidence = 0;
            var evidenceByCountry = new Dictionary<string, TaxDeterminationEvidence>(StringComparer.OrdinalIgnoreCase);
            foreach (var seed in evidenceSeeds)
            {
                var existingItem = existingEvidence.FirstOrDefault(x =>
                    string.Equals(x.BuyerCountryCode, seed.CountryCode, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.BuyerStateCode ?? string.Empty, seed.StateCode ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.IpAddress, seed.IpAddress, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.BuyerTaxId, seed.BuyerTaxId, StringComparison.OrdinalIgnoreCase));

                if (existingItem != null)
                {
                    evidenceByCountry[seed.CountryCode] = existingItem;
                    continue;
                }

                var evidence = new TaxDeterminationEvidence
                {
                    BuyerCountryCode = seed.CountryCode,
                    BuyerStateCode = seed.StateCode,
                    BillingCountryCode = seed.CountryCode,
                    IpAddress = seed.IpAddress,
                    BuyerTaxId = seed.BuyerTaxId,
                    BuyerTaxIdValidated = seed.BuyerTaxIdValidated,
                    VatValidationProvider = seed.Provider,
                    VatValidationRawResponse = "{\"seeded\":true}",
                    CapturedAt = DateTime.UtcNow
                };

                _context.TaxDeterminationEvidences.Add(evidence);
                evidenceByCountry[seed.CountryCode] = evidence;
                insertedEvidence++;
            }

            if (insertedEvidence > 0)
            {
                result.InsertedByTable["TaxDeterminationEvidences"] = insertedEvidence;
            }

            var existingSnapshots = await _context.OrderTaxSnapshots
                .Where(x => x.IdempotencyKey != null && x.IdempotencyKey.StartsWith("seed-tax-"))
                .ToListAsync();

            var availableOrders = await _context.Orders
                .OrderBy(x => x.Id)
                .Take(4)
                .ToListAsync();

            var insertedSnapshots = 0;
            for (var index = 0; index < taxJurisdictionSeeds.Length && index < availableOrders.Count; index++)
            {
                var seed = taxJurisdictionSeeds[index];
                var order = availableOrders[index];
                var idempotencyKey = $"seed-tax-{seed.CountryCode}";

                var snapshotExists = existingSnapshots.Any(x =>
                    x.OrderId == order.Id
                    && string.Equals(x.IdempotencyKey, idempotencyKey, StringComparison.OrdinalIgnoreCase));

                if (snapshotExists)
                {
                    continue;
                }

                var netAmount = 100m + (index * 25m);
                var taxAmount = Math.Round(netAmount * seed.TaxRate, 2, MidpointRounding.AwayFromZero);
                var grossAmount = netAmount + taxAmount;

                _context.OrderTaxSnapshots.Add(new OrderTaxSnapshot
                {
                    OrderId = order.Id,
                    TaxJurisdiction = jurisdictionsByCode[seed.JurisdictionCode],
                    BuyerCountryCode = seed.CountryCode,
                    BuyerStateCode = seed.StateCode,
                    BuyerType = ISPAdmin.Data.Enums.CustomerType.B2B,
                    BuyerTaxId = seed.RegistrationNumber,
                    BuyerTaxIdValidated = seed.ReverseCharge,
                    TaxCurrencyCode = seed.TaxCurrencyCode,
                    DisplayCurrencyCode = seed.TaxCurrencyCode,
                    NetAmount = netAmount,
                    TaxAmount = taxAmount,
                    GrossAmount = grossAmount,
                    AppliedTaxRate = seed.TaxRate,
                    AppliedTaxName = seed.TaxName,
                    ReverseChargeApplied = seed.ReverseCharge,
                    RuleVersion = "seed-v1",
                    IdempotencyKey = idempotencyKey,
                    TaxDeterminationEvidence = evidenceByCountry.GetValueOrDefault(seed.CountryCode),
                    CalculationInputsJson = JsonSerializer.Serialize(new
                    {
                        seeded = true,
                        countryCode = seed.CountryCode,
                        stateCode = seed.StateCode,
                        taxCategory = seed.TaxCategoryCode,
                        orderId = order.Id
                    })
                });

                insertedSnapshots++;
            }

            if (insertedSnapshots > 0)
            {
                result.InsertedByTable["OrderTaxSnapshots"] = insertedSnapshots;
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
    /// Exports the current admin user and MyCompany profile to a debug snapshot file.
    /// </summary>
    /// <param name="fileName">Optional snapshot file name. If omitted, a timestamped file name is generated.</param>
    /// <returns>Summary and payload for the exported debug snapshot.</returns>
    public async Task<AdminUserMyCompanyExportResultDto> ExportAdminUserAndMyCompanyAsync(string? fileName = null)
    {
        var result = new AdminUserMyCompanyExportResultDto();

        try
        {
            var adminUser = await _context.Users
                .AsNoTracking()
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == RoleNames.ADMIN))
                .OrderBy(u => u.Id)
                .FirstOrDefaultAsync();

            if (adminUser == null)
            {
                throw new InvalidOperationException("No admin user was found.");
            }

            var myCompany = await _context.MyCompanies
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync();

            ContactPerson? primaryContactPerson = null;

            if (adminUser.CustomerId.HasValue)
            {
                primaryContactPerson = await _context.ContactPersons
                    .AsNoTracking()
                    .Where(cp => cp.CustomerId == adminUser.CustomerId && cp.IsActive)
                    .OrderByDescending(cp => cp.IsPrimary)
                    .ThenBy(cp => cp.Id)
                    .FirstOrDefaultAsync();
            }

            primaryContactPerson ??= await _context.ContactPersons
                .AsNoTracking()
                .Where(cp => cp.CustomerId == null)
                .OrderByDescending(cp => cp.IsPrimary)
                .ThenBy(cp => cp.Id)
                .FirstOrDefaultAsync();

            var snapshot = new AdminUserMyCompanySnapshotDto
            {
                ExportedAtUtc = DateTime.UtcNow,
                AdminUser = new AdminUserSnapshotDto
                {
                    Username = adminUser.Username,
                    Email = adminUser.Email,
                    PasswordHash = adminUser.PasswordHash,
                    IsActive = adminUser.IsActive,
                    EmailConfirmed = adminUser.EmailConfirmed,
                    IsMailTwoFactorEnabled = adminUser.IsMailTwoFactorEnabled,
                    IsAuthenticatorTwoFactorEnabled = adminUser.IsAuthenticatorTwoFactorEnabled,
                    AuthenticatorKey = adminUser.AuthenticatorKey
                },
                MyCompany = myCompany == null
                    ? null
                    : new MyCompanySnapshotDto
                    {
                        Name = myCompany.Name,
                        LegalName = myCompany.LegalName,
                        Email = myCompany.Email,
                        Phone = myCompany.Phone,
                        AddressLine1 = myCompany.AddressLine1,
                        AddressLine2 = myCompany.AddressLine2,
                        PostalCode = myCompany.PostalCode,
                        City = myCompany.City,
                        State = myCompany.State,
                        CountryCode = myCompany.CountryCode,
                        OrganizationNumber = myCompany.OrganizationNumber,
                        TaxId = myCompany.TaxId,
                        VatNumber = myCompany.VatNumber,
                        InvoiceEmail = myCompany.InvoiceEmail,
                        Website = myCompany.Website,
                        LogoUrl = myCompany.LogoUrl,
                        LetterheadFooter = myCompany.LetterheadFooter
                    },
                PrimaryContactPerson = primaryContactPerson == null
                    ? null
                    : new PrimaryContactPersonSnapshotDto
                    {
                        FirstName = primaryContactPerson.FirstName,
                        LastName = primaryContactPerson.LastName,
                        Email = primaryContactPerson.Email,
                        Phone = primaryContactPerson.Phone,
                        Notes = primaryContactPerson.Notes
                    }
            };

            var snapshotsDirectory = GetDebugSnapshotsDirectory();
            var safeFileName = BuildDebugSnapshotFileName(fileName);
            var filePath = Path.Combine(snapshotsDirectory, safeFileName);

            await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
            {
                WriteIndented = true
            }));

            result.Success = true;
            result.FileName = safeFileName;
            result.FilePath = filePath;
            result.Snapshot = snapshot;

            _log.Information("Exported admin/MyCompany debug snapshot to {FilePath}", filePath);
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error exporting admin/MyCompany debug snapshot");
            result.Success = false;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    /// <summary>
    /// Imports a previously exported admin user and MyCompany profile snapshot from a debug file.
    /// </summary>
    /// <param name="request">Import request containing snapshot file details.</param>
    /// <returns>Summary of the import operation.</returns>
    public async Task<AdminUserMyCompanyImportResultDto> ImportAdminUserAndMyCompanyAsync(AdminUserMyCompanyImportRequestDto request)
    {
        var result = new AdminUserMyCompanyImportResultDto();

        try
        {
            if (string.IsNullOrWhiteSpace(request.FileName))
            {
                throw new InvalidOperationException("FileName is required.");
            }

            var snapshotsDirectory = GetDebugSnapshotsDirectory();
            var safeFileName = Path.GetFileName(request.FileName.Trim());
            var filePath = Path.Combine(snapshotsDirectory, safeFileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Snapshot file not found: {filePath}");
            }

            var json = await File.ReadAllTextAsync(filePath);
            var snapshot = JsonSerializer.Deserialize<AdminUserMyCompanySnapshotDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (snapshot == null)
            {
                throw new InvalidOperationException("Snapshot content is invalid.");
            }

            if (string.IsNullOrWhiteSpace(snapshot.AdminUser.Username) || string.IsNullOrWhiteSpace(snapshot.AdminUser.Email))
            {
                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;
                JsonElement userElement;

                if (root.TryGetProperty("User", out userElement) ||
                    root.TryGetProperty("user", out userElement) ||
                    root.TryGetProperty("AdminUser", out userElement) ||
                    root.TryGetProperty("adminUser", out userElement))
                {
                    if ((userElement.TryGetProperty("Username", out var usernameProp) || userElement.TryGetProperty("username", out usernameProp)) && string.IsNullOrWhiteSpace(snapshot.AdminUser.Username))
                    {
                        snapshot.AdminUser.Username = usernameProp.GetString() ?? string.Empty;
                    }

                    if ((userElement.TryGetProperty("Email", out var emailProp) || userElement.TryGetProperty("email", out emailProp)) && string.IsNullOrWhiteSpace(snapshot.AdminUser.Email))
                    {
                        snapshot.AdminUser.Email = emailProp.GetString() ?? string.Empty;
                    }

                    if ((userElement.TryGetProperty("PasswordHash", out var passwordHashProp) || userElement.TryGetProperty("passwordHash", out passwordHashProp)) && string.IsNullOrWhiteSpace(snapshot.AdminUser.PasswordHash))
                    {
                        snapshot.AdminUser.PasswordHash = passwordHashProp.GetString() ?? string.Empty;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(snapshot.AdminUser.Username) || string.IsNullOrWhiteSpace(snapshot.AdminUser.Email))
            {
                throw new InvalidOperationException("Admin user snapshot is missing required fields.");
            }

            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == RoleNames.ADMIN);
            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Name = RoleNames.ADMIN,
                    Description = "Administrator role",
                    Code = RoleNames.ADMIN
                };

                _context.Roles.Add(adminRole);
                await _context.SaveChangesAsync();
            }

            var adminUser = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserRoles.Any(ur => ur.RoleId == adminRole.Id));

            if (adminUser == null)
            {
                adminUser = await _context.Users
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.Email == snapshot.AdminUser.Email || u.Username == snapshot.AdminUser.Username);

                if (adminUser == null)
                {
                    adminUser = new User();
                    _context.Users.Add(adminUser);
                    result.AdminUserCreated = true;
                }
                else
                {
                    result.AdminUserUpdated = true;
                }
            }
            else
            {
                result.AdminUserUpdated = true;
            }

            adminUser.Username = snapshot.AdminUser.Username;
            adminUser.Email = snapshot.AdminUser.Email;
            adminUser.IsActive = snapshot.AdminUser.IsActive;
            adminUser.EmailConfirmed = result.AdminUserCreated
                ? DateTime.UtcNow
                : snapshot.AdminUser.EmailConfirmed;
            adminUser.IsMailTwoFactorEnabled = snapshot.AdminUser.IsMailTwoFactorEnabled;
            adminUser.IsAuthenticatorTwoFactorEnabled = snapshot.AdminUser.IsAuthenticatorTwoFactorEnabled;
            adminUser.AuthenticatorKey = snapshot.AdminUser.AuthenticatorKey;

            if (!string.IsNullOrWhiteSpace(snapshot.AdminUser.PasswordHash))
            {
                adminUser.PasswordHash = snapshot.AdminUser.PasswordHash;
            }

            await _context.SaveChangesAsync();

            var hasAdminRole = await _context.UserRoles.AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id);
            if (!hasAdminRole)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                });
            }

            var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == RoleNames.CUSTOMER);
            if (customerRole == null)
            {
                customerRole = new Role
                {
                    Name = RoleNames.CUSTOMER,
                    Description = "Customer role",
                    Code = RoleNames.CUSTOMER
                };

                _context.Roles.Add(customerRole);
                await _context.SaveChangesAsync();
            }

            var hasCustomerRole = await _context.UserRoles.AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == customerRole.Id);
            if (!hasCustomerRole)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = customerRole.Id
                });
            }

            if (!adminUser.CustomerId.HasValue)
            {
                var existingCustomer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Email == adminUser.Email);

                if (existingCustomer == null)
                {
                    var firstName = snapshot.PrimaryContactPerson?.FirstName?.Trim();
                    var lastName = snapshot.PrimaryContactPerson?.LastName?.Trim();
                    var customerName = string.Join(' ', new[] { firstName, lastName }.Where(x => !string.IsNullOrWhiteSpace(x))).Trim();

                    if (string.IsNullOrWhiteSpace(customerName))
                    {
                        customerName = snapshot.MyCompany?.Name?.Trim();
                    }

                    if (string.IsNullOrWhiteSpace(customerName))
                    {
                        customerName = adminUser.Username;
                    }

                    existingCustomer = new Customer
                    {
                        Name = customerName,
                        Email = adminUser.Email,
                        Phone = snapshot.PrimaryContactPerson?.Phone
                            ?? snapshot.MyCompany?.Phone
                            ?? string.Empty,
                        IsSelfRegistered = true,
                        IsActive = true,
                        Status = "Active"
                    };

                    _context.Customers.Add(existingCustomer);
                    await _context.SaveChangesAsync();
                }

                adminUser.CustomerId = existingCustomer.Id;
            }

            var myCompanySnapshot = snapshot.MyCompany;
            if (myCompanySnapshot != null)
            {
                var myCompany = await _context.MyCompanies
                    .OrderBy(x => x.Id)
                    .FirstOrDefaultAsync();

                if (myCompany == null)
                {
                    myCompany = new MyCompany();
                    _context.MyCompanies.Add(myCompany);
                    result.MyCompanyCreated = true;
                }
                else
                {
                    result.MyCompanyUpdated = true;
                }

                myCompany.Name = myCompanySnapshot.Name;
                myCompany.LegalName = myCompanySnapshot.LegalName;
                myCompany.Email = myCompanySnapshot.Email;
                myCompany.Phone = myCompanySnapshot.Phone;
                myCompany.AddressLine1 = myCompanySnapshot.AddressLine1;
                myCompany.AddressLine2 = myCompanySnapshot.AddressLine2;
                myCompany.PostalCode = myCompanySnapshot.PostalCode;
                myCompany.City = myCompanySnapshot.City;
                myCompany.State = myCompanySnapshot.State;
                myCompany.CountryCode = myCompanySnapshot.CountryCode;
                myCompany.OrganizationNumber = myCompanySnapshot.OrganizationNumber;
                myCompany.TaxId = myCompanySnapshot.TaxId;
                myCompany.VatNumber = myCompanySnapshot.VatNumber;
                myCompany.InvoiceEmail = myCompanySnapshot.InvoiceEmail;
                myCompany.Website = myCompanySnapshot.Website;
                myCompany.LogoUrl = myCompanySnapshot.LogoUrl;
                myCompany.LetterheadFooter = myCompanySnapshot.LetterheadFooter;
            }

            var primaryContactSnapshot = snapshot.PrimaryContactPerson;
            if (primaryContactSnapshot != null)
            {
                var primaryContactPerson = await _context.ContactPersons
                    .Where(cp => cp.CustomerId == adminUser.CustomerId)
                    .OrderByDescending(cp => cp.IsPrimary)
                    .ThenBy(cp => cp.Id)
                    .FirstOrDefaultAsync();

                if (primaryContactPerson == null)
                {
                    primaryContactPerson = new ContactPerson();
                    _context.ContactPersons.Add(primaryContactPerson);
                    result.PrimaryContactPersonCreated = true;
                }
                else
                {
                    result.PrimaryContactPersonUpdated = true;
                }

                primaryContactPerson.CustomerId = adminUser.CustomerId;
                primaryContactPerson.FirstName = primaryContactSnapshot.FirstName;
                primaryContactPerson.LastName = primaryContactSnapshot.LastName;
                primaryContactPerson.Email = primaryContactSnapshot.Email;
                primaryContactPerson.Phone = primaryContactSnapshot.Phone;
                primaryContactPerson.Notes = primaryContactSnapshot.Notes;
                primaryContactPerson.IsPrimary = true;
                primaryContactPerson.IsActive = true;
                primaryContactPerson.IsDefaultOwner = true;
                primaryContactPerson.IsDefaultAdministrator = true;
                primaryContactPerson.IsDefaultTech = true;
                primaryContactPerson.IsDefaultBilling = true;
                primaryContactPerson.IsDomainGlobal = true;
            }

            await _context.SaveChangesAsync();

            result.Success = true;
            result.FilePath = filePath;

            _log.Information("Imported admin/MyCompany debug snapshot from {FilePath}", filePath);
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error importing admin/MyCompany debug snapshot");
            result.Success = false;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    public async Task<AdminUserMyCompanyImportResultDto> ImportCustomerUserSnapshotAsync(AdminUserMyCompanyImportRequestDto request)
    {
        var adminExists = await _context.Users
            .AnyAsync(u => u.UserRoles.Any(ur => ur.Role.Name == RoleNames.ADMIN));

        if (!adminExists)
        {
            return new AdminUserMyCompanyImportResultDto
            {
                Success = false,
                ErrorMessage = "Customer with user and contact person cannot be imported before an admin user exists."
            };
        }

        return await ImportCustomerUserSnapshotFileAsync(request);
    }

    private async Task<AdminUserMyCompanyImportResultDto> ImportCustomerUserSnapshotFileAsync(AdminUserMyCompanyImportRequestDto request)
    {
        var result = new AdminUserMyCompanyImportResultDto();
        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transaction = null;

        try
        {
            if (string.IsNullOrWhiteSpace(request.FileName))
            {
                throw new InvalidOperationException("FileName is required.");
            }

            var snapshotsDirectory = GetDebugSnapshotsDirectory();
            var safeFileName = Path.GetFileName(request.FileName.Trim());
            var filePath = Path.Combine(snapshotsDirectory, safeFileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Snapshot file not found: {filePath}");
            }

            var json = await File.ReadAllTextAsync(filePath);
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            transaction = await _context.Database.BeginTransactionAsync();

            JsonElement GetObject(params string[] names)
            {
                foreach (var name in names)
                {
                    if (root.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.Object)
                    {
                        return value;
                    }
                }

                return default;
            }

            var userObject = GetObject("User", "user", "AdminUser", "adminUser");
            if (userObject.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidOperationException("User snapshot object is missing.");
            }

            var username = userObject.TryGetProperty("Username", out var usernameProp) || userObject.TryGetProperty("username", out usernameProp)
                ? usernameProp.GetString() ?? string.Empty
                : string.Empty;

            var email = userObject.TryGetProperty("Email", out var emailProp) || userObject.TryGetProperty("email", out emailProp)
                ? emailProp.GetString() ?? string.Empty
                : string.Empty;

            var passwordHash = userObject.TryGetProperty("PasswordHash", out var passwordHashProp) || userObject.TryGetProperty("passwordHash", out passwordHashProp)
                ? passwordHashProp.GetString() ?? string.Empty
                : string.Empty;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
            {
                throw new InvalidOperationException("User snapshot is missing required fields Username or Email.");
            }

            var customerObject = GetObject("Customer", "customer", "MyCompany", "myCompany");
            var contactObject = GetObject("PrimaryContactPerson", "primaryContactPerson", "ContactPerson", "contactPerson");

            var customerName = customerObject.ValueKind == JsonValueKind.Object &&
                (customerObject.TryGetProperty("Name", out var customerNameProp) || customerObject.TryGetProperty("name", out customerNameProp))
                ? customerNameProp.GetString() ?? string.Empty
                : string.Empty;

            var customerEmail = customerObject.ValueKind == JsonValueKind.Object &&
                (customerObject.TryGetProperty("Email", out var customerEmailProp) || customerObject.TryGetProperty("email", out customerEmailProp))
                ? customerEmailProp.GetString() ?? string.Empty
                : string.Empty;

            var customerPhone = customerObject.ValueKind == JsonValueKind.Object &&
                (customerObject.TryGetProperty("Phone", out var customerPhoneProp) || customerObject.TryGetProperty("phone", out customerPhoneProp))
                ? customerPhoneProp.GetString() ?? string.Empty
                : string.Empty;

            if (string.IsNullOrWhiteSpace(customerName))
            {
                customerName = username;
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email || (!string.IsNullOrWhiteSpace(customerEmail) && c.Email == customerEmail));

            if (customer == null)
            {
                customer = new Customer
                {
                    Name = customerName,
                    Email = string.IsNullOrWhiteSpace(customerEmail) ? email : customerEmail,
                    Phone = customerPhone,
                    IsSelfRegistered = true,
                    IsActive = true,
                    Status = "Active"
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }

            var emailExists = await _context.Users.AnyAsync(u => u.Email == email);
            if (emailExists)
            {
                throw new InvalidOperationException($"A user with email '{email}' already exists. Import requires a new unique user.");
            }

            var usernameExists = await _context.Users.AnyAsync(u => u.Username == username);
            if (usernameExists)
            {
                throw new InvalidOperationException($"A user with username '{username}' already exists. Import requires a new unique user.");
            }

            var createdAtUtc = DateTime.UtcNow;

            var user = new User
            {
                Username = username,
                Email = email,
                CustomerId = customer.Id,
                IsActive = true,
                PasswordHash = string.IsNullOrWhiteSpace(passwordHash) ? string.Empty : passwordHash,
                CreatedAt = createdAtUtc,
                UpdatedAt = createdAtUtc,
                EmailConfirmed = createdAtUtc
            };

            _context.Users.Add(user);
            result.AdminUserCreated = true;

            var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == RoleNames.CUSTOMER);
            if (customerRole == null)
            {
                customerRole = new Role
                {
                    Name = RoleNames.CUSTOMER,
                    Description = "Customer role",
                    Code = RoleNames.CUSTOMER
                };

                _context.Roles.Add(customerRole);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();

            var hasCustomerRole = await _context.UserRoles.AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == customerRole.Id);
            if (!hasCustomerRole)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = customerRole.Id
                });
            }

            if (contactObject.ValueKind == JsonValueKind.Object)
            {
                var firstName = contactObject.TryGetProperty("FirstName", out var firstNameProp) || contactObject.TryGetProperty("firstName", out firstNameProp)
                    ? firstNameProp.GetString() ?? string.Empty
                    : string.Empty;
                var lastName = contactObject.TryGetProperty("LastName", out var lastNameProp) || contactObject.TryGetProperty("lastName", out lastNameProp)
                    ? lastNameProp.GetString() ?? string.Empty
                    : string.Empty;
                var contactEmail = contactObject.TryGetProperty("Email", out var contactEmailProp) || contactObject.TryGetProperty("email", out contactEmailProp)
                    ? contactEmailProp.GetString() ?? string.Empty
                    : email;
                var contactPhone = contactObject.TryGetProperty("Phone", out var contactPhoneProp) || contactObject.TryGetProperty("phone", out contactPhoneProp)
                    ? contactPhoneProp.GetString() ?? string.Empty
                    : customerPhone;
                var notes = contactObject.TryGetProperty("Notes", out var notesProp) || contactObject.TryGetProperty("notes", out notesProp)
                    ? notesProp.GetString()
                    : null;

                var primaryContact = await _context.ContactPersons
                    .Where(cp => cp.CustomerId == customer.Id)
                    .OrderByDescending(cp => cp.IsPrimary)
                    .ThenBy(cp => cp.Id)
                    .FirstOrDefaultAsync();

                if (primaryContact == null)
                {
                    primaryContact = new ContactPerson();
                    _context.ContactPersons.Add(primaryContact);
                    result.PrimaryContactPersonCreated = true;
                }
                else
                {
                    result.PrimaryContactPersonUpdated = true;
                }

                primaryContact.CustomerId = customer.Id;
                primaryContact.FirstName = firstName;
                primaryContact.LastName = lastName;
                primaryContact.Email = contactEmail;
                primaryContact.Phone = contactPhone;
                primaryContact.Notes = notes;
                primaryContact.IsPrimary = true;
                primaryContact.IsActive = true;
                primaryContact.IsDefaultOwner = true;
                primaryContact.IsDefaultAdministrator = true;
                primaryContact.IsDefaultTech = true;
                primaryContact.IsDefaultBilling = true;
                primaryContact.IsDomainGlobal = true;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            result.Success = true;
            result.FilePath = filePath;

            _log.Information("Imported customer user snapshot from {FilePath}", filePath);
            return result;
        }
        catch (Exception ex)
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync();
            }

            _log.Error(ex, "Error importing customer user snapshot");
            result.Success = false;
            result.ErrorMessage = ex.Message;
            return result;
        }
        finally
        {
            if (transaction != null)
            {
                await transaction.DisposeAsync();
            }
        }
    }

    private string GetDebugSnapshotsDirectory()
    {
        var contentRoot = string.IsNullOrWhiteSpace(_hostEnvironment.ContentRootPath)
            ? Directory.GetCurrentDirectory()
            : _hostEnvironment.ContentRootPath;

        var snapshotsDirectory = Path.Combine(contentRoot, "DebugSnapshots");
        Directory.CreateDirectory(snapshotsDirectory);
        return snapshotsDirectory;
    }

    private static string BuildDebugSnapshotFileName(string? fileName)
    {
        var candidate = string.IsNullOrWhiteSpace(fileName)
            ? $"admin-mycompany-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json"
            : Path.GetFileName(fileName.Trim());

        if (!candidate.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            candidate += ".json";
        }

        return candidate;
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



