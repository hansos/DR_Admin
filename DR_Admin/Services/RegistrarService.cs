using DomainRegistrationLib.Factories;
using DomainRegistrationLib.Implementations;
using DomainRegistrationLib.Interfaces;
using DomainRegistrationLib.Models;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Result of merging domains to database
/// </summary>
public class DomainMergeResult
{
    public int DomainsProcessed { get; set; }
    public int DomainsCreated { get; set; }
    public int DomainsUpdated { get; set; }
    public int DomainsSkipped { get; set; }
    public int TldsCreated { get; set; }
    public int RegistrarTldsCreated { get; set; }
    public int ContactsCreated { get; set; }
    public int ContactsUpdated { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    
    public bool IsSuccessful => DomainsCreated > 0 || DomainsUpdated > 0;
    
    public string GetSummary()
    {
        if (!IsSuccessful && DomainsProcessed > 0)
        {
            return $"Failed to save any domains. Processed: {DomainsProcessed}, Skipped: {DomainsSkipped}. " +
                   (Errors.Any() ? $"Errors: {string.Join("; ", Errors.Take(3))}" : "");
        }
        
        return $"Processed {DomainsProcessed} domains: {DomainsCreated} created, {DomainsUpdated} updated, {DomainsSkipped} skipped";
    }
}

public class RegistrarService : IRegistrarService
{
    private readonly ApplicationDbContext _context;
    private readonly DomainRegistrarFactory _registrarFactory;
    private readonly ICustomerService _customerService;
    private readonly IServiceTypeService _serviceTypeService;
    private readonly IServiceService _serviceService;
    private readonly IResellerCompanyService _resellerCompanyService;
    private readonly IDomainService _domainService;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarService>();

    public RegistrarService(ApplicationDbContext context, DomainRegistrarFactory registrarFactory, 
        ICustomerService customerService, IServiceTypeService serviceTypeService, IServiceService serviceService, 
        IResellerCompanyService resellerCompanyService, IDomainService domainService)
    {
        _context = context;
        _registrarFactory = registrarFactory;
        _customerService = customerService;
        _serviceTypeService = serviceTypeService;
        _serviceService = serviceService;
        _resellerCompanyService = resellerCompanyService;
        _domainService = domainService;
    }

    public async Task<RegistrarTldDto> AssignTldToRegistrarAsync(int registrarId, TldDto tldDto)
    {
        try
        {
            // Normalize extension
            var extension = tldDto.Extension?.Trim().TrimStart('.') ?? string.Empty;
            _log.Information("Assigning or creating TLD {Extension} and linking to registrar {RegistrarId}", extension, registrarId);

            var registrarExists = await _context.Registrars.AnyAsync(r => r.Id == registrarId);
            if (!registrarExists)
            {
                _log.Warning("Registrar with ID {RegistrarId} does not exist", registrarId);
                throw new InvalidOperationException($"Registrar with ID {registrarId} does not exist");
            }

            if (string.IsNullOrEmpty(extension))
            {
                _log.Warning("TLD extension is empty or invalid");
                throw new InvalidOperationException("TLD extension cannot be empty");
            }

            // Check if TLD exists; if not create it
            var tld = await _context.Tlds.FirstOrDefaultAsync(t => t.Extension == extension);
            if (tld == null)
            {
                _log.Information("TLD {Extension} does not exist, creating new TLD", extension);
                var newTld = new Data.Entities.Tld
                {
                    Extension = extension,
                    Description = tldDto.Description,
                    IsActive = tldDto.IsActive,
                    DefaultRegistrationYears = tldDto.DefaultRegistrationYears,
                    MaxRegistrationYears = tldDto.MaxRegistrationYears,
                    RequiresPrivacy = tldDto.RequiresPrivacy,
                    Notes = tldDto.Notes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Tlds.Add(newTld);
                await _context.SaveChangesAsync();

                tld = newTld;
            }

            // Now create RegistrarTld linking registrarId and tld.Id
            var existing = await _context.RegistrarTlds.FirstOrDefaultAsync(rt => rt.RegistrarId == registrarId && rt.TldId == tld.Id);
            if (existing != null)
            {
                _log.Warning("Registrar TLD for registrar {RegistrarId} and TLD {TldId} already exists", registrarId, tld.Id);
                throw new InvalidOperationException("This registrar-TLD combination already exists");
            }

            var registrarTld = new Data.Entities.RegistrarTld
            {
                RegistrarId = registrarId,
                TldId = tld.Id,
                RegistrationCost = 0,
                RegistrationPrice = 0,
                RenewalCost = 0,
                RenewalPrice = 0,
                TransferCost = 0,
                TransferPrice = 0,
                Currency = "USD",
                IsAvailable = true,
                AutoRenew = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.RegistrarTlds.Add(registrarTld);
            await _context.SaveChangesAsync();

            await _context.Entry(registrarTld).Reference(rt => rt.Registrar).LoadAsync();
            await _context.Entry(registrarTld).Reference(rt => rt.Tld).LoadAsync();

            _log.Information("Successfully created registrar TLD with ID: {RegistrarTldId}", registrarTld.Id);
            return new RegistrarTldDto
            {
                Id = registrarTld.Id,
                RegistrarId = registrarTld.RegistrarId,
                RegistrarName = registrarTld.Registrar?.Name,
                TldId = registrarTld.TldId,
                TldExtension = registrarTld.Tld?.Extension,
                RegistrationCost = registrarTld.RegistrationCost,
                RegistrationPrice = registrarTld.RegistrationPrice,
                RenewalCost = registrarTld.RenewalCost,
                RenewalPrice = registrarTld.RenewalPrice,
                TransferCost = registrarTld.TransferCost,
                TransferPrice = registrarTld.TransferPrice,
                PrivacyCost = registrarTld.PrivacyCost,
                PrivacyPrice = registrarTld.PrivacyPrice,
                Currency = registrarTld.Currency,
                IsAvailable = registrarTld.IsAvailable,
                AutoRenew = registrarTld.AutoRenew,
                MinRegistrationYears = registrarTld.MinRegistrationYears,
                MaxRegistrationYears = registrarTld.MaxRegistrationYears,
                Notes = registrarTld.Notes,
                CreatedAt = registrarTld.CreatedAt,
                UpdatedAt = registrarTld.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while assigning/creating TLD {Extension} for registrar {RegistrarId}", tldDto.Extension, registrarId);
            throw;
        }
    }

    public async Task<RegistrarTldDto> AssignTldToRegistrarAsync(int registrarId, int tldId)
    {
        try
        {
            _log.Information("Assigning TLD {TldId} to registrar {RegistrarId}", tldId, registrarId);

            var registrarExists = await _context.Registrars.AnyAsync(r => r.Id == registrarId);
            if (!registrarExists)
            {
                _log.Warning("Registrar with ID {RegistrarId} does not exist", registrarId);
                throw new InvalidOperationException($"Registrar with ID {registrarId} does not exist");
            }

            var tldExists = await _context.Tlds.AnyAsync(t => t.Id == tldId);
            if (!tldExists)
            {
                _log.Warning("TLD with ID {TldId} does not exist", tldId);
                throw new InvalidOperationException($"TLD with ID {tldId} does not exist");
            }

            var existing = await _context.RegistrarTlds.FirstOrDefaultAsync(rt => rt.RegistrarId == registrarId && rt.TldId == tldId);
            if (existing != null)
            {
                _log.Warning("Registrar TLD for registrar {RegistrarId} and TLD {TldId} already exists", registrarId, tldId);
                throw new InvalidOperationException("This registrar-TLD combination already exists");
            }

            var registrarTld = new Data.Entities.RegistrarTld
            {
                RegistrarId = registrarId,
                TldId = tldId,
                RegistrationCost = 0,
                RegistrationPrice = 0,
                RenewalCost = 0,
                RenewalPrice = 0,
                TransferCost = 0,
                TransferPrice = 0,
                Currency = "USD",
                IsAvailable = true,
                AutoRenew = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.RegistrarTlds.Add(registrarTld);
            await _context.SaveChangesAsync();

            await _context.Entry(registrarTld).Reference(rt => rt.Registrar).LoadAsync();
            await _context.Entry(registrarTld).Reference(rt => rt.Tld).LoadAsync();

            _log.Information("Successfully assigned TLD {TldId} to registrar {RegistrarId}", tldId, registrarId);
            return new RegistrarTldDto
            {
                Id = registrarTld.Id,
                RegistrarId = registrarTld.RegistrarId,
                RegistrarName = registrarTld.Registrar?.Name,
                TldId = registrarTld.TldId,
                TldExtension = registrarTld.Tld?.Extension,
                RegistrationCost = registrarTld.RegistrationCost,
                RegistrationPrice = registrarTld.RegistrationPrice,
                RenewalCost = registrarTld.RenewalCost,
                RenewalPrice = registrarTld.RenewalPrice,
                TransferCost = registrarTld.TransferCost,
                TransferPrice = registrarTld.TransferPrice,
                PrivacyCost = registrarTld.PrivacyCost,
                PrivacyPrice = registrarTld.PrivacyPrice,
                Currency = registrarTld.Currency,
                IsAvailable = registrarTld.IsAvailable,
                AutoRenew = registrarTld.AutoRenew,
                MinRegistrationYears = registrarTld.MinRegistrationYears,
                MaxRegistrationYears = registrarTld.MaxRegistrationYears,
                Notes = registrarTld.Notes,
                CreatedAt = registrarTld.CreatedAt,
                UpdatedAt = registrarTld.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while assigning TLD {TldId} to registrar {RegistrarId}", tldId, registrarId);
            throw;
        }
    }

    public async Task<RegistrarTldDto> AssignTldToRegistrarAsync(CreateRegistrarTldDto createDto)
    {
        try
        {
            _log.Information("Assigning TLD {TldId} to registrar {RegistrarId} via DTO", createDto.TldId, createDto.RegistrarId);

            var registrarExists = await _context.Registrars.AnyAsync(r => r.Id == createDto.RegistrarId);
            if (!registrarExists)
            {
                _log.Warning("Registrar with ID {RegistrarId} does not exist", createDto.RegistrarId);
                throw new InvalidOperationException($"Registrar with ID {createDto.RegistrarId} does not exist");
            }

            var tldExists = await _context.Tlds.AnyAsync(t => t.Id == createDto.TldId);
            if (!tldExists)
            {
                _log.Warning("TLD with ID {TldId} does not exist", createDto.TldId);
                throw new InvalidOperationException($"TLD with ID {createDto.TldId} does not exist");
            }

            var existing = await _context.RegistrarTlds.FirstOrDefaultAsync(rt => rt.RegistrarId == createDto.RegistrarId && rt.TldId == createDto.TldId);
            if (existing != null)
            {
                _log.Warning("Registrar TLD for registrar {RegistrarId} and TLD {TldId} already exists", createDto.RegistrarId, createDto.TldId);
                throw new InvalidOperationException("This registrar-TLD combination already exists");
            }

            var registrarTld = new Data.Entities.RegistrarTld
            {
                RegistrarId = createDto.RegistrarId,
                TldId = createDto.TldId,
                RegistrationCost = createDto.RegistrationCost,
                RegistrationPrice = createDto.RegistrationPrice,
                RenewalCost = createDto.RenewalCost,
                RenewalPrice = createDto.RenewalPrice,
                TransferCost = createDto.TransferCost,
                TransferPrice = createDto.TransferPrice,
                PrivacyCost = createDto.PrivacyCost,
                PrivacyPrice = createDto.PrivacyPrice,
                Currency = createDto.Currency,
                IsAvailable = createDto.IsAvailable,
                AutoRenew = createDto.AutoRenew,
                MinRegistrationYears = createDto.MinRegistrationYears,
                MaxRegistrationYears = createDto.MaxRegistrationYears,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.RegistrarTlds.Add(registrarTld);
            await _context.SaveChangesAsync();

            await _context.Entry(registrarTld).Reference(rt => rt.Registrar).LoadAsync();
            await _context.Entry(registrarTld).Reference(rt => rt.Tld).LoadAsync();

            _log.Information("Successfully created registrar TLD with ID: {RegistrarTldId}", registrarTld.Id);
            return new RegistrarTldDto
            {
                Id = registrarTld.Id,
                RegistrarId = registrarTld.RegistrarId,
                RegistrarName = registrarTld.Registrar?.Name,
                TldId = registrarTld.TldId,
                TldExtension = registrarTld.Tld?.Extension,
                RegistrationCost = registrarTld.RegistrationCost,
                RegistrationPrice = registrarTld.RegistrationPrice,
                RenewalCost = registrarTld.RenewalCost,
                RenewalPrice = registrarTld.RenewalPrice,
                TransferCost = registrarTld.TransferCost,
                TransferPrice = registrarTld.TransferPrice,
                PrivacyCost = registrarTld.PrivacyCost,
                PrivacyPrice = registrarTld.PrivacyPrice,
                Currency = registrarTld.Currency,
                IsAvailable = registrarTld.IsAvailable,
                AutoRenew = registrarTld.AutoRenew,
                MinRegistrationYears = registrarTld.MinRegistrationYears,
                MaxRegistrationYears = registrarTld.MaxRegistrationYears,
                Notes = registrarTld.Notes,
                CreatedAt = registrarTld.CreatedAt,
                UpdatedAt = registrarTld.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while assigning TLD {TldId} to registrar {RegistrarId}", createDto.TldId, createDto.RegistrarId);
            throw;
        }
    }

    public async Task<IEnumerable<RegistrarDto>> GetAllRegistrarsAsync()
    {
        try
        {
            _log.Information("Fetching all registrars");
            
            var registrars = await _context.Registrars
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .ToListAsync();

            var registrarDtos = registrars.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} registrars", registrars.Count);
            return registrarDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all registrars");
            throw;
        }
    }

    public async Task<IEnumerable<RegistrarDto>> GetActiveRegistrarsAsync()
    {
        try
        {
            _log.Information("Fetching active registrars");
            
            var registrars = await _context.Registrars
                .AsNoTracking()
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();

            var registrarDtos = registrars.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} active registrars", registrars.Count);
            return registrarDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active registrars");
            throw;
        }
    }

    public async Task<RegistrarDto?> GetRegistrarByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching registrar with ID: {RegistrarId}", id);
            
            var registrar = await _context.Registrars
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (registrar == null)
            {
                _log.Warning("Registrar with ID {RegistrarId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched registrar with ID: {RegistrarId}", id);
            return MapToDto(registrar);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching registrar with ID: {RegistrarId}", id);
            throw;
        }
    }

    public async Task<RegistrarDto?> GetRegistrarByCodeAsync(string code)
    {
        try
        {
            _log.Information("Fetching registrar with code: {RegistrarCode}", code);
            
            var registrar = await _context.Registrars
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Code == code);

            if (registrar == null)
            {
                _log.Warning("Registrar with code {RegistrarCode} not found", code);
                return null;
            }

            _log.Information("Successfully fetched registrar with code: {RegistrarCode}", code);
            return MapToDto(registrar);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching registrar with code: {RegistrarCode}", code);
            throw;
        }
    }

    public async Task<RegistrarDto> CreateRegistrarAsync(CreateRegistrarDto createDto)
    {
        try
        {
            _log.Information("Creating new registrar with code: {RegistrarCode}", createDto.Code);

            var existingRegistrar = await _context.Registrars
                .FirstOrDefaultAsync(r => r.Code == createDto.Code);

            if (existingRegistrar != null)
            {
                _log.Warning("Registrar with code {RegistrarCode} already exists", createDto.Code);
                throw new InvalidOperationException($"Registrar with code {createDto.Code} already exists");
            }

            var registrar = new Registrar
            {
                Name = createDto.Name,
                Code = createDto.Code.ToUpper(),
                IsActive = createDto.IsActive,
                ContactEmail = createDto.ContactEmail,
                ContactPhone = createDto.ContactPhone,
                Website = createDto.Website,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Registrars.Add(registrar);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created registrar with ID: {RegistrarId} and code: {RegistrarCode}", 
                registrar.Id, registrar.Code);
            return MapToDto(registrar);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating registrar with code: {RegistrarCode}", createDto.Code);
            throw;
        }
    }

    public async Task<RegistrarDto?> UpdateRegistrarAsync(int id, UpdateRegistrarDto updateDto)
    {
        try
        {
            _log.Information("Updating registrar with ID: {RegistrarId}", id);

            var registrar = await _context.Registrars.FindAsync(id);

            if (registrar == null)
            {
                _log.Warning("Registrar with ID {RegistrarId} not found for update", id);
                return null;
            }

            var duplicateCode = await _context.Registrars
                .AnyAsync(r => r.Code == updateDto.Code && r.Id != id);

            if (duplicateCode)
            {
                _log.Warning("Cannot update registrar {RegistrarId}: code {RegistrarCode} already exists", 
                    id, updateDto.Code);
                throw new InvalidOperationException($"Registrar with code {updateDto.Code} already exists");
            }

            registrar.Name = updateDto.Name;
            registrar.Code = updateDto.Code.ToUpper();
            registrar.IsActive = updateDto.IsActive;
            registrar.ContactEmail = updateDto.ContactEmail;
            registrar.ContactPhone = updateDto.ContactPhone;
            registrar.Website = updateDto.Website;
            registrar.Notes = updateDto.Notes;
            registrar.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated registrar with ID: {RegistrarId}", id);
            return MapToDto(registrar);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating registrar with ID: {RegistrarId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteRegistrarAsync(int id)
    {
        try
        {
            _log.Information("Deleting registrar with ID: {RegistrarId}", id);

            var registrar = await _context.Registrars.FindAsync(id);

            if (registrar == null)
            {
                _log.Warning("Registrar with ID {RegistrarId} not found for deletion", id);
                return false;
            }

            var hasDomains = await _context.Domains.AnyAsync(d => d.RegistrarId == id);
            if (hasDomains)
            {
                _log.Warning("Cannot delete registrar {RegistrarId}: has associated domains", id);
                throw new InvalidOperationException("Cannot delete registrar with associated domains");
            }

            var hasRegistrarTlds = await _context.RegistrarTlds.AnyAsync(rt => rt.RegistrarId == id);
            if (hasRegistrarTlds)
            {
                _log.Warning("Cannot delete registrar {RegistrarId}: has associated TLD offerings", id);
                throw new InvalidOperationException("Cannot delete registrar with associated TLD offerings");
            }

            _context.Registrars.Remove(registrar);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted registrar with ID: {RegistrarId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting registrar with ID: {RegistrarId}", id);
            throw;
        }
    }

    private static RegistrarDto MapToDto(Registrar registrar)
    {
        return new RegistrarDto
        {
            Id = registrar.Id,
            Name = registrar.Name,
            Code = registrar.Code,
            IsActive = registrar.IsActive,
            ContactEmail = registrar.ContactEmail,
            ContactPhone = registrar.ContactPhone,
            Website = registrar.Website,
            Notes = registrar.Notes,
            CreatedAt = registrar.CreatedAt,
            UpdatedAt = registrar.UpdatedAt
        };
    }

    public async Task<IEnumerable<TldDto>> GetTldsByRegistrarAsync(int registrarId)
    {
        try
        {
            _log.Information("Fetching TLDs for registrar {RegistrarId}", registrarId);

            var tlds = await _context.RegistrarTlds
                .AsNoTracking()
                .Where(rt => rt.RegistrarId == registrarId)
                .Include(rt => rt.Tld)
                .Select(rt => rt.Tld)
                .OrderBy(t => t.Extension)
                .ToListAsync();

            var tldDtos = tlds.Select(t => new TldDto
            {
                Id = t.Id,
                Extension = t.Extension,
                Description = t.Description,
                IsActive = t.IsActive,
                DefaultRegistrationYears = t.DefaultRegistrationYears,
                MaxRegistrationYears = t.MaxRegistrationYears,
                RequiresPrivacy = t.RequiresPrivacy,
                Notes = t.Notes,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            });

            _log.Information("Successfully fetched {Count} TLDs for registrar {RegistrarId}", tlds.Count, registrarId);
            return tldDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching TLDs for registrar {RegistrarId}", registrarId);
            throw;
        }
    }

    public async Task<int> DownloadTldsForRegistrarAsync(int registrarId, string tld)
    {
        try
        {
            _log.Information("Downloading TLDs for registrar {RegistrarId} (filter: {Tld})", registrarId, tld);

            // Get registrar details
            var registrar = await _context.Registrars
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == registrarId);

            if (registrar == null)
            {
                _log.Warning("Registrar with ID {RegistrarId} not found", registrarId);
                throw new InvalidOperationException($"Registrar with ID {registrarId} not found");
            }

            // Create registrar client instance
            var registrarClient = _registrarFactory.CreateRegistrar(registrar.Code);

            // Download supported TLDs from the registrar API (filtered)
            List<TldInfo> supportedTlds = await registrarClient.GetSupportedTldsAsync(tld);
            _log.Information("Retrieved {Count} TLDs from registrar {RegistrarCode} (filter: {Tld})", supportedTlds.Count, registrar.Code, tld);

            int updatedCount = 0;

            foreach (var tldInfo in supportedTlds)
            {
                var extension = tldInfo.Name.TrimStart('.');

                var tldEntity = await _context.Tlds.FirstOrDefaultAsync(t => t.Extension == extension);
                if (tldEntity == null)
                {
                    _log.Information("Creating new TLD: {Extension}", extension);
                    tldEntity = new Tld
                    {
                        Extension = extension,
                        Description = $"{extension.ToUpper()} domain",
                        IsActive = true,
                        DefaultRegistrationYears = 1,
                        MaxRegistrationYears = tldInfo.MaxRegistrationYears ?? 10,
                        RequiresPrivacy = tldInfo.SupportsPrivacy ?? false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Tlds.Add(tldEntity);
                    await _context.SaveChangesAsync();
                }

                var registrarTld = await _context.RegistrarTlds
                    .FirstOrDefaultAsync(rt => rt.RegistrarId == registrarId && rt.TldId == tldEntity.Id);

                if (registrarTld == null)
                {
                    _log.Information("Creating new RegistrarTld for registrar {RegistrarId} and TLD {TldExtension}", 
                        registrarId, extension);

                    registrarTld = new RegistrarTld
                    {
                        RegistrarId = registrarId,
                        TldId = tldEntity.Id,
                        RegistrationCost = tldInfo.RegistrationPrice ?? 0,
                        RegistrationPrice = tldInfo.RegistrationPrice ?? 0,
                        RenewalCost = tldInfo.RenewalPrice ?? 0,
                        RenewalPrice = tldInfo.RenewalPrice ?? 0,
                        TransferCost = tldInfo.TransferPrice ?? 0,
                        TransferPrice = tldInfo.TransferPrice ?? 0,
                        Currency = tldInfo.Currency ?? "USD",
                        IsAvailable = true,
                        AutoRenew = false,
                        MinRegistrationYears = tldInfo.MinRegistrationYears,
                        MaxRegistrationYears = tldInfo.MaxRegistrationYears,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.RegistrarTlds.Add(registrarTld);
                }
                else
                {
                    _log.Information("Updating existing RegistrarTld for registrar {RegistrarId} and TLD {TldExtension}", 
                        registrarId, extension);

                    registrarTld.RegistrationCost = tldInfo.RegistrationPrice ?? registrarTld.RegistrationCost;
                    registrarTld.RegistrationPrice = tldInfo.RegistrationPrice ?? registrarTld.RegistrationPrice;
                    registrarTld.RenewalCost = tldInfo.RenewalPrice ?? registrarTld.RenewalCost;
                    registrarTld.RenewalPrice = tldInfo.RenewalPrice ?? registrarTld.RenewalPrice;
                    registrarTld.TransferCost = tldInfo.TransferPrice ?? registrarTld.TransferCost;
                    registrarTld.TransferPrice = tldInfo.TransferPrice ?? registrarTld.TransferPrice;
                    registrarTld.Currency = tldInfo.Currency ?? registrarTld.Currency;
                    registrarTld.MinRegistrationYears = tldInfo.MinRegistrationYears ?? registrarTld.MinRegistrationYears;
                    registrarTld.MaxRegistrationYears = tldInfo.MaxRegistrationYears ?? registrarTld.MaxRegistrationYears;
                    registrarTld.IsAvailable = true;
                    registrarTld.UpdatedAt = DateTime.UtcNow;
                }

                updatedCount++;
            }

            await _context.SaveChangesAsync();
            _log.Information("Successfully downloaded and updated {Count} TLDs for registrar {RegistrarId}", 
                updatedCount, registrarId);

            return updatedCount;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while downloading TLDs for registrar {RegistrarId} (filter: {Tld})", registrarId, tld);
            throw;
        }
    }

    public async Task<int> DownloadTldsForRegistrarAsync(int registrarId, List<string> tlds)
    {
        try
        {
            _log.Information("Downloading TLDs for registrar {RegistrarId} (filter list count: {Count})", registrarId, tlds?.Count ?? 0);

            if(tlds == null)
            {
                throw new ArgumentNullException("TldList cannot be null.");
            }

            if (tlds.Count == 0)
            {
                throw new ArgumentNullException("TldList cannot be empty.");
            }

            // Get registrar details
            var registrar = await _context.Registrars
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == registrarId);

            if (registrar == null)
            {
                _log.Warning("Registrar with ID {RegistrarId} not found", registrarId);
                throw new InvalidOperationException($"Registrar with ID {registrarId} not found");
            }

            // Create registrar client instance
            var registrarClient = _registrarFactory.CreateRegistrar(registrar.Code);

            // Download supported TLDs from the registrar API (filtered list)
            List<TldInfo> supportedTlds = await registrarClient.GetSupportedTldsAsync(tlds);
            _log.Information("Retrieved {Count} TLDs from registrar {RegistrarCode}", supportedTlds.Count, registrar.Code);

            int updatedCount = 0;

            foreach (var tldInfo in supportedTlds)
            {
                var extension = tldInfo.Name.TrimStart('.');

                var tldEntity = await _context.Tlds.FirstOrDefaultAsync(t => t.Extension == extension);
                if (tldEntity == null)
                {
                    _log.Information("Creating new TLD: {Extension}", extension);
                    tldEntity = new Tld
                    {
                        Extension = extension,
                        Description = $"{extension.ToUpper()} domain",
                        IsActive = true,
                        DefaultRegistrationYears = 1,
                        MaxRegistrationYears = tldInfo.MaxRegistrationYears ?? 10,
                        RequiresPrivacy = tldInfo.SupportsPrivacy ?? false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Tlds.Add(tldEntity);
                    await _context.SaveChangesAsync();
                }

                var registrarTld = await _context.RegistrarTlds
                    .FirstOrDefaultAsync(rt => rt.RegistrarId == registrarId && rt.TldId == tldEntity.Id);

                if (registrarTld == null)
                {
                    _log.Information("Creating new RegistrarTld for registrar {RegistrarId} and TLD {TldExtension}", 
                        registrarId, extension);

                    registrarTld = new RegistrarTld
                    {
                        RegistrarId = registrarId,
                        TldId = tldEntity.Id,
                        RegistrationCost = tldInfo.RegistrationPrice ?? 0,
                        RegistrationPrice = tldInfo.RegistrationPrice ?? 0,
                        RenewalCost = tldInfo.RenewalPrice ?? 0,
                        RenewalPrice = tldInfo.RenewalPrice ?? 0,
                        TransferCost = tldInfo.TransferPrice ?? 0,
                        TransferPrice = tldInfo.TransferPrice ?? 0,
                        Currency = tldInfo.Currency ?? "USD",
                        IsAvailable = true,
                        AutoRenew = false,
                        MinRegistrationYears = tldInfo.MinRegistrationYears,
                        MaxRegistrationYears = tldInfo.MaxRegistrationYears,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.RegistrarTlds.Add(registrarTld);
                }
                else
                {
                    _log.Information("Updating existing RegistrarTld for registrar {RegistrarId} and TLD {TldExtension}", 
                        registrarId, extension);

                    registrarTld.RegistrationCost = tldInfo.RegistrationPrice ?? registrarTld.RegistrationCost;
                    registrarTld.RegistrationPrice = tldInfo.RegistrationPrice ?? registrarTld.RegistrationPrice;
                    registrarTld.RenewalCost = tldInfo.RenewalPrice ?? registrarTld.RenewalCost;
                    registrarTld.RenewalPrice = tldInfo.RenewalPrice ?? registrarTld.RenewalPrice;
                    registrarTld.TransferCost = tldInfo.TransferPrice ?? registrarTld.TransferCost;
                    registrarTld.TransferPrice = tldInfo.TransferPrice ?? registrarTld.TransferPrice;
                    registrarTld.Currency = tldInfo.Currency ?? registrarTld.Currency;
                    registrarTld.MinRegistrationYears = tldInfo.MinRegistrationYears ?? registrarTld.MinRegistrationYears;
                    registrarTld.MaxRegistrationYears = tldInfo.MaxRegistrationYears ?? registrarTld.MaxRegistrationYears;
                    registrarTld.IsAvailable = true;
                    registrarTld.UpdatedAt = DateTime.UtcNow;
                }

                updatedCount++;
            }

            await _context.SaveChangesAsync();
            _log.Information("Successfully downloaded and updated {Count} TLDs for registrar {RegistrarId}", 
                updatedCount, registrarId);

            return updatedCount;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while downloading TLDs for registrar {RegistrarId} (filter list)", registrarId);
            throw;
        }
    }

    public async Task<int> DownloadTldsForRegistrarAsync(int registrarId)
    {
        try
        {
            _log.Information("Downloading TLDs for registrar {RegistrarId}", registrarId);

            // Get registrar details
            var registrar = await _context.Registrars
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == registrarId);

            if (registrar == null)
            {
                _log.Warning("Registrar with ID {RegistrarId} not found", registrarId);
                throw new InvalidOperationException($"Registrar with ID {registrarId} not found");
            }

            // Create registrar client instance
            var registrarClient = _registrarFactory.CreateRegistrar(registrar.Code);

            // Download supported TLDs from the registrar API
            List<TldInfo> supportedTlds = await registrarClient.GetSupportedTldsAsync();
            _log.Information("Retrieved {Count} TLDs from registrar {RegistrarCode}", supportedTlds.Count, registrar.Code);

            int updatedCount = 0;

            foreach (var tldInfo in supportedTlds)
            {
                // Normalize TLD extension (remove leading dot if present)
                var extension = tldInfo.Name.TrimStart('.');

                // Find or create TLD in database
                var tld = await _context.Tlds.FirstOrDefaultAsync(t => t.Extension == extension);
                if (tld == null)
                {
                    _log.Information("Creating new TLD: {Extension}", extension);
                    tld = new Tld
                    {
                        Extension = extension,
                        Description = $"{extension.ToUpper()} domain",
                        IsActive = true,
                        DefaultRegistrationYears = 1,
                        MaxRegistrationYears = tldInfo.MaxRegistrationYears ?? 10,
                        RequiresPrivacy = tldInfo.SupportsPrivacy ?? false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Tlds.Add(tld);
                    await _context.SaveChangesAsync();
                }

                // Find or create RegistrarTld relationship
                var registrarTld = await _context.RegistrarTlds
                    .FirstOrDefaultAsync(rt => rt.RegistrarId == registrarId && rt.TldId == tld.Id);

                if (registrarTld == null)
                {
                    _log.Information("Creating new RegistrarTld for registrar {RegistrarId} and TLD {TldExtension}", 
                        registrarId, extension);
                    
                    registrarTld = new RegistrarTld
                    {
                        RegistrarId = registrarId,
                        TldId = tld.Id,
                        RegistrationCost = tldInfo.RegistrationPrice ?? 0,
                        RegistrationPrice = tldInfo.RegistrationPrice ?? 0,
                        RenewalCost = tldInfo.RenewalPrice ?? 0,
                        RenewalPrice = tldInfo.RenewalPrice ?? 0,
                        TransferCost = tldInfo.TransferPrice ?? 0,
                        TransferPrice = tldInfo.TransferPrice ?? 0,
                        Currency = tldInfo.Currency ?? "USD",
                        IsAvailable = true,
                        AutoRenew = false,
                        MinRegistrationYears = tldInfo.MinRegistrationYears,
                        MaxRegistrationYears = tldInfo.MaxRegistrationYears,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.RegistrarTlds.Add(registrarTld);
                }
                else
                {
                    _log.Information("Updating existing RegistrarTld for registrar {RegistrarId} and TLD {TldExtension}", 
                        registrarId, extension);
                    
                    // Update pricing information
                    registrarTld.RegistrationCost = tldInfo.RegistrationPrice ?? registrarTld.RegistrationCost;
                    registrarTld.RegistrationPrice = tldInfo.RegistrationPrice ?? registrarTld.RegistrationPrice;
                    registrarTld.RenewalCost = tldInfo.RenewalPrice ?? registrarTld.RenewalCost;
                    registrarTld.RenewalPrice = tldInfo.RenewalPrice ?? registrarTld.RenewalPrice;
                    registrarTld.TransferCost = tldInfo.TransferPrice ?? registrarTld.TransferCost;
                    registrarTld.TransferPrice = tldInfo.TransferPrice ?? registrarTld.TransferPrice;
                    registrarTld.Currency = tldInfo.Currency ?? registrarTld.Currency;
                    registrarTld.MinRegistrationYears = tldInfo.MinRegistrationYears ?? registrarTld.MinRegistrationYears;
                    registrarTld.MaxRegistrationYears = tldInfo.MaxRegistrationYears ?? registrarTld.MaxRegistrationYears;
                    registrarTld.IsAvailable = true;
                    registrarTld.UpdatedAt = DateTime.UtcNow;
                }

                updatedCount++;
            }

            await _context.SaveChangesAsync();
            _log.Information("Successfully downloaded and updated {Count} TLDs for registrar {RegistrarId}", 
                updatedCount, registrarId);

            return updatedCount;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while downloading TLDs for registrar {RegistrarId}", registrarId);
            throw;
        }
    }

    public async Task<DomainAvailabilityResult> CheckDomainAvailabilityAsync(int registrarId, string domainName)
    {
        try
        {
            _log.Information("Checking domain availability for {DomainName} using registrar {RegistrarId}", domainName, registrarId);

            // Get registrar details
            var registrar = await _context.Registrars
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == registrarId);

            if (registrar == null)
            {
                _log.Warning("Registrar with ID {RegistrarId} not found", registrarId);
                throw new InvalidOperationException($"Registrar with ID {registrarId} not found");
            }

            if (!registrar.IsActive)
            {
                _log.Warning("Registrar with ID {RegistrarId} is not active", registrarId);
                throw new InvalidOperationException($"Registrar with ID {registrarId} is not active");
            }

            // Create registrar client instance
            var registrarClient = _registrarFactory.CreateRegistrar(registrar.Code);

            // Check domain availability
            var result = await registrarClient.CheckAvailabilityAsync(domainName);
            
            _log.Information("Domain availability check completed for {DomainName}: Available={IsAvailable}", 
                domainName, result.IsAvailable);

            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while checking domain availability for {DomainName} using registrar {RegistrarId}", 
                domainName, registrarId);
            throw;
        }
    }

    public async Task<int> DownloadDomainsForRegistrarAsync(int registrarId)
    {
        try
        {
            _log.Information("Downloading domains for registrar {RegistrarId}", registrarId);

            // Get registered domains from the registrar (with save option)
            var result = await GetRegisteredDomainsAsync(registrarId);
            
            if (!result.Success)
            {
                _log.Warning("Failed to retrieve domains from registrar {RegistrarId}: {Message}", 
                    registrarId, result.Message);
                throw new InvalidOperationException($"Failed to retrieve domains: {result.Message}");
            }

            // If domains exist, merge to database
            if (result.Domains != null && result.Domains.Any())
            {
                var mergeResult = await MergeRegisteredDomainsToDatabase(registrarId, result.Domains);
                
                // Check if merge was actually successful
                if (!mergeResult.IsSuccessful)
                {
                    var errorMessage = $"Failed to save any domains to database. {mergeResult.GetSummary()}";
                    
                    if (mergeResult.Errors.Any())
                    {
                        errorMessage += $" Errors: {string.Join("; ", mergeResult.Errors.Take(5))}";
                    }
                    
                    if (mergeResult.Warnings.Any())
                    {
                        errorMessage += $" Warnings: {string.Join("; ", mergeResult.Warnings.Take(10))}";
                    }
                    
                    _log.Error("Domain merge failed for registrar {RegistrarId}: {ErrorMessage}", registrarId, errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }
                
                // Log warnings if any domains were skipped
                if (mergeResult.DomainsSkipped > 0)
                {
                    _log.Warning("Some domains were skipped during merge for registrar {RegistrarId}: {Summary}", 
                        registrarId, mergeResult.GetSummary());
                    
                    if (mergeResult.Warnings.Any())
                    {
                        _log.Warning("Skip reasons: {Warnings}", string.Join("; ", mergeResult.Warnings.Take(10)));
                    }
                }
                
                _log.Information("Domain merge completed for registrar {RegistrarId}: {Summary}", 
                    registrarId, mergeResult.GetSummary());
                
                // Return only successfully saved domains (created + updated)
                return mergeResult.DomainsCreated + mergeResult.DomainsUpdated;
            }

            // No domains to process
            return 0;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while downloading domains for registrar {RegistrarId}", registrarId);
            throw;
        }
    }

    public async Task<RegisteredDomainsResult> GetRegisteredDomainsAsync(int registrarId)
    {
        try
        {
            _log.Information("Getting registered domains for registrar {RegistrarId}", registrarId);

            // Get registrar details
            var registrar = await _context.Registrars
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == registrarId);

            if (registrar == null)
            {
                _log.Warning("Registrar with ID {RegistrarId} not found", registrarId);
                throw new InvalidOperationException($"Registrar with ID {registrarId} not found");
            }

            if (!registrar.IsActive)
            {
                _log.Warning("Registrar with ID {RegistrarId} is not active", registrarId);
                throw new InvalidOperationException($"Registrar with ID {registrarId} is not active");
            }

            // Create registrar client instance
            var registrarClient = _registrarFactory.CreateRegistrar(registrar.Code);

            // Get registered domains from the registrar API
            var result = await registrarClient.GetRegisteredDomainsAsync();
            
            _log.Information("Retrieved {Count} domains from registrar {RegistrarCode}: Success={Success}", 
                result.Domains?.Count ?? 0, registrar.Code, result.Success);

            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while getting registered domains for registrar {RegistrarId}", registrarId);
            throw;
        }
    }

    /// <summary>
    /// Helper method to merge registered domains, their TLDs, and contact persons to the database
    /// </summary>
    private async Task<DomainMergeResult> MergeRegisteredDomainsToDatabase(int registrarId, List<RegisteredDomainInfo> domains)
    {
        var result = new DomainMergeResult();
        
        try
        {
            _log.Debug("Starting merge of {DomainCount} domains for registrar {RegistrarId}", domains.Count, registrarId);

            foreach (var domainInfo in domains)
            {
                result.DomainsProcessed++;
                _log.Debug("Processing domain {CurrentDomain}/{TotalDomains}: {DomainName}", 
                    result.DomainsProcessed, domains.Count, domainInfo.DomainName);
                
                var normalizedName = domainInfo.DomainName.ToLowerInvariant();
                _log.Debug("Domain {DomainName} normalized to {NormalizedName}", domainInfo.DomainName, normalizedName);
                
                // Extract TLD from domain name
                var tldExtension = ExtractTldFromDomain(domainInfo.DomainName);
                if (string.IsNullOrEmpty(tldExtension))
                {
                    var warning = $"Could not extract TLD from domain {domainInfo.DomainName}";
                    _log.Warning(warning);
                    result.Warnings.Add(warning);
                    result.DomainsSkipped++;
                    continue;
                }
                
                _log.Debug("Extracted TLD extension: {TldExtension} from domain {DomainName}", tldExtension, domainInfo.DomainName);

                // Find or create TLD
                _log.Debug("Checking if TLD {TldExtension} exists in database", tldExtension);
                var tld = await _context.Tlds.FirstOrDefaultAsync(t => t.Extension == tldExtension);
                if (tld == null)
                {
                    _log.Information("TLD {Extension} not found, creating new TLD record", tldExtension);
                    tld = new Tld
                    {
                        Extension = tldExtension,
                        Description = $"{tldExtension.ToUpper()} domain",
                        IsActive = true,
                        DefaultRegistrationYears = 1,
                        MaxRegistrationYears = 10,
                        RequiresPrivacy = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Tlds.Add(tld);
                    await _context.SaveChangesAsync();
                    _log.Debug("Created TLD {Extension} with ID {TldId}", tldExtension, tld.Id);
                    result.TldsCreated++;
                }
                else
                {
                    _log.Debug("TLD {Extension} already exists with ID {TldId}", tldExtension, tld.Id);
                }

                // Find or create RegistrarTld relationship
                _log.Debug("Checking if RegistrarTld exists for registrar {RegistrarId} and TLD {TldId}", registrarId, tld.Id);
                var registrarTld = await _context.RegistrarTlds
                    .FirstOrDefaultAsync(rt => rt.RegistrarId == registrarId && rt.TldId == tld.Id);

                if (registrarTld == null)
                {
                    _log.Information("RegistrarTld not found, creating new relationship for registrar {RegistrarId} and TLD {TldExtension}", 
                        registrarId, tldExtension);
                    
                    registrarTld = new RegistrarTld
                    {
                        RegistrarId = registrarId,
                        TldId = tld.Id,
                        RegistrationCost = 0,
                        RegistrationPrice = 0,
                        RenewalCost = 0,
                        RenewalPrice = 0,
                        TransferCost = 0,
                        TransferPrice = 0,
                        Currency = "USD",
                        IsAvailable = true,
                        AutoRenew = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.RegistrarTlds.Add(registrarTld);
                    await _context.SaveChangesAsync();
                    _log.Debug("Created RegistrarTld with ID {RegistrarTldId}", registrarTld.Id);
                    result.RegistrarTldsCreated++;
                }
                else
                {
                    _log.Debug("RegistrarTld already exists with ID {RegistrarTldId}", registrarTld.Id);
                }

                // Find existing domain
                _log.Debug("Checking if domain {DomainName} exists in database (normalized: {NormalizedName})", 
                    domainInfo.DomainName, normalizedName);
                var domain = await _context.Domains
                    .FirstOrDefaultAsync(d => d.NormalizedName == normalizedName);

                if (domain != null)
                {
                    // Update existing domain
                    _log.Debug("Domain {DomainName} found with ID {DomainId}, updating existing record", 
                        domainInfo.DomainName, domain.Id);
                    _log.Debug("Updating domain: Status={Status}, ExpirationDate={ExpirationDate}, AutoRenew={AutoRenew}, PrivacyProtection={PrivacyProtection}",
                        domainInfo.Status, domainInfo.ExpirationDate, domainInfo.AutoRenew, domainInfo.PrivacyProtection);
                    
                    domain.Status = domainInfo.Status ?? domain.Status;
                    domain.ExpirationDate = domainInfo.ExpirationDate ?? domain.ExpirationDate;
                    domain.AutoRenew = domainInfo.AutoRenew;
                    domain.PrivacyProtection = domainInfo.PrivacyProtection;
                    domain.RegistrarTldId = registrarTld.Id;
                    domain.UpdatedAt = DateTime.UtcNow;
                    result.DomainsUpdated++;

                    // Merge contact information if available
                    if (domainInfo.Contacts != null && domainInfo.Contacts.Any())
                    {
                        _log.Debug("Merging {ContactCount} contacts for existing domain {DomainId}", 
                            domainInfo.Contacts.Count, domain.Id);
                        var contactStats = await MergeDomainContactsAsync(domain.Id, domainInfo.Contacts);
                        result.ContactsCreated += contactStats.Created;
                        result.ContactsUpdated += contactStats.Updated;
                        _log.Debug("Contact merge complete: {Created} created, {Updated} updated", 
                            contactStats.Created, contactStats.Updated);
                    }
                    else
                    {
                        _log.Debug("No contacts to merge for domain {DomainId}", domain.Id);
                    }
                }
                else
                {
                    _log.Debug("Domain {DomainName} not found in database, attempting to create new record", domainInfo.DomainName);
                    
                    // Try to find customer based on registrant email
                    int? customerId = null;
                    
                    if (domainInfo.Contacts != null && domainInfo.Contacts.Any())
                    {
                        _log.Debug("Processing {ContactCount} contacts to identify domain owner for {DomainName}", 
                            domainInfo.Contacts.Count, domainInfo.DomainName);
                        
                        // Look for registrant contact first, then admin contact
                        var registrantContact = domainInfo.Contacts
                            .FirstOrDefault(c => c.ContactType.Equals("Registrant", StringComparison.OrdinalIgnoreCase));
                        
                        var ownerContact = registrantContact ?? 
                            domainInfo.Contacts.FirstOrDefault(c => c.ContactType.Equals("Admin", StringComparison.OrdinalIgnoreCase)) ??
                            domainInfo.Contacts.FirstOrDefault();

                        if (ownerContact != null)
                        {
                            _log.Debug("Selected {ContactType} contact as owner: {Email}", 
                                ownerContact.ContactType, ownerContact.Email);
                        }

                        if (ownerContact != null && !string.IsNullOrWhiteSpace(ownerContact.Email))
                        {
                            _log.Debug("Looking up customer by email: {Email}", ownerContact.Email);
                            var customer = await _customerService.GetCustomerByEmailAsync(ownerContact.Email);
                            if (customer != null)
                            {
                                customerId = customer.Id;
                                _log.Information("Found customer {CustomerId} ({CustomerName}) for domain {DomainName} using email {Email}", 
                                    customerId, customer.Name, domainInfo.DomainName, ownerContact.Email);
                            }
                            else
                            {
                                var warning = $"No customer found with email {ownerContact.Email} for domain {domainInfo.DomainName}";
                                _log.Warning(warning);
                                result.Warnings.Add(warning);
                            }
                        }
                        else
                        {
                            var warning = $"No valid owner contact email found for domain {domainInfo.DomainName}";
                            _log.Debug(warning);
                            result.Warnings.Add(warning);
                        }
                    }
                    else
                    {
                        var warning = $"No contacts available for domain {domainInfo.DomainName}, cannot identify customer";
                        _log.Debug(warning);
                        result.Warnings.Add(warning);
                    }

                    if (!customerId.HasValue)
                    {
                        var warning = $"Domain {domainInfo.DomainName} skipped: no customer could be identified";
                        _log.Debug(warning);
                        result.Warnings.Add(warning);
                        result.DomainsSkipped++;
                    }
                    else
                    {
                        try
                        {
                            _log.Information("Creating new domain {DomainName} for customer {CustomerId}", 
                                domainInfo.DomainName, customerId);

                            _log.Debug("Looking up DOMAIN service type");
                            var serviceType = await _serviceTypeService.GetServiceTypeByNameAsync("DOMAIN");
                            if (serviceType == null)
                            {
                                var error = "Service type 'DOMAIN' not found in database";
                                _log.Error(error);
                                result.Errors.Add(error);
                                result.DomainsSkipped++;
                                continue;
                            }
                            _log.Debug("Found service type ID: {ServiceTypeId}", serviceType.Id);
                            
                            _log.Debug("Getting default reseller company");
                            var resellerCompany = await _resellerCompanyService.GetDefaultResellerCompanyAsync();
                            _log.Debug("Default reseller company ID: {ResellerCompanyId}", resellerCompany?.Id ?? 0);
                            
                            var createServiceDto = new CreateServiceDto
                            {
                                ServiceTypeId = serviceType.Id,
                                Name = domainInfo.DomainName + " Domain registration",
                                ResellerCompanyId = resellerCompany?.Id,
                            };

                            _log.Debug("Creating service for domain {DomainName}", domainInfo.DomainName);
                            var service = await _serviceService.CreateServiceAsync(createServiceDto);
                            _log.Debug("Created service with ID {ServiceId}", service.Id);

                            if (!domainInfo.ExpirationDate.HasValue || !domainInfo.RegistrationDate.HasValue)
                            {
                                var error = $"Domain {domainInfo.DomainName} missing required dates (Registration: {domainInfo.RegistrationDate}, Expiration: {domainInfo.ExpirationDate})";
                                _log.Error(error);
                                result.Errors.Add(error);
                                result.DomainsSkipped++;
                                continue;
                            }

                            var createDomainDto = new CreateDomainDto
                            {
                                ServiceId = service.Id,
                                CustomerId = customerId.Value,
                                ExpirationDate = domainInfo.ExpirationDate.Value,
                                RegistrationDate = domainInfo.RegistrationDate.Value,
                                ProviderId = registrarId,
                                Name = domainInfo.DomainName,
                                Status = "Imported",
                            };
                            
                            _log.Debug("Creating domain record: ServiceId={ServiceId}, CustomerId={CustomerId}, RegDate={RegistrationDate}, ExpDate={ExpirationDate}",
                                service.Id, customerId, domainInfo.RegistrationDate, domainInfo.ExpirationDate);
                            var createdDomain = await _domainService.CreateDomainAsync(createDomainDto);
                            
                            _log.Information("Successfully created domain {DomainName} with ID {DomainId}", 
                                domainInfo.DomainName, createdDomain.Id);
                            result.DomainsCreated++;

                            // Merge contact information if available
                            if (domainInfo.Contacts != null && domainInfo.Contacts.Any())
                            {
                                _log.Debug("Merging {ContactCount} contacts for newly created domain {DomainId}", 
                                    domainInfo.Contacts.Count, createdDomain.Id);
                                var contactStats = await MergeDomainContactsAsync(createdDomain.Id, domainInfo.Contacts);
                                result.ContactsCreated += contactStats.Created;
                                result.ContactsUpdated += contactStats.Updated;
                                _log.Debug("Contact merge complete: {Created} created, {Updated} updated", 
                                    contactStats.Created, contactStats.Updated);
                            }
                        }
                        catch (Exception ex)
                        {
                            var error = $"Failed to create domain {domainInfo.DomainName}: {ex.Message}";
                            _log.Error(ex, error);
                            result.Errors.Add(error);
                            result.DomainsSkipped++;
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            _log.Information("Database merge completed for registrar {RegistrarId}:", registrarId);
            _log.Information("  - Domains processed: {Processed}", result.DomainsProcessed);
            _log.Information("  - Domains created: {Created}", result.DomainsCreated);
            _log.Information("  - Domains updated: {Updated}", result.DomainsUpdated);
            _log.Information("  - Domains skipped: {Skipped}", result.DomainsSkipped);
            _log.Information("  - TLDs created: {TldsCreated}", result.TldsCreated);
            _log.Information("  - RegistrarTlds created: {RegistrarTldsCreated}", result.RegistrarTldsCreated);
            _log.Information("  - Contacts created: {ContactsCreated}", result.ContactsCreated);
            _log.Information("  - Contacts updated: {ContactsUpdated}", result.ContactsUpdated);
            
            if (result.Errors.Any())
            {
                _log.Error("  - Errors encountered: {ErrorCount}", result.Errors.Count);
            }
            if (result.Warnings.Any())
            {
                _log.Warning("  - Warnings: {WarningCount}", result.Warnings.Count);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error merging registered domains to database for registrar {RegistrarId}", registrarId);
            result.Errors.Add($"Fatal error during merge: {ex.Message}");
            return result;
        }
    }

    /// <summary>
    /// Merges domain contact information from registrar into the DomainContacts table
    /// </summary>
    /// <param name="domainId">The domain ID to associate contacts with</param>
    /// <param name="contacts">List of contact information from the registrar</param>
    /// <returns>Statistics about created and updated contacts</returns>
    private async Task<(int Created, int Updated)> MergeDomainContactsAsync(int domainId, List<DomainRegistrationLib.Models.DomainContactInfo> contacts)
    {
        try
        {
            int created = 0;
            int updated = 0;

            _log.Debug("Merging {Count} contacts for domain ID {DomainId}", contacts.Count, domainId);

            foreach (var contactInfo in contacts)
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(contactInfo.Email) || 
                    string.IsNullOrWhiteSpace(contactInfo.FirstName) || 
                    string.IsNullOrWhiteSpace(contactInfo.LastName))
                {
                    _log.Warning("Skipping contact for domain {DomainId} due to missing required fields (Email, FirstName, or LastName)", domainId);
                    continue;
                }

                // Try to find existing contact by type and email
                var existingContact = await _context.DomainContacts
                    .FirstOrDefaultAsync(dc => 
                        dc.DomainId == domainId && 
                        dc.ContactType == contactInfo.ContactType &&
                        dc.Email == contactInfo.Email);

                if (existingContact != null)
                {
                    // Update existing contact
                    _log.Debug("Updating existing {ContactType} contact for domain {DomainId}", contactInfo.ContactType, domainId);
                    
                    existingContact.FirstName = contactInfo.FirstName;
                    existingContact.LastName = contactInfo.LastName;
                    existingContact.Organization = contactInfo.Organization;
                    existingContact.Phone = contactInfo.Phone ?? existingContact.Phone;
                    existingContact.Fax = contactInfo.Fax;
                    existingContact.Address1 = contactInfo.Address1 ?? existingContact.Address1;
                    existingContact.Address2 = contactInfo.Address2;
                    existingContact.City = contactInfo.City ?? existingContact.City;
                    existingContact.State = contactInfo.State;
                    existingContact.PostalCode = contactInfo.PostalCode ?? existingContact.PostalCode;
                    existingContact.CountryCode = contactInfo.CountryCode ?? existingContact.CountryCode;
                    existingContact.IsActive = contactInfo.IsActive;
                    existingContact.Notes = contactInfo.Notes;
                    existingContact.UpdatedAt = DateTime.UtcNow;
                    
                    updated++;
                }
                else
                {
                    // Create new contact
                    _log.Debug("Creating new {ContactType} contact for domain {DomainId}", contactInfo.ContactType, domainId);
                    
                    var newContact = new DomainContact
                    {
                        DomainId = domainId,
                        ContactType = contactInfo.ContactType,
                        FirstName = contactInfo.FirstName,
                        LastName = contactInfo.LastName,
                        Organization = contactInfo.Organization,
                        Email = contactInfo.Email,
                        Phone = contactInfo.Phone ?? string.Empty,
                        Fax = contactInfo.Fax,
                        Address1 = contactInfo.Address1 ?? string.Empty,
                        Address2 = contactInfo.Address2,
                        City = contactInfo.City ?? string.Empty,
                        State = contactInfo.State,
                        PostalCode = contactInfo.PostalCode ?? string.Empty,
                        CountryCode = contactInfo.CountryCode ?? string.Empty,
                        IsActive = contactInfo.IsActive,
                        Notes = contactInfo.Notes,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    
                    _context.DomainContacts.Add(newContact);
                    created++;
                }
            }

            return (created, updated);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error merging domain contacts for domain ID {DomainId}", domainId);
            throw;
        }
    }

    /// <summary>
    /// Extracts TLD extension from a domain name
    /// </summary>
    private string ExtractTldFromDomain(string domainName)
    {
        if (string.IsNullOrEmpty(domainName))
            return string.Empty;

        var normalized = domainName.TrimEnd('.').ToLowerInvariant();
        var lastDotIndex = normalized.LastIndexOf('.');
        
        if (lastDotIndex <= 0 || lastDotIndex >= normalized.Length - 1)
            return string.Empty;

        return normalized.Substring(lastDotIndex + 1);
    }
}

