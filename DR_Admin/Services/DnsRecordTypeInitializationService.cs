using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class DnsRecordTypeInitializationService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<DnsRecordTypeInitializationService>();

    public DnsRecordTypeInitializationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task InitializeDnsRecordTypesAsync()
    {
        try
        {
            _log.Information("Initializing DNS record types");

            var defaultDnsRecordTypes = new List<DnsRecordType>
            {
                new DnsRecordType
                {
                    Type = "A",
                    Description = "IPv4 address record - Maps a domain name to an IPv4 address",
                    HasPriority = false,
                    HasWeight = false,
                    HasPort = false,
                    IsEditableByUser = true,
                    IsActive = true,
                    DefaultTTL = 3600,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new DnsRecordType
                {
                    Type = "AAAA",
                    Description = "IPv6 address record - Maps a domain name to an IPv6 address",
                    HasPriority = false,
                    HasWeight = false,
                    HasPort = false,
                    IsEditableByUser = true,
                    IsActive = true,
                    DefaultTTL = 3600,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new DnsRecordType
                {
                    Type = "CNAME",
                    Description = "Canonical name record - Creates an alias from one domain name to another",
                    HasPriority = false,
                    HasWeight = false,
                    HasPort = false,
                    IsEditableByUser = true,
                    IsActive = true,
                    DefaultTTL = 3600,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new DnsRecordType
                {
                    Type = "MX",
                    Description = "Mail exchange record - Specifies mail servers for the domain",
                    HasPriority = true,
                    HasWeight = false,
                    HasPort = false,
                    IsEditableByUser = true,
                    IsActive = true,
                    DefaultTTL = 3600,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new DnsRecordType
                {
                    Type = "TXT",
                    Description = "Text record - Stores arbitrary text information (SPF, DKIM, verification)",
                    HasPriority = false,
                    HasWeight = false,
                    HasPort = false,
                    IsEditableByUser = true,
                    IsActive = true,
                    DefaultTTL = 3600,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new DnsRecordType
                {
                    Type = "NS",
                    Description = "Name server record - Specifies authoritative name servers for the domain",
                    HasPriority = false,
                    HasWeight = false,
                    HasPort = false,
                    IsEditableByUser = false, // System-managed
                    IsActive = true,
                    DefaultTTL = 86400,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new DnsRecordType
                {
                    Type = "SRV",
                    Description = "Service record - Specifies location of services (requires priority, weight, and port)",
                    HasPriority = true,
                    HasWeight = true,
                    HasPort = true,
                    IsEditableByUser = true,
                    IsActive = true,
                    DefaultTTL = 3600,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new DnsRecordType
                {
                    Type = "CAA",
                    Description = "Certification Authority Authorization - Specifies which CAs can issue certificates",
                    HasPriority = false,
                    HasWeight = false,
                    HasPort = false,
                    IsEditableByUser = true,
                    IsActive = true,
                    DefaultTTL = 3600,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new DnsRecordType
                {
                    Type = "PTR",
                    Description = "Pointer record - Used for reverse DNS lookups",
                    HasPriority = false,
                    HasWeight = false,
                    HasPort = false,
                    IsEditableByUser = true,
                    IsActive = true,
                    DefaultTTL = 86400,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new DnsRecordType
                {
                    Type = "SOA",
                    Description = "Start of Authority record - Specifies authoritative information about DNS zone",
                    HasPriority = false,
                    HasWeight = false,
                    HasPort = false,
                    IsEditableByUser = false, // System-managed
                    IsActive = true,
                    DefaultTTL = 3600,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            foreach (var dnsRecordType in defaultDnsRecordTypes)
            {
                var existing = await _context.DnsRecordTypes.FirstOrDefaultAsync(t => t.Type == dnsRecordType.Type);
                if (existing == null)
                {
                    _context.DnsRecordTypes.Add(dnsRecordType);
                    _log.Information("Adding DNS record type: {Type}", dnsRecordType.Type);
                    continue;
                }

                var changed = false;

                if (existing.Description != dnsRecordType.Description)
                {
                    existing.Description = dnsRecordType.Description;
                    changed = true;
                }
                if (existing.HasPriority != dnsRecordType.HasPriority)
                {
                    existing.HasPriority = dnsRecordType.HasPriority;
                    changed = true;
                }
                if (existing.HasWeight != dnsRecordType.HasWeight)
                {
                    existing.HasWeight = dnsRecordType.HasWeight;
                    changed = true;
                }
                if (existing.HasPort != dnsRecordType.HasPort)
                {
                    existing.HasPort = dnsRecordType.HasPort;
                    changed = true;
                }
                if (existing.IsEditableByUser != dnsRecordType.IsEditableByUser)
                {
                    existing.IsEditableByUser = dnsRecordType.IsEditableByUser;
                    changed = true;
                }
                if (existing.IsActive != dnsRecordType.IsActive)
                {
                    existing.IsActive = dnsRecordType.IsActive;
                    changed = true;
                }
                if (existing.DefaultTTL != dnsRecordType.DefaultTTL)
                {
                    existing.DefaultTTL = dnsRecordType.DefaultTTL;
                    changed = true;
                }

                if (changed)
                {
                    existing.UpdatedAt = DateTime.UtcNow;
                    _log.Information("Updated DNS record type defaults: {Type}", existing.Type);
                }
            }

            await _context.SaveChangesAsync();
            _log.Information("DNS record types initialization completed successfully");
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while initializing DNS record types");
            throw;
        }
    }
}
