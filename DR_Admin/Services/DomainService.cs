using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;
using DomainEntity = ISPAdmin.Data.Entities.Domain;

namespace ISPAdmin.Services;

public class DomainService : IDomainService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<DomainService>();

    public DomainService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DomainDto>> GetAllDomainsAsync()
    {
        try
        {
            _log.Information("Fetching all domains");
            
            var domains = await _context.Domains
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

    public async Task<IEnumerable<DomainDto>> GetDomainsByCustomerIdAsync(int customerId)
    {
        try
        {
            _log.Information("Fetching domains for customer {CustomerId}", customerId);
            
            var domains = await _context.Domains
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

    public async Task<IEnumerable<DomainDto>> GetDomainsByRegistrarIdAsync(int registrarId)
    {
        try
        {
            _log.Information("Fetching domains for registrar {RegistrarId}", registrarId);
            
            var domains = await _context.Domains
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

    public async Task<IEnumerable<DomainDto>> GetDomainsByStatusAsync(string status)
    {
        try
        {
            _log.Information("Fetching domains with status {Status}", status);
            
            var domains = await _context.Domains
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

    public async Task<IEnumerable<DomainDto>> GetDomainsExpiringInDaysAsync(int days)
    {
        try
        {
            _log.Information("Fetching domains expiring in {Days} days", days);
            
            var targetDate = DateTime.UtcNow.AddDays(days);
            
            var domains = await _context.Domains
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

    public async Task<DomainDto?> GetDomainByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching domain with ID: {DomainId}", id);
            
            var domain = await _context.Domains
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

    public async Task<DomainDto?> GetDomainByNameAsync(string name)
    {
        try
        {
            _log.Information("Fetching domain with name: {DomainName}", name);
            
            var normalizedName = name.ToLowerInvariant();
            
            var domain = await _context.Domains
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

    public async Task<DomainDto> CreateDomainAsync(CreateDomainDto createDto)
    {
        try
        {
            _log.Information("Creating new domain: {DomainName}", createDto.Name);

            var normalizedName = createDto.Name.ToLowerInvariant();
            var existingDomain = await _context.Domains
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

            var domain = new DomainEntity
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

            _context.Domains.Add(domain);
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

    public async Task<DomainDto?> UpdateDomainAsync(int id, UpdateDomainDto updateDto)
    {
        try
        {
            _log.Information("Updating domain with ID: {DomainId}", id);

            var domain = await _context.Domains.FindAsync(id);

            if (domain == null)
            {
                _log.Warning("Domain with ID {DomainId} not found for update", id);
                return null;
            }

            var normalizedName = updateDto.Name.ToLowerInvariant();
            var duplicateName = await _context.Domains
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

            var domain = await _context.Domains.FindAsync(id);

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

            _context.Domains.Remove(domain);
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

    private static DomainDto MapToDto(DomainEntity domain)
    {
        return new DomainDto
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
}
