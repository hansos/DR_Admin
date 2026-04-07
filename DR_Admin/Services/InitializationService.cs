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
    private readonly IMyAccountService _myAccountService;
    private static readonly Serilog.ILogger _log = Log.ForContext<InitializationService>();

    public InitializationService(ApplicationDbContext context, IMyAccountService myAccountService)
    {
        _context = context;
        _myAccountService = myAccountService;
    }

    public async Task<bool> IsInitializedAsync()
    {
        return await _context.Users.AnyAsync();
    }

    public async Task<CodeTablesResponseDto> CheckAndUpdateCodeTablesAsync()
    {
        var response = new CodeTablesResponseDto { Success = true, Message = "Code tables checked and updated successfully" };

        try
        {
            var now = DateTime.UtcNow;

            // Ensure all standard roles exist
            var rolesAdded = 0;
            var standardRoles = new[] { ADMIN, SUPPORT, SALES, FINANCE, CUSTOMER, DOMAIN, GEOGRAPHICAL, HOSTING };
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

            // Ensure default payment instruments exist
            var paymentInstruments = new[]
            {
                new PaymentInstrument { Code = "CreditCard", Name = "Credit Card", Description = "Card-based payments", IsActive = true, DisplayOrder = 10, NormalizedCode = "CREDITCARD", NormalizedName = "CREDIT CARD", CreatedAt = now, UpdatedAt = now },
                new PaymentInstrument { Code = "BankAccount", Name = "Bank Account", Description = "Bank transfer and account debit", IsActive = true, DisplayOrder = 20, NormalizedCode = "BANKACCOUNT", NormalizedName = "BANK ACCOUNT", CreatedAt = now, UpdatedAt = now },
                new PaymentInstrument { Code = "PayPal", Name = "PayPal", Description = "PayPal wallet payments", IsActive = true, DisplayOrder = 30, NormalizedCode = "PAYPAL", NormalizedName = "PAYPAL", CreatedAt = now, UpdatedAt = now },
                new PaymentInstrument { Code = "Cash", Name = "Cash", Description = "Cash payments", IsActive = true, DisplayOrder = 40, NormalizedCode = "CASH", NormalizedName = "CASH", CreatedAt = now, UpdatedAt = now },
                new PaymentInstrument { Code = "Other", Name = "Other", Description = "Other payment methods", IsActive = true, DisplayOrder = 50, NormalizedCode = "OTHER", NormalizedName = "OTHER", CreatedAt = now, UpdatedAt = now }
            };

            foreach (var instrument in paymentInstruments)
            {
                var exists = await _context.PaymentInstruments.FirstOrDefaultAsync(x => x.Code == instrument.Code && x.DeletedAt == null);
                if (exists == null)
                {
                    _context.PaymentInstruments.Add(instrument);
                    _log.Information("Created payment instrument: {Code}", instrument.Code);
                }
            }

            await _context.SaveChangesAsync();

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

            // Ensure system settings exist
            var systemSettingsAdded = 0;
            var systemSettings = new[]
            {
                new SystemSetting { Key = "CNR", Value = "1001", Description = "Next customer number", IsSystemKey=true},
                new SystemSetting { Key = "PNR", Value = "1001", Description = "Next reference number", IsSystemKey=true},
                new SystemSetting { Key = "ONR", Value = "1001", Description = "Next order number", IsSystemKey=true},
                new SystemSetting { Key = "CSX", Value = "CUST", Description = "customer number prefix", IsSystemKey=true},
                new SystemSetting { Key = "RSX", Value = "REF", Description = "Reference number prefix", IsSystemKey=true},
                new SystemSetting { Key = "OSX", Value = "ORD", Description = "Order number prefix", IsSystemKey=true},
            };

            foreach (var ss in systemSettings)
            {
                var exists = await _context.SystemSettings.FirstOrDefaultAsync(x => x.Key == ss.Key);
                if (exists == null)
                {
                    _context.SystemSettings.Add(ss);
                    systemSettingsAdded++;
                    _log.Information("Created system setting: {Key}", ss.Key);
                }
            }
            await _context.SaveChangesAsync();

            _log.Information("Code tables updated - Roles: {RolesAdded}/{TotalRoles}, Statuses: {StatusesAdded}/{TotalStatuses}, DNS Types: {DnsAdded}/{TotalDns}, Service Types: {ServiceAdded}/{TotalService}, System Settings: {SettingsAdded}",
                rolesAdded, response.TotalRoles, statusesAdded, response.TotalCustomerStatuses, 
                dnsTypesAdded, response.TotalDnsRecordTypes, serviceTypesAdded, response.TotalServiceTypes, systemSettingsAdded);

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

    /// <summary>
    /// Initializes the reseller panel by creating the first administrator and queuing an email confirmation message.
    /// </summary>
    /// <param name="request">Initialization data for the first administrator account.</param>
    /// <returns>Initialization result when successful; otherwise <c>null</c>.</returns>
    public async Task<InitializationResponseDto?> InitializeAdminAsync(InitializationRequestDto request)
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

            var companyName = request.CompanyName?.Trim();
            var companyEmail = request.CompanyEmail?.Trim();
            var companyPhone = request.CompanyPhone?.Trim();

            if (!string.IsNullOrWhiteSpace(companyName) ||
                !string.IsNullOrWhiteSpace(companyEmail) ||
                !string.IsNullOrWhiteSpace(companyPhone))
            {
                var existingCompany = await _context.MyCompanies
                    .OrderBy(x => x.Id)
                    .FirstOrDefaultAsync();

                if (existingCompany == null)
                {
                    _context.MyCompanies.Add(new MyCompany
                    {
                        Name = !string.IsNullOrWhiteSpace(companyName) ? companyName : request.Username,
                        Email = string.IsNullOrWhiteSpace(companyEmail) ? null : companyEmail,
                        Phone = string.IsNullOrWhiteSpace(companyPhone) ? null : companyPhone,
                    });

                    await _context.SaveChangesAsync();
                }
            }

            var confirmationEmailQueued = await _myAccountService.RequestEmailConfirmationAsync(user.Id, "reseller");
            if (!confirmationEmailQueued)
            {
                _log.Warning("Initial admin user created, but confirmation email could not be queued for user: {UserId}", user.Id);
            }

            _log.Information("Initial admin user created: {Username}", user.Username);

            return new InitializationResponseDto
            {
                Success = true,
                Message = confirmationEmailQueued
                    ? "System initialized successfully with admin user. A confirmation email has been sent."
                    : "System initialized successfully with admin user.",
                Username = user.Username
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during system initialization");
            return null;
        }
    }

    public async Task<UserPanelInitializationResponseDto?> InitializeUserPanelAsync(UserPanelInitializationRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword) ||
                string.IsNullOrWhiteSpace(request.CompanyName) ||
                string.IsNullOrWhiteSpace(request.ContactFirstName) ||
                string.IsNullOrWhiteSpace(request.ContactLastName))
            {
                _log.Warning("User panel initialization attempted with invalid input");
                return null;
            }

            if (request.Password != request.ConfirmPassword)
            {
                _log.Warning("User panel initialization attempted with mismatched passwords");
                return null;
            }

            var customerUserExists = await _context.Users
                .AnyAsync(u => u.UserRoles.Any(ur => ur.Role.Name == CUSTOMER));

            if (customerUserExists)
            {
                _log.Warning("User panel initialization attempted but customer users already exist");
                return null;
            }

            var codeTablesResult = await CheckAndUpdateCodeTablesAsync();
            if (!codeTablesResult.Success)
            {
                _log.Error("Failed to initialize code tables for user panel initialization: {Message}", codeTablesResult.Message);
                return null;
            }

            var registerResult = await _myAccountService.RegisterAsync(new RegisterAccountRequestDto
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword,
                CustomerName = request.CompanyName,
                CustomerEmail = request.Email,
                CustomerPhone = request.CompanyPhone,
                CustomerAddress = string.Empty,
                ContactFirstName = request.ContactFirstName,
                ContactLastName = request.ContactLastName,
                SiteCode = "shop",
                IsSelfRegisteredCustomer = true
            });

            return new UserPanelInitializationResponseDto
            {
                Success = true,
                Message = "User panel initialized successfully with first customer user",
                UserId = registerResult.UserId,
                Username = request.Username,
                Email = registerResult.Email,
                CompanyName = request.CompanyName
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during user panel initialization");
            return null;
        }
    }

    public Task<InitializationResponseDto?> InitializeAsync(InitializationRequestDto request)
    {
        return InitializeAdminAsync(request);
    }
}
