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

public class RegistrarService : IRegistrarService
{
    private readonly ApplicationDbContext _context;
    private readonly DomainRegistrarFactory _registrarFactory;
    private readonly ICustomerService _customerService;
    private readonly IServiceTypeService _serviceTypeService;
    private readonly IServiceService _serviceService;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarService>();

    public RegistrarService(ApplicationDbContext context, DomainRegistrarFactory registrarFactory, ICustomerService customerService, IServiceTypeService serviceTypeService, IServiceService serviceService)
    {
        _context = context;
        _registrarFactory = registrarFactory;
        _customerService = customerService;
        _serviceTypeService = serviceTypeService;
        _serviceService = serviceService;
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

            // If save is true, merge to database
            if (result.Domains != null && result.Domains.Any())
            {
                await MergeRegisteredDomainsToDatabase(registrarId, result.Domains);
            }


            // Return count of domains processed
            return result.TotalCount;
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
    private async Task MergeRegisteredDomainsToDatabase(int registrarId, List<RegisteredDomainInfo> domains)
    {
        try
        {
            int domainsUpdated = 0;
            int tldsCreated = 0;
            int registrarTldsCreated = 0;
            int contactsCreated = 0;
            int contactsUpdated = 0;

            foreach (var domainInfo in domains)
            {
                var normalizedName = domainInfo.DomainName.ToLowerInvariant();
                
                // Extract TLD from domain name
                var tldExtension = ExtractTldFromDomain(domainInfo.DomainName);
                if (string.IsNullOrEmpty(tldExtension))
                {
                    _log.Warning("Could not extract TLD from domain {DomainName}, skipping", domainInfo.DomainName);
                    continue;
                }

                // Find or create TLD
                var tld = await _context.Tlds.FirstOrDefaultAsync(t => t.Extension == tldExtension);
                if (tld == null)
                {
                    _log.Information("Creating new TLD: {Extension}", tldExtension);
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
                    tldsCreated++;
                }

                // Find or create RegistrarTld relationship
                var registrarTld = await _context.RegistrarTlds
                    .FirstOrDefaultAsync(rt => rt.RegistrarId == registrarId && rt.TldId == tld.Id);

                if (registrarTld == null)
                {
                    _log.Information("Creating new RegistrarTld for registrar {RegistrarId} and TLD {TldExtension}", 
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
                    registrarTldsCreated++;
                }

                // Find existing domain
                var domain = await _context.Domains
                    .FirstOrDefaultAsync(d => d.NormalizedName == normalizedName);

                if (domain != null)
                {
                    // Update existing domain
                    _log.Debug("Updating existing domain {DomainName}", domainInfo.DomainName);
                    domain.Status = domainInfo.Status ?? domain.Status;
                    domain.ExpirationDate = domainInfo.ExpirationDate ?? domain.ExpirationDate;
                    domain.AutoRenew = domainInfo.AutoRenew;
                    domain.PrivacyProtection = domainInfo.PrivacyProtection;
                    domain.RegistrarTldId = registrarTld.Id;
                    domain.UpdatedAt = DateTime.UtcNow;
                    domainsUpdated++;

                    // Merge contact information if available
                    if (domainInfo.Contacts != null && domainInfo.Contacts.Any())
                    {
                        var contactStats = await MergeDomainContactsAsync(domain.Id, domainInfo.Contacts);
                        contactsCreated += contactStats.Created;
                        contactsUpdated += contactStats.Updated;
                    }
                }
                else
                {
                    // Try to find customer based on registrant email
                    int? customerId = null;
                    
                    if (domainInfo.Contacts != null && domainInfo.Contacts.Any())
                    {
                        // Look for registrant contact first, then admin contact
                        var registrantContact = domainInfo.Contacts
                            .FirstOrDefault(c => c.ContactType.Equals("Registrant", StringComparison.OrdinalIgnoreCase));
                        
                        var ownerContact = registrantContact ?? 
                            domainInfo.Contacts.FirstOrDefault(c => c.ContactType.Equals("Admin", StringComparison.OrdinalIgnoreCase)) ??
                            domainInfo.Contacts.FirstOrDefault();

                        if (ownerContact != null && !string.IsNullOrWhiteSpace(ownerContact.Email))
                        {
                            var customer = await _customerService.GetCustomerByEmailAsync(ownerContact.Email);
                            if (customer != null)
                            {
                                customerId = customer.Id;
                                _log.Information("Found customer {CustomerId} for domain {DomainName} using email {Email}", 
                                    customerId, domainInfo.DomainName, ownerContact.Email);
                            }
                            else
                            {
                                _log.Warning("No customer found with email {Email} for domain {DomainName}", 
                                    ownerContact.Email, domainInfo.DomainName);
                            }
                        }
                    }

                    if (!customerId.HasValue)
                    {
                        _log.Debug("Domain {DomainName} not found in database and no customer could be identified for registrar {RegistrarId}, skipping creation", 
                            domainInfo.DomainName, registrarId);
                    }
                    else
                    {
                        _log.Information("Creating new domain {DomainName} for customer {CustomerId}", 
                            domainInfo.DomainName, customerId);

                        var serviceType = await _serviceTypeService.GetServiceTypeByNameAsync("DOMAIN") ?? throw new NullReferenceException("Service type not found");

                        var createServiceDto = new CreateServiceDto
                        {
                            ServiceTypeId = serviceType.Id,
                            Name = domainInfo.DomainName + " Domain registration",
                        };

                        var createDomainDto = new CreateDomainDto
                        {
                            ServiceId = serviceType.Id,
                            CustomerId = (int) customerId,
                            ExpirationDate = (DateTime) domainInfo.ExpirationDate!,
                            RegistrationDate = (DateTime) domainInfo.RegistrationDate!,
                            ProviderId = registrarId,
                            Name = domainInfo.DomainName + " Domain registration",
                            Status = "Imported",
                        };
                        //await _serviceService.CreateServiceAsync(createDto);
                        _log.Warning("Cannot create domain {DomainName}: ServiceId is required but not available from registrar data. Manual creation required.", 
                            domainInfo.DomainName);
                    }
                }
            }

            await _context.SaveChangesAsync();
            _log.Information("Database merge completed: {DomainsUpdated} domains updated, {TldsCreated} TLDs created, {RegistrarTldsCreated} RegistrarTlds created, {ContactsCreated} contacts created, {ContactsUpdated} contacts updated", 
                domainsUpdated, tldsCreated, registrarTldsCreated, contactsCreated, contactsUpdated);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error merging registered domains to database for registrar {RegistrarId}", registrarId);
            throw;
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
