using ISPAdmin.Data;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for running DNS troubleshooting checks.
/// </summary>
public class DnsTroubleshootService : IDnsTroubleshootService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<DnsTroubleshootService>();

    /// <summary>
    /// Initializes a new instance of the <see cref="DnsTroubleshootService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public DnsTroubleshootService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Runs DNS troubleshoot tests for a domain.
    /// </summary>
    /// <param name="domainId">The domain identifier.</param>
    /// <returns>A troubleshooting report for the domain.</returns>
    public async Task<DnsTroubleshootReportDto?> RunForDomainAsync(int domainId)
    {
        _log.Information("Running DNS troubleshoot for domain ID {DomainId}", domainId);

        var domain = await _context.RegisteredDomains
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == domainId);

        if (domain == null)
        {
            _log.Information("DNS troubleshoot domain not found for ID {DomainId}", domainId);
            return null;
        }

        var records = await _context.DnsRecords
            .AsNoTracking()
            .Include(x => x.DnsRecordType)
            .Where(x => x.DomainId == domainId)
            .ToListAsync();

        var activeRecords = records.Where(x => !x.IsDeleted).ToList();

        var report = new DnsTroubleshootReportDto
        {
            DomainId = domain.Id,
            DomainName = domain.Name,
            GeneratedAtUtc = DateTime.UtcNow,
            Tests = []
        };

        var dnsZonesFixUrl = $"/dns/zones?domain-id={domain.Id}";

        AddRecordPresenceTest(report.Tests, activeRecords.Count, dnsZonesFixUrl);
        AddPendingSyncTest(report.Tests, activeRecords.Count(x => x.IsPendingSync), dnsZonesFixUrl);
        AddDuplicateRecordTest(report.Tests, activeRecords, dnsZonesFixUrl);
        AddTtlRangeTest(report.Tests, activeRecords, dnsZonesFixUrl);
        AddMxPriorityTest(report.Tests, activeRecords, dnsZonesFixUrl);
        AddCnameConflictTest(report.Tests, activeRecords, domain.Name, dnsZonesFixUrl);

        return report;
    }

    private static void AddRecordPresenceTest(List<DnsTroubleshootTestResultDto> tests, int activeRecordCount, string fixUrl)
    {
        tests.Add(new DnsTroubleshootTestResultDto
        {
            Key = "record-presence",
            Name = "DNS records present",
            Severity = activeRecordCount > 0 ? "Info" : "Error",
            Passed = activeRecordCount > 0,
            Message = activeRecordCount > 0
                ? $"Found {activeRecordCount} active DNS record(s)."
                : "No active DNS records found.",
            Details = activeRecordCount > 0 ? null : "Create required DNS records before go-live.",
            FixUrl = fixUrl
        });
    }

    private static void AddPendingSyncTest(List<DnsTroubleshootTestResultDto> tests, int pendingSyncCount, string fixUrl)
    {
        tests.Add(new DnsTroubleshootTestResultDto
        {
            Key = "pending-sync",
            Name = "Pending sync check",
            Severity = pendingSyncCount > 0 ? "Warning" : "Info",
            Passed = pendingSyncCount == 0,
            Message = pendingSyncCount > 0
                ? $"{pendingSyncCount} DNS record(s) are pending sync to DNS server."
                : "All DNS records are synced.",
            Details = pendingSyncCount > 0 ? "Push pending records to DNS server from DNS Zones page." : null,
            FixUrl = fixUrl
        });
    }

    private static void AddDuplicateRecordTest(List<DnsTroubleshootTestResultDto> tests, List<Data.Entities.DnsRecord> activeRecords, string fixUrl)
    {
        var duplicateGroups = activeRecords
            .GroupBy(x => new
            {
                Type = (x.DnsRecordType?.Type ?? string.Empty).Trim().ToUpperInvariant(),
                Name = (x.Name ?? string.Empty).Trim().ToLowerInvariant(),
                Value = (x.Value ?? string.Empty).Trim().ToLowerInvariant()
            })
            .Where(g => g.Count() > 1)
            .ToList();

        tests.Add(new DnsTroubleshootTestResultDto
        {
            Key = "duplicate-records",
            Name = "Duplicate DNS records",
            Severity = duplicateGroups.Count > 0 ? "Warning" : "Info",
            Passed = duplicateGroups.Count == 0,
            Message = duplicateGroups.Count > 0
                ? $"Found {duplicateGroups.Count} duplicate DNS record group(s)."
                : "No duplicate DNS records found.",
            Details = duplicateGroups.Count > 0
                ? string.Join("; ", duplicateGroups.Take(5).Select(g => $"{g.Key.Type} {g.Key.Name} {g.Key.Value}"))
                : null,
            FixUrl = fixUrl
        });
    }

    private static void AddTtlRangeTest(List<DnsTroubleshootTestResultDto> tests, List<Data.Entities.DnsRecord> activeRecords, string fixUrl)
    {
        var outOfRangeCount = activeRecords.Count(x =>
        {
            var type = (x.DnsRecordType?.Type ?? string.Empty).Trim().ToUpperInvariant();

            var minTtl = type == "SOA" ? 900 : 60;
            var maxTtl = type == "NS" ? 172800 : 86400;

            if (x.TTL < minTtl)
            {
                return true;
            }

            return x.TTL > maxTtl;
        });

        tests.Add(new DnsTroubleshootTestResultDto
        {
            Key = "ttl-range",
            Name = "TTL range check",
            Severity = outOfRangeCount > 0 ? "Warning" : "Info",
            Passed = outOfRangeCount == 0,
            Message = outOfRangeCount > 0
                ? $"{outOfRangeCount} DNS record(s) have TTL outside recommended range (60-86400, NS up to 172800, SOA from 900)."
                : "All DNS record TTL values are within recommended range.",
            Details = outOfRangeCount > 0 ? "Use sane TTL values to improve cache behavior and propagation balance. NS records may use TTL up to 172800, SOA should be at least 900." : null,
            FixUrl = fixUrl
        });
    }

    private static void AddMxPriorityTest(List<DnsTroubleshootTestResultDto> tests, List<Data.Entities.DnsRecord> activeRecords, string fixUrl)
    {
        var mxMissingPriorityCount = activeRecords.Count(x =>
            string.Equals(x.DnsRecordType?.Type, "MX", StringComparison.OrdinalIgnoreCase) &&
            !x.Priority.HasValue);

        tests.Add(new DnsTroubleshootTestResultDto
        {
            Key = "mx-priority",
            Name = "MX priority check",
            Severity = mxMissingPriorityCount > 0 ? "Error" : "Info",
            Passed = mxMissingPriorityCount == 0,
            Message = mxMissingPriorityCount > 0
                ? $"{mxMissingPriorityCount} MX record(s) are missing priority."
                : "All MX records have priority set.",
            Details = mxMissingPriorityCount > 0 ? "Set priority for all MX records." : null,
            FixUrl = fixUrl
        });
    }

    private static void AddCnameConflictTest(List<DnsTroubleshootTestResultDto> tests, List<Data.Entities.DnsRecord> activeRecords, string domainName, string fixUrl)
    {
        var cnameRecordNames = activeRecords
            .Where(x => string.Equals(x.DnsRecordType?.Type, "CNAME", StringComparison.OrdinalIgnoreCase))
            .Select(x => (x.Name ?? string.Empty).Trim().ToLowerInvariant())
            .Where(x => x.Length > 0)
            .ToHashSet();

        var conflictingNames = activeRecords
            .Where(x => !string.Equals(x.DnsRecordType?.Type, "CNAME", StringComparison.OrdinalIgnoreCase))
            .Select(x => (x.Name ?? string.Empty).Trim().ToLowerInvariant())
            .Where(name => cnameRecordNames.Contains(name))
            .Distinct()
            .ToList();

        var apexConflict = activeRecords.Any(x =>
            string.Equals(x.DnsRecordType?.Type, "CNAME", StringComparison.OrdinalIgnoreCase) &&
            (string.Equals((x.Name ?? string.Empty).Trim(), "@", StringComparison.OrdinalIgnoreCase)
             || string.Equals((x.Name ?? string.Empty).Trim(), domainName, StringComparison.OrdinalIgnoreCase)));

        var hasConflict = conflictingNames.Count > 0 || apexConflict;

        tests.Add(new DnsTroubleshootTestResultDto
        {
            Key = "cname-conflicts",
            Name = "CNAME conflict check",
            Severity = hasConflict ? "Error" : "Info",
            Passed = !hasConflict,
            Message = hasConflict
                ? "CNAME conflicts detected with other record types or at zone apex."
                : "No CNAME conflicts detected.",
            Details = hasConflict
                ? string.Join("; ", conflictingNames.Take(5).Select(name => $"Conflict on '{name}'")) + (apexConflict ? (conflictingNames.Count > 0 ? "; " : string.Empty) + "Apex CNAME detected" : string.Empty)
                : null,
            FixUrl = fixUrl
        });
    }
}
