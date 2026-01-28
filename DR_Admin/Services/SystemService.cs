using System.Diagnostics;
using ISPAdmin.Data;
using ISPAdmin.Utilities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for system-level operations including data normalization
/// </summary>
public class SystemService : ISystemService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<SystemService>();

    public SystemService(ApplicationDbContext context)
    {
        _context = context;
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
            customer.NormalizedContactPerson = NormalizationHelper.Normalize(customer.ContactPerson);
        }

        await _context.SaveChangesAsync();
        return customers.Count;
    }

    private async Task<int> NormalizeDomainsAsync()
    {
        var domains = await _context.Domains.ToListAsync();
        
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
}
