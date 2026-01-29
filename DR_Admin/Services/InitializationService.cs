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


            // Find or create Admin role
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == ADMIN);
            
            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Name = ADMIN,
                    Description = "Administrator with full system access"
                };
                _context.Roles.Add(adminRole);
                await _context.SaveChangesAsync();
                _log.Information("Admin role created");
            }

            // Ensure other standard roles exist
            var otherRoles = new[] { SUPPORT, SALES, FINANCE, CUSTOMER };
            foreach (var roleName in otherRoles)
            {
                var existing = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (existing == null)
                {
                    _context.Roles.Add(new Role
                    {
                        Name = roleName,
                        Description = roleName + " role"
                    });
                    _log.Information("Created role: {Role}", roleName);
                }
            }
            await _context.SaveChangesAsync();

            // Ensure customer statuses exist
            var now = DateTime.UtcNow;
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
                    _log.Information("Created customer status: {Code}", cs.Code);
                }
            }
            await _context.SaveChangesAsync();

            // Ensure common DNS record types exist
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
                    _log.Information("Created DNS record type: {Type}", dt.Type);
                }
            }
            await _context.SaveChangesAsync();

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
