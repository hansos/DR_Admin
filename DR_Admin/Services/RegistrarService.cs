using DomainRegistrationLib.Factories;
using DomainRegistrationLib.Implementations;
using DomainRegistrationLib.Interfaces;
using DomainRegistrationLib.Models;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using ISPAdmin.Services.Helpers;
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
    public int NameServersCreated { get; set; }
    public int NameServersUpdated { get; set; }
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
    private readonly DomainMergeHelper _domainMergeHelper;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarService>();

    public RegistrarService(ApplicationDbContext context, DomainRegistrarFactory registrarFactory, 
        DomainMergeHelper domainMergeHelper)
    {
        _context = context;
        _registrarFactory = registrarFactory;
        _domainMergeHelper = domainMergeHelper;
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
                IsActive = true,
                AutoRenew = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.RegistrarTlds.Add(registrarTld);
            await _context.SaveChangesAsync();

            await _context.Entry(registrarTld).Reference(rt => rt.Registrar).LoadAsync();
            await _context.Entry(registrarTld).Reference(rt => rt.Tld).LoadAsync();

            _log.Information("Successfully created registrar TLD with ID: {RegistrarTldId}. " +
                "Pricing should be created separately using RegistrarTldCostPricing and TldSalesPricing endpoints.",
                registrarTld.Id);
            return new RegistrarTldDto
            {
                Id = registrarTld.Id,
                RegistrarId = registrarTld.RegistrarId,
                RegistrarName = registrarTld.Registrar?.Name,
                TldId = registrarTld.TldId,
                TldExtension = registrarTld.Tld?.Extension,
                IsActive = registrarTld.IsActive,
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
                IsActive = true,
                AutoRenew = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.RegistrarTlds.Add(registrarTld);
            await _context.SaveChangesAsync();

            await _context.Entry(registrarTld).Reference(rt => rt.Registrar).LoadAsync();
            await _context.Entry(registrarTld).Reference(rt => rt.Tld).LoadAsync();

            _log.Information("Successfully assigned TLD {TldId} to registrar {RegistrarId}. " +
                "Pricing should be created separately using RegistrarTldCostPricing and TldSalesPricing endpoints.",
                tldId, registrarId);
            return new RegistrarTldDto
            {
                Id = registrarTld.Id,
                RegistrarId = registrarTld.RegistrarId,
                RegistrarName = registrarTld.Registrar?.Name,
                TldId = registrarTld.TldId,
                TldExtension = registrarTld.Tld?.Extension,
                IsActive = registrarTld.IsActive,
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
                IsActive = createDto.IsActive,
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

            _log.Information("Successfully created registrar TLD with ID: {RegistrarTldId}. " +
                "Pricing should be created separately using RegistrarTldCostPricing and TldSalesPricing endpoints.",
                registrarTld.Id);
            return new RegistrarTldDto
            {
                Id = registrarTld.Id,
                RegistrarId = registrarTld.RegistrarId,
                RegistrarName = registrarTld.Registrar?.Name,
                TldId = registrarTld.TldId,
                TldExtension = registrarTld.Tld?.Extension,
                IsActive = registrarTld.IsActive,
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

            // If this registrar is being set as default, unset other defaults
            if (createDto.IsDefault)
            {
                await UnsetAllDefaultRegistrarsAsync();
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
                IsDefault = createDto.IsDefault,
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

            // If this registrar is being set as default, unset other defaults
            if (updateDto.IsDefault && !registrar.IsDefault)
            {
                await UnsetAllDefaultRegistrarsAsync();
            }

            registrar.Name = updateDto.Name;
            registrar.Code = updateDto.Code.ToUpper();
            registrar.IsActive = updateDto.IsActive;
            registrar.ContactEmail = updateDto.ContactEmail;
            registrar.ContactPhone = updateDto.ContactPhone;
            registrar.Website = updateDto.Website;
            registrar.Notes = updateDto.Notes;
            registrar.IsDefault = updateDto.IsDefault;
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

            var hasDomains = await _context.RegisteredDomains.AnyAsync(d => d.RegistrarId == id);
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
            IsDefault = registrar.IsDefault,
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
                    _log.Information("Creating new RegistrarTld for registrar {RegistrarId} and TLD {TldExtension}. " +
                        "Pricing should be created separately using RegistrarTldCostPricing and TldSalesPricing endpoints.",
                        registrarId, extension);

                    registrarTld = new RegistrarTld
                    {
                        RegistrarId = registrarId,
                        TldId = tldEntity.Id,
                        IsActive = true,
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
                    _log.Information("Updating existing RegistrarTld for registrar {RegistrarId} and TLD {TldExtension}. " +
                        "Pricing updates should be done via RegistrarTldCostPricing and TldSalesPricing endpoints.",
                        registrarId, extension);

                    // Only update non-pricing fields
                    registrarTld.MinRegistrationYears = tldInfo.MinRegistrationYears ?? registrarTld.MinRegistrationYears;
                    registrarTld.MaxRegistrationYears = tldInfo.MaxRegistrationYears ?? registrarTld.MaxRegistrationYears;
                    registrarTld.IsActive = true;
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
                    _log.Information("Creating new RegistrarTld for registrar {RegistrarId} and TLD {TldExtension}. " +
                        "Pricing should be created separately using RegistrarTldCostPricing and TldSalesPricing endpoints.",
                        registrarId, extension);

                    registrarTld = new RegistrarTld
                    {
                        RegistrarId = registrarId,
                        TldId = tldEntity.Id,
                        IsActive = true,
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
                    _log.Information("Updating existing RegistrarTld for registrar {RegistrarId} and TLD {TldExtension}. " +
                        "Pricing updates should be done via RegistrarTldCostPricing and TldSalesPricing endpoints.",
                        registrarId, extension);

                    // Only update non-pricing fields
                    registrarTld.MinRegistrationYears = tldInfo.MinRegistrationYears ?? registrarTld.MinRegistrationYears;
                    registrarTld.MaxRegistrationYears = tldInfo.MaxRegistrationYears ?? registrarTld.MaxRegistrationYears;
                    registrarTld.IsActive = true;
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
                    _log.Information("Creating new RegistrarTld for registrar {RegistrarId} and TLD {TldExtension}. " +
                        "Pricing should be created separately using RegistrarTldCostPricing and TldSalesPricing endpoints.",
                        registrarId, extension);
                    
                    registrarTld = new RegistrarTld
                    {
                        RegistrarId = registrarId,
                        TldId = tld.Id,
                        IsActive = true,
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
                    _log.Information("Updating existing RegistrarTld for registrar {RegistrarId} and TLD {TldExtension}. " +
                        "Pricing updates should be done via RegistrarTldCostPricing and TldSalesPricing endpoints.",
                        registrarId, extension);
                    
                    // Only update non-pricing fields
                    registrarTld.MinRegistrationYears = tldInfo.MinRegistrationYears ?? registrarTld.MinRegistrationYears;
                    registrarTld.MaxRegistrationYears = tldInfo.MaxRegistrationYears ?? registrarTld.MaxRegistrationYears;
                    registrarTld.IsActive = true;
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
                var mergeResult = await _domainMergeHelper.MergeRegisteredDomainsToDatabase(registrarId, result.Domains);
                
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
    /// Sets a registrar as the default registrar
    /// </summary>
    /// <param name="id">The unique identifier of the registrar to set as default</param>
    /// <returns>True if successful, false if registrar not found</returns>
    public async Task<bool> SetDefaultRegistrarAsync(int id)
    {
        try
        {
            _log.Information("Setting registrar {RegistrarId} as default", id);

            var registrar = await _context.Registrars.FindAsync(id);

            if (registrar == null)
            {
                _log.Warning("Registrar with ID {RegistrarId} not found", id);
                return false;
            }

            // Unset all other defaults
            await UnsetAllDefaultRegistrarsAsync();

            registrar.IsDefault = true;
            await _context.SaveChangesAsync();

            _log.Information("Successfully set registrar {RegistrarId} as default", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while setting registrar {RegistrarId} as default", id);
            throw;
        }
    }

    /// <summary>
    /// Unsets the default flag on all registrars
    /// </summary>
    private async Task UnsetAllDefaultRegistrarsAsync()
    {
        var defaultRegistrars = await _context.Registrars
            .Where(r => r.IsDefault)
            .ToListAsync();

        foreach (var registrar in defaultRegistrars)
        {
            registrar.IsDefault = false;
        }

        if (defaultRegistrars.Any())
        {
            await _context.SaveChangesAsync();
        }
    }
}


