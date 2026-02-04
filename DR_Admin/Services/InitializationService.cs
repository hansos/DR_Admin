using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

using static ISPAdmin.Infrastructure.RoleNames;

public class InitializationService : IInitializationService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<InitializationService>();

    public InitializationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CodeTablesResponseDto> CheckAndUpdateCodeTablesAsync()
    {
        var response = new CodeTablesResponseDto { Success = true, Message = "Code tables checked and updated successfully" };

        try
        {
            var now = DateTime.UtcNow;

            // Ensure all standard roles exist
            var rolesAdded = 0;
            var standardRoles = new[] { ADMIN, SUPPORT, SALES, FINANCE, CUSTOMER, DOMAIN };
            foreach (var roleName in standardRoles)
            {
                var existing = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (existing == null)
                {
                    var description = roleName == ADMIN ? "Administrator with full system access" : roleName + " role";
                    _context.Roles.Add(new Role
                    {
                        Name = roleName,
                        Description = description
                    });
                    rolesAdded++;
                    _log.Information("Created role: {Role}", roleName);
                }
            }
            await _context.SaveChangesAsync();
            response.RolesAdded = rolesAdded;
            response.TotalRoles = await _context.Roles.CountAsync();

            // Ensure customer statuses exist
            var statusesAdded = 0;
            var statuses = new[]
            {
                new CustomerStatus { Code = "ACTIVE", Name = "Active", Color = "#28a745", IsActive = true, IsDefault = false, SortOrder = 20, NormalizedCode = "ACTIVE", NormalizedName = "ACTIVE", CreatedAt = now, UpdatedAt = now },
                new CustomerStatus { Code = "SUSPENDED", Name = "Suspended", Color = "#a82955", IsActive = true, IsDefault = false, SortOrder = 40, NormalizedCode = "SUSPENDED", NormalizedName = "SUSPENDED", CreatedAt = now, UpdatedAt = now },
                new CustomerStatus { Code = "PENDING", Name = "Pending", Color = "#dfcf26", IsActive = true, IsDefault = true, SortOrder = 10, NormalizedCode = "PENDING", NormalizedName = "PENDING", CreatedAt = now, UpdatedAt = now },
                new CustomerStatus { Code = "INACTIVE", Name = "Inactive", Color = "#000000", IsActive = true, IsDefault = false, SortOrder = 30, NormalizedCode = "INACTIVE", NormalizedName = "INACTIVE", CreatedAt = now, UpdatedAt = now }
            };

            foreach (var cs in statuses)
            {
                var exists = await _context.CustomerStatuses.FirstOrDefaultAsync(x => x.Code == cs.Code);
                if (exists == null)
                {
                    _context.CustomerStatuses.Add(cs);
                    statusesAdded++;
                    _log.Information("Created customer status: {Code}", cs.Code);
                }
            }
            await _context.SaveChangesAsync();
            response.CustomerStatusesAdded = statusesAdded;
            response.TotalCustomerStatuses = await _context.CustomerStatuses.CountAsync();

            // Ensure common DNS record types exist
            var dnsTypesAdded = 0;
            var dnsTypes = new[]
            {
                new DnsRecordType { Type = "A", Description = "IPv4 address record", HasPriority = false, HasWeight = false, HasPort = false, IsEditableByUser = true, IsActive = true, DefaultTTL = 3600, CreatedAt = now, UpdatedAt = now },
                new DnsRecordType { Type = "AAAA", Description = "IPv6 address record", HasPriority = false, HasWeight = false, HasPort = false, IsEditableByUser = true, IsActive = true, DefaultTTL = 3600, CreatedAt = now, UpdatedAt = now },
                new DnsRecordType { Type = "CNAME", Description = "Canonical name (alias) record", HasPriority = false, HasWeight = false, HasPort = false, IsEditableByUser = true, IsActive = true, DefaultTTL = 3600, CreatedAt = now, UpdatedAt = now },
                new DnsRecordType { Type = "MX", Description = "Mail exchange record", HasPriority = true, HasWeight = false, HasPort = false, IsEditableByUser = true, IsActive = true, DefaultTTL = 3600, CreatedAt = now, UpdatedAt = now },
                new DnsRecordType { Type = "NS", Description = "Name server record", HasPriority = false, HasWeight = false, HasPort = false, IsEditableByUser = true, IsActive = true, DefaultTTL = 3600, CreatedAt = now, UpdatedAt = now },
                new DnsRecordType { Type = "TXT", Description = "Text record (SPF, DKIM, etc.)", HasPriority = false, HasWeight = false, HasPort = false, IsEditableByUser = true, IsActive = true, DefaultTTL = 3600, CreatedAt = now, UpdatedAt = now },
                new DnsRecordType { Type = "SRV", Description = "Service locator record", HasPriority = true, HasWeight = true, HasPort = true, IsEditableByUser = true, IsActive = true, DefaultTTL = 3600, CreatedAt = now, UpdatedAt = now },
                new DnsRecordType { Type = "PTR", Description = "Pointer record (reverse DNS)", HasPriority = false, HasWeight = false, HasPort = false, IsEditableByUser = true, IsActive = true, DefaultTTL = 3600, CreatedAt = now, UpdatedAt = now },
                new DnsRecordType { Type = "SOA", Description = "Start of authority record", HasPriority = false, HasWeight = false, HasPort = false, IsEditableByUser = false, IsActive = true, DefaultTTL = 3600, CreatedAt = now, UpdatedAt = now },
                new DnsRecordType { Type = "CAA", Description = "Certification Authority Authorization", HasPriority = false, HasWeight = false, HasPort = false, IsEditableByUser = true, IsActive = true, DefaultTTL = 3600, CreatedAt = now, UpdatedAt = now }
            };

            foreach (var dt in dnsTypes)
            {
                var exists = await _context.DnsRecordTypes.FirstOrDefaultAsync(x => x.Type.ToUpper() == dt.Type.ToUpper());
                if (exists == null)
                {
                    _context.DnsRecordTypes.Add(dt);
                    dnsTypesAdded++;
                    _log.Information("Created DNS record type: {Type}", dt.Type);
                }
            }
            await _context.SaveChangesAsync();
            response.DnsRecordTypesAdded = dnsTypesAdded;
            response.TotalDnsRecordTypes = await _context.DnsRecordTypes.CountAsync();

            // Ensure common service types exist
            var serviceTypesAdded = 0;
            var serviceTypes = new[]
            {
                new ServiceType { Name = "DOMAIN", Description = "Domain registration and management services", CreatedAt = now, UpdatedAt = now },
                new ServiceType { Name = "HOSTING", Description = "Web hosting and server services", CreatedAt = now, UpdatedAt = now }
            };

            foreach (var st in serviceTypes)
            {
                var exists = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Name == st.Name);
                if (exists == null)
                {
                    _context.ServiceTypes.Add(st);
                    serviceTypesAdded++;
                    _log.Information("Created service type: {Name}", st.Name);
                }
            }
            await _context.SaveChangesAsync();
            response.ServiceTypesAdded = serviceTypesAdded;
            response.TotalServiceTypes = await _context.ServiceTypes.CountAsync();

            // Ensure address types exist
            var addressTypesAdded = 0;
            var addressTypes = new[]
            {
                new AddressType { Code = "OFFICE", Name = "Office Address", Description = "Office or business address", IsActive = true, IsDefault = false, SortOrder = 10, NormalizedCode = "OFFICE", NormalizedName = "OFFICE", CreatedAt = now, UpdatedAt = now },
                new AddressType { Code = "POSTAL", Name = "Postal Address", Description = "Postal or mailing address", IsActive = true, IsDefault = false, SortOrder = 20, NormalizedCode = "POSTAL", NormalizedName = "POSTAL", CreatedAt = now, UpdatedAt = now }
            };

            foreach (var at in addressTypes)
            {
                var exists = await _context.AddressTypes.FirstOrDefaultAsync(x => x.Code == at.Code);
                if (exists == null)
                {
                    _context.AddressTypes.Add(at);
                    addressTypesAdded++;
                    _log.Information("Created address type: {Code}", at.Code);
                }
            }

            await _context.SaveChangesAsync();

            _log.Information("Code tables updated - Roles: {RolesAdded}/{TotalRoles}, Statuses: {StatusesAdded}/{TotalStatuses}, DNS Types: {DnsAdded}/{TotalDns}, Service Types: {ServiceAdded}/{TotalService}",
                rolesAdded, response.TotalRoles, statusesAdded, response.TotalCustomerStatuses, 
                dnsTypesAdded, response.TotalDnsRecordTypes, serviceTypesAdded, response.TotalServiceTypes);

            return response;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error checking and updating code tables");
            response.Success = false;
            response.Message = $"Error updating code tables: {ex.Message}";
            return response;
        }
    }

    public async Task<InitializationResponseDto?> InitializeAsync(InitializationRequestDto request)
    {
        try
        {
            // Check if any users exist
            var userCount = await _context.Users.CountAsync();
            
            if (userCount > 0)
            {
                _log.Warning("Initialization attempted but users already exist in the system");
                return null;
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Password) || 
                string.IsNullOrWhiteSpace(request.Email))
            {
                _log.Warning("Initialization attempted with invalid input");
                return null;
            }

            // Check and update code tables (Roles, CustomerStatuses, DnsRecordTypes, ServiceTypes)
            var codeTablesResult = await CheckAndUpdateCodeTablesAsync();
            if (!codeTablesResult.Success)
            {
                _log.Error("Failed to initialize code tables: {Message}", codeTablesResult.Message);
                return null;
            }

            // Get Admin role (should exist after code tables update)
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == ADMIN);
            if (adminRole == null)
            {
                _log.Error("Admin role not found after code tables update");
                return null;
            }

            // Create the first admin user
            // Note: In production, use proper password hashing (e.g., BCrypt, PBKDF2)
            // Hash password using BCrypt (same algorithm used by AuthService)
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                Email = request.Email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Assign Admin role to the user
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = adminRole.Id
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            _log.Information("Initial admin user created: {Username}", user.Username);

            return new InitializationResponseDto
            {
                Success = true,
                Message = "System initialized successfully with admin user",
                Username = user.Username
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during system initialization");
            return null;
        }
    }
}
