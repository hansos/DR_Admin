using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using ISPAdmin.Utilities;
using ISPAdmin.Workflow.Domain.Workflows;
using ISPAdmin.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using RegisteredDomainEntity = ISPAdmin.Data.Entities.RegisteredDomain;

namespace ISPAdmin.Services;

public class DomainRegistrationService : IRegisteredDomainService
{
    private readonly ApplicationDbContext _context;
    private readonly IDomainRegistrationWorkflow _domainRegistrationWorkflow;
    private readonly DomainRegistrationSettings _domainRegistrationSettings;
    private static readonly Serilog.ILogger _log = Log.ForContext<DomainRegistrationService>();

    public DomainRegistrationService(
        ApplicationDbContext context,
        IDomainRegistrationWorkflow domainRegistrationWorkflow,
        IOptions<DomainRegistrationSettings> domainRegistrationSettings)
    {
        _context = context;
        _domainRegistrationWorkflow = domainRegistrationWorkflow;
        _domainRegistrationSettings = domainRegistrationSettings.Value;
    }

    public async Task<IEnumerable<RegisteredDomainDto>> GetAllDomainsAsync()
    {
        try
        {
            _log.Information("Fetching all domains");
            
            var domains = await _context.RegisteredDomains
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .ToListAsync();

            var domainDtos = domains.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} domains", domains.Count);
            return domainDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all domains");
            throw;
        }
    }

    public async Task<IEnumerable<RegisteredDomainDto>> GetDomainsByCustomerIdAsync(int customerId)
    {
        try
        {
            _log.Information("Fetching domains for customer {CustomerId}", customerId);
            
            var domains = await _context.RegisteredDomains
                .AsNoTracking()
                .Where(d => d.CustomerId == customerId)
                .OrderBy(d => d.Name)
                .ToListAsync();

            var domainDtos = domains.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} domains for customer {CustomerId}", domains.Count, customerId);
            return domainDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching domains for customer {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<IEnumerable<RegisteredDomainDto>> GetDomainsByRegistrarIdAsync(int registrarId)
    {
        try
        {
            _log.Information("Fetching domains for registrar {RegistrarId}", registrarId);
            
            var domains = await _context.RegisteredDomains
                .AsNoTracking()
                .Where(d => d.RegistrarId == registrarId)
                .OrderBy(d => d.Name)
                .ToListAsync();

            var domainDtos = domains.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} domains for registrar {RegistrarId}", domains.Count, registrarId);
            return domainDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching domains for registrar {RegistrarId}", registrarId);
            throw;
        }
    }

    public async Task<IEnumerable<RegisteredDomainDto>> GetDomainsByStatusAsync(string status)
    {
        try
        {
            _log.Information("Fetching domains with status {Status}", status);
            
            var domains = await _context.RegisteredDomains
                .AsNoTracking()
                .Where(d => d.Status == status)
                .OrderBy(d => d.Name)
                .ToListAsync();

            var domainDtos = domains.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} domains with status {Status}", domains.Count, status);
            return domainDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching domains with status {Status}", status);
            throw;
        }
    }

    public async Task<IEnumerable<RegisteredDomainDto>> GetDomainsExpiringInDaysAsync(int days)
    {
        try
        {
            _log.Information("Fetching domains expiring in {Days} days", days);
            
            var targetDate = DateTime.UtcNow.AddDays(days);

            var domains = await _context.RegisteredDomains
                .AsNoTracking()
                .Where(d => d.ExpirationDate <= targetDate && d.ExpirationDate >= DateTime.UtcNow)
                .OrderBy(d => d.ExpirationDate)
                .ToListAsync();

            var domainDtos = domains.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} domains expiring in {Days} days", domains.Count, days);
            return domainDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching domains expiring in {Days} days", days);
            throw;
        }
    }

    public async Task<RegisteredDomainDto?> GetDomainByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching domain with ID: {DomainId}", id);

            var domain = await _context.RegisteredDomains
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (domain == null)
            {
                _log.Warning("Domain with ID {DomainId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched domain with ID: {DomainId}", id);
            return MapToDto(domain);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching domain with ID: {DomainId}", id);
            throw;
        }
    }

    public async Task<RegisteredDomainDto?> GetDomainByNameAsync(string name)
    {
        try
        {
            _log.Information("Fetching domain with name: {DomainName}", name);
            
            var normalizedName = NormalizationHelper.Normalize(name);
            
            var domain = await _context.RegisteredDomains
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.NormalizedName == normalizedName);

            if (domain == null)
            {
                _log.Warning("Domain with name {DomainName} not found", name);
                return null;
            }

            _log.Information("Successfully fetched domain with name: {DomainName}", name);
            return MapToDto(domain);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching domain with name: {DomainName}", name);
            throw;
        }
    }

    public async Task<RegisteredDomainDto> CreateDomainAsync(CreateRegisteredDomainDto createDto)
    {
        try
        {
            _log.Information("Creating new domain: {DomainName}", createDto.Name);

            var normalizedName = createDto.Name.ToLowerInvariant();
            var existingDomain = await _context.RegisteredDomains
                .FirstOrDefaultAsync(d => d.NormalizedName == normalizedName);

            if (existingDomain != null)
            {
                _log.Warning("Domain with name {DomainName} already exists", createDto.Name);
                throw new InvalidOperationException($"Domain with name {createDto.Name} already exists");
            }

            var customerExists = await _context.Customers.AnyAsync(c => c.Id == createDto.CustomerId);
            if (!customerExists)
            {
                _log.Warning("Customer with ID {CustomerId} does not exist", createDto.CustomerId);
                throw new InvalidOperationException($"Customer with ID {createDto.CustomerId} does not exist");
            }

            var serviceExists = await _context.Services.AnyAsync(s => s.Id == createDto.ServiceId);
            if (!serviceExists)
            {
                _log.Warning("Service with ID {ServiceId} does not exist", createDto.ServiceId);
                throw new InvalidOperationException($"Service with ID {createDto.ServiceId} does not exist");
            }

            var registrarExists = await _context.Registrars.AnyAsync(r => r.Id == createDto.ProviderId);
            if (!registrarExists)
            {
                _log.Warning("Registrar with ID {RegistrarId} does not exist", createDto.ProviderId);
                throw new InvalidOperationException($"Registrar with ID {createDto.ProviderId} does not exist");
            }

            var domain = new RegisteredDomainEntity
            {
                CustomerId = createDto.CustomerId,
                ServiceId = createDto.ServiceId,
                Name = createDto.Name,
                NormalizedName = normalizedName,
                RegistrarId = createDto.ProviderId,
                Status = createDto.Status,
                RegistrationDate = createDto.RegistrationDate,
                ExpirationDate = createDto.ExpirationDate,
                AutoRenew = false,
                PrivacyProtection = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.RegisteredDomains.Add(domain);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created domain with ID: {DomainId} and name: {DomainName}", 
                domain.Id, domain.Name);
            return MapToDto(domain);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating domain: {DomainName}", createDto.Name);
            throw;
        }
    }

    public async Task<RegisteredDomainDto?> UpdateDomainAsync(int id, UpdateRegisteredDomainDto updateDto)
    {
        try
        {
            _log.Information("Updating domain with ID: {DomainId}", id);

            var domain = await _context.RegisteredDomains.FindAsync(id);

            if (domain == null)
            {
                _log.Warning("Domain with ID {DomainId} not found for update", id);
                return null;
            }

            var normalizedName = updateDto.Name.ToLowerInvariant();
            var duplicateName = await _context.RegisteredDomains
                .AnyAsync(d => d.NormalizedName == normalizedName && d.Id != id);

            if (duplicateName)
            {
                _log.Warning("Cannot update domain {DomainId}: name {DomainName} already exists", 
                    id, updateDto.Name);
                throw new InvalidOperationException($"Domain with name {updateDto.Name} already exists");
            }

            var customerExists = await _context.Customers.AnyAsync(c => c.Id == updateDto.CustomerId);
            if (!customerExists)
            {
                _log.Warning("Customer with ID {CustomerId} does not exist", updateDto.CustomerId);
                throw new InvalidOperationException($"Customer with ID {updateDto.CustomerId} does not exist");
            }

            var serviceExists = await _context.Services.AnyAsync(s => s.Id == updateDto.ServiceId);
            if (!serviceExists)
            {
                _log.Warning("Service with ID {ServiceId} does not exist", updateDto.ServiceId);
                throw new InvalidOperationException($"Service with ID {updateDto.ServiceId} does not exist");
            }

            var registrarExists = await _context.Registrars.AnyAsync(r => r.Id == updateDto.ProviderId);
            if (!registrarExists)
            {
                _log.Warning("Registrar with ID {RegistrarId} does not exist", updateDto.ProviderId);
                throw new InvalidOperationException($"Registrar with ID {updateDto.ProviderId} does not exist");
            }

            domain.CustomerId = updateDto.CustomerId;
            domain.ServiceId = updateDto.ServiceId;
            domain.Name = updateDto.Name;
            domain.NormalizedName = normalizedName;
            domain.RegistrarId = updateDto.ProviderId;
            domain.Status = updateDto.Status;
            domain.RegistrationDate = updateDto.RegistrationDate;
            domain.ExpirationDate = updateDto.ExpirationDate;
            domain.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated domain with ID: {DomainId}", id);
            return MapToDto(domain);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating domain with ID: {DomainId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteDomainAsync(int id)
    {
        try
        {
            _log.Information("Deleting domain with ID: {DomainId}", id);

            var domain = await _context.RegisteredDomains.FindAsync(id);

            if (domain == null)
            {
                _log.Warning("Domain with ID {DomainId} not found for deletion", id);
                return false;
            }

            var hasDnsRecords = await _context.DnsRecords.AnyAsync(dr => dr.DomainId == id);
            if (hasDnsRecords)
            {
                _log.Warning("Cannot delete domain {DomainId}: has associated DNS records", id);
                throw new InvalidOperationException("Cannot delete domain with associated DNS records");
            }

            var hasDomainContacts = await _context.DomainContacts.AnyAsync(dc => dc.DomainId == id);
            if (hasDomainContacts)
            {
                _log.Warning("Cannot delete domain {DomainId}: has associated domain contacts", id);
                throw new InvalidOperationException("Cannot delete domain with associated domain contacts");
            }

            _context.RegisteredDomains.Remove(domain);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted domain with ID: {DomainId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting domain with ID: {DomainId}", id);
            throw;
        }
    }

    private static RegisteredDomainDto MapToDto(RegisteredDomainEntity domain)
    {
        return new RegisteredDomainDto
        {
            Id = domain.Id,
            CustomerId = domain.CustomerId,
            ServiceId = domain.ServiceId,
            Name = domain.Name,
            ProviderId = domain.RegistrarId,
            Status = domain.Status,
            RegistrationDate = domain.RegistrationDate,
            ExpirationDate = domain.ExpirationDate,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public async Task<DomainRegistrationResponseDto> RegisterDomainAsync(RegisterDomainDto dto, int customerId)
    {
        try
        {
            _log.Information("Customer {CustomerId} initiating domain registration for {DomainName}", 
                customerId, dto.DomainName);

            if (!_domainRegistrationSettings.AllowCustomerRegistration)
            {
                _log.Warning("Customer domain registration is disabled");
                throw new InvalidOperationException("Customer domain registration is currently disabled");
            }

            // Validate customer exists
            var customerExists = await _context.Customers.AnyAsync(c => c.Id == customerId);
            if (!customerExists)
            {
                _log.Warning("Customer with ID {CustomerId} does not exist", customerId);
                throw new InvalidOperationException($"Customer with ID {customerId} does not exist");
            }

            // Get default registrar from database
            var registrar = await _context.Registrars
                .FirstOrDefaultAsync(r => r.IsDefault && r.IsActive);
                
            if (registrar == null)
            {
                _log.Error("No default registrar found or default registrar is inactive");
                throw new InvalidOperationException("Domain registration service is currently unavailable. No default registrar configured.");
            }

            // Extract TLD from domain name
            var tld = ExtractTld(dto.DomainName);
            
            // Get registrar TLD configuration to validate years
            var registrarTld = await _context.RegistrarTlds
                .Include(rt => rt.Tld)
                .FirstOrDefaultAsync(rt => rt.RegistrarId == registrar.Id && 
                                          rt.Tld.Extension.ToLower() == tld.ToLower() &&
                                          rt.IsActive);

            if (registrarTld == null)
            {
                _log.Warning("TLD {Tld} is not available with default registrar {RegistrarId}", tld, registrar.Id);
                throw new InvalidOperationException($"The TLD '.{tld}' is not available for registration");
            }

            // Validate registration years against registrar's limits
            var minYears = registrarTld.MinRegistrationYears ?? 1;
            var maxYears = registrarTld.MaxRegistrationYears ?? 10;
            
            if (dto.Years < minYears || dto.Years > maxYears)
            {
                throw new InvalidOperationException(
                    $"Registration period for .{tld} domains must be between {minYears} and {maxYears} years");
            }

            // Create workflow input
            var workflowInput = new DomainRegistrationWorkflowInput
            {
                CustomerId = customerId,
                DomainName = dto.DomainName.ToLowerInvariant().Trim(),
                RegistrarId = registrar.Id,
                Years = dto.Years,
                AutoRenew = dto.AutoRenew,
                PrivacyProtection = dto.PrivacyProtection,
                OrderType = OrderType.New,
                Notes = dto.Notes
            };

            // Execute workflow
            var workflowResult = await _domainRegistrationWorkflow.ExecuteAsync(workflowInput);

            var response = new DomainRegistrationResponseDto
            {
                Success = workflowResult.IsSuccess,
                Message = workflowResult.IsSuccess ? workflowResult.Message! : workflowResult.ErrorMessage!,
                OrderId = workflowResult.AggregateId,
                CorrelationId = workflowResult.CorrelationId,
                RequiresApproval = _domainRegistrationSettings.RequireApprovalForCustomers,
                ApprovalStatus = _domainRegistrationSettings.RequireApprovalForCustomers ? "Pending" : "Auto-Approved"
            };

            // Get order details if successful
            if (workflowResult.IsSuccess && workflowResult.AggregateId.HasValue)
            {
                var order = await _context.Orders
                    .Include(o => o.Invoices)
                    .FirstOrDefaultAsync(o => o.Id == workflowResult.AggregateId.Value);

                if (order != null)
                {
                    response.OrderNumber = order.OrderNumber;
                    response.InvoiceId = order.Invoices.FirstOrDefault()?.Id;
                    response.TotalAmount = order.RecurringAmount;
                }
            }

            _log.Information("Domain registration {Status} for {DomainName} - Order: {OrderId}", 
                response.Success ? "successful" : "failed", dto.DomainName, response.OrderId);

            return response;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during domain registration for {DomainName}", dto.DomainName);
            throw;
        }
    }

    public async Task<DomainRegistrationResponseDto> RegisterDomainForCustomerAsync(RegisterDomainForCustomerDto dto)
    {
        try
        {
            _log.Information("Admin/Sales initiating domain registration for customer {CustomerId}, domain {DomainName}", 
                dto.CustomerId, dto.DomainName);

            // Validate customer exists
            var customerExists = await _context.Customers.AnyAsync(c => c.Id == dto.CustomerId);
            if (!customerExists)
            {
                _log.Warning("Customer with ID {CustomerId} does not exist", dto.CustomerId);
                throw new InvalidOperationException($"Customer with ID {dto.CustomerId} does not exist");
            }

            // Validate registrar
            var registrar = await _context.Registrars.FindAsync(dto.RegistrarId);
            if (registrar == null || !registrar.IsActive)
            {
                _log.Warning("Registrar with ID {RegistrarId} not found or inactive", dto.RegistrarId);
                throw new InvalidOperationException($"Registrar with ID {dto.RegistrarId} is not available");
            }

            // Extract TLD from domain name
            var tld = ExtractTld(dto.DomainName);
            
            // Get registrar TLD configuration to validate years
            var registrarTld = await _context.RegistrarTlds
                .Include(rt => rt.Tld)
                .FirstOrDefaultAsync(rt => rt.RegistrarId == dto.RegistrarId && 
                                          rt.Tld.Extension.ToLower() == tld.ToLower() &&
                                          rt.IsActive);

            if (registrarTld == null)
            {
                _log.Warning("TLD {Tld} is not available with registrar {RegistrarId}", tld, dto.RegistrarId);
                throw new InvalidOperationException($"The TLD '.{tld}' is not available with the selected registrar");
            }

            // Validate registration years against registrar's limits
            var minYears = registrarTld.MinRegistrationYears ?? 1;
            var maxYears = registrarTld.MaxRegistrationYears ?? 10;
            
            if (dto.Years < minYears || dto.Years > maxYears)
            {
                throw new InvalidOperationException(
                    $"Registration period for .{tld} domains with this registrar must be between {minYears} and {maxYears} years");
            }

            // Create workflow input
            var workflowInput = new DomainRegistrationWorkflowInput
            {
                CustomerId = dto.CustomerId,
                DomainName = dto.DomainName.ToLowerInvariant().Trim(),
                RegistrarId = dto.RegistrarId,
                Years = dto.Years,
                AutoRenew = dto.AutoRenew,
                PrivacyProtection = dto.PrivacyProtection,
                OrderType = OrderType.New,
                Notes = dto.Notes
            };

            // Execute workflow
            var workflowResult = await _domainRegistrationWorkflow.ExecuteAsync(workflowInput);

            var response = new DomainRegistrationResponseDto
            {
                Success = workflowResult.IsSuccess,
                Message = workflowResult.IsSuccess ? workflowResult.Message! : workflowResult.ErrorMessage!,
                OrderId = workflowResult.AggregateId,
                CorrelationId = workflowResult.CorrelationId,
                RequiresApproval = _domainRegistrationSettings.RequireApprovalForSales,
                ApprovalStatus = _domainRegistrationSettings.RequireApprovalForSales ? "Pending" : "Auto-Approved"
            };

            // Get order details if successful
            if (workflowResult.IsSuccess && workflowResult.AggregateId.HasValue)
            {
                var order = await _context.Orders
                    .Include(o => o.Invoices)
                    .FirstOrDefaultAsync(o => o.Id == workflowResult.AggregateId.Value);

                if (order != null)
                {
                    response.OrderNumber = order.OrderNumber;
                    response.InvoiceId = order.Invoices.FirstOrDefault()?.Id;
                    response.TotalAmount = order.RecurringAmount;
                }
            }

            _log.Information("Domain registration {Status} for customer {CustomerId}, domain {DomainName} - Order: {OrderId}", 
                response.Success ? "successful" : "failed", dto.CustomerId, dto.DomainName, response.OrderId);

            return response;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error during domain registration for customer {CustomerId}, domain {DomainName}", 
                dto.CustomerId, dto.DomainName);
            throw;
        }
    }

    public async Task<DomainAvailabilityResponseDto> CheckDomainAvailabilityAsync(string domainName)
    {
        try
        {
            _log.Information("Checking availability for domain: {DomainName}", domainName);

            var normalizedName = NormalizationHelper.Normalize(domainName);

            // Check if domain already exists in our system
            var existingDomain = await _context.RegisteredDomains
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.NormalizedName == normalizedName);

            if (existingDomain != null)
            {
                return new DomainAvailabilityResponseDto
                {
                    DomainName = domainName,
                    IsAvailable = false,
                    Message = "This domain is already registered in our system",
                    IsPremium = false
                };
            }

            // TODO: When registrar is configured, check with external registrar
            // For now, assume available if not in our system
            if (_domainRegistrationSettings.EnableAvailabilityCheck)
            {
                _log.Information("External availability check is enabled but not yet implemented");
            }

            return new DomainAvailabilityResponseDto
            {
                DomainName = domainName,
                IsAvailable = true,
                Message = "Domain appears to be available",
                IsPremium = false
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error checking availability for domain: {DomainName}", domainName);
            throw;
        }
    }

    public async Task<DomainPricingDto?> GetDomainPricingAsync(string tld, int? registrarId = null)
    {
        try
        {
            _log.Information("Getting pricing for TLD: {Tld}, Registrar: {RegistrarId}", tld, registrarId);

            // Normalize TLD (remove leading dot if present)
            var normalizedTld = tld.TrimStart('.').ToLowerInvariant();

            // Build query
            var query = _context.RegistrarTlds
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .Where(rt => rt.Tld.Extension.ToLower() == normalizedTld && rt.IsActive && rt.Tld.IsActive);

            // Filter by registrar if specified
            if (registrarId.HasValue)
            {
                query = query.Where(rt => rt.RegistrarId == registrarId.Value);
            }
            else
            {
                // Use default registrar from database
                query = query.Where(rt => rt.Registrar.IsDefault);
            }

            var registrarTld = await query.FirstOrDefaultAsync();

            if (registrarTld == null)
            {
                _log.Warning("No pricing found for TLD: {Tld}", tld);
                return null;
            }

            // NOTE: Pricing has been moved to temporal pricing tables
            // This method should be updated to use ITldPricingService.CalculatePricingAsync()
            // For now, return a basic structure with placeholder values
            var pricingDto = new DomainPricingDto
            {
                Tld = registrarTld.Tld.Extension,
                RegistrarId = registrarTld.RegistrarId,
                RegistrarName = registrarTld.Registrar.Name,
                RegistrationPrice = 0, // TODO: Get from TldSalesPricing
                RenewalPrice = 0, // TODO: Get from TldSalesPricing
                TransferPrice = 0, // TODO: Get from TldSalesPricing
                Currency = "USD", // TODO: Get from TldSalesPricing
                PriceByYears = new Dictionary<int, decimal>()
            };

            // Calculate multi-year pricing using registrar's max years
            var maxYears = registrarTld.MaxRegistrationYears ?? 10;
            for (int years = 1; years <= maxYears; years++)
            {
                pricingDto.PriceByYears[years] = 0; // TODO: Calculate using TldSalesPricing
            }

            _log.Warning("GetPricingForTld is using placeholder pricing. Update to use ITldPricingService.");
            return pricingDto;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting pricing for TLD: {Tld}", tld);
            throw;
        }
    }

    public async Task<IEnumerable<AvailableTldDto>> GetAvailableTldsAsync()
    {
        try
        {
            _log.Information("Fetching all available TLDs");

            var tlds = await _context.RegistrarTlds
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .Where(rt => rt.IsActive && rt.Registrar.IsActive && rt.Tld.IsActive)
                .OrderBy(rt => rt.Tld.Extension)
                .ToListAsync();

            // NOTE: Pricing has been moved to temporal pricing tables
            // This method should be updated to use ITldPricingService
            var tldDtos = tlds.Select(rt => new AvailableTldDto
            {
                Id = rt.Id,
                Tld = rt.Tld.Extension,
                RegistrarId = rt.RegistrarId,
                RegistrarName = rt.Registrar.Name,
                RegistrationPrice = 0, // TODO: Get from TldSalesPricing
                RenewalPrice = 0, // TODO: Get from TldSalesPricing
                Currency = "USD", // TODO: Get from TldSalesPricing
                IsActive = rt.IsActive
            });

            _log.Warning("GetAvailableTldsAsync is using placeholder pricing. Update to use ITldPricingService.");
            _log.Information("Successfully fetched {Count} available TLDs", tlds.Count);
            return tldDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error fetching available TLDs");
            throw;
        }
    }

    /// <summary>
    /// Extracts the TLD from a domain name
    /// </summary>
    /// <param name="domainName">The full domain name (e.g., "example.com")</param>
    /// <returns>The TLD without the dot (e.g., "com")</returns>
    private static string ExtractTld(string domainName)
    {
        var parts = domainName.Split('.');
        if (parts.Length < 2)
        {
            throw new ArgumentException($"Invalid domain name format: {domainName}");
        }
        
        // Return the last part as the TLD
        return parts[^1].ToLowerInvariant();
    }
}
