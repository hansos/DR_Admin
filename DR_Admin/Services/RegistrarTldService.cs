using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class RegistrarTldService : IRegistrarTldService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarTldService>();

    public RegistrarTldService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RegistrarTldDto>> GetAllRegistrarTldsAsync()
    {
        try
        {
            _log.Information("Fetching all registrar TLDs");
            
            var registrarTlds = await _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .OrderBy(rt => rt.Registrar.Name)
                .ThenBy(rt => rt.Tld.Extension)
                .ToListAsync();

            var registrarTldDtos = registrarTlds.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} registrar TLDs", registrarTlds.Count);
            return registrarTldDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all registrar TLDs");
            throw;
        }
    }

    public async Task<IEnumerable<RegistrarTldDto>> GetAvailableRegistrarTldsAsync()
    {
        try
        {
            _log.Information("Fetching available registrar TLDs");
            
            var registrarTlds = await _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .Where(rt => rt.IsAvailable && rt.Registrar.IsActive && rt.Tld.IsActive)
                .OrderBy(rt => rt.Registrar.Name)
                .ThenBy(rt => rt.Tld.Extension)
                .ToListAsync();

            var registrarTldDtos = registrarTlds.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} available registrar TLDs", registrarTlds.Count);
            return registrarTldDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching available registrar TLDs");
            throw;
        }
    }

    public async Task<IEnumerable<RegistrarTldDto>> GetRegistrarTldsByRegistrarAsync(int registrarId)
    {
        try
        {
            _log.Information("Fetching registrar TLDs for registrar: {RegistrarId}", registrarId);
            
            var registrarTlds = await _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .Where(rt => rt.RegistrarId == registrarId)
                .OrderBy(rt => rt.Tld.Extension)
                .ToListAsync();

            var registrarTldDtos = registrarTlds.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} registrar TLDs for registrar {RegistrarId}", 
                registrarTlds.Count, registrarId);
            return registrarTldDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching registrar TLDs for registrar: {RegistrarId}", registrarId);
            throw;
        }
    }

    public async Task<IEnumerable<RegistrarTldDto>> GetRegistrarTldsByTldAsync(int tldId)
    {
        try
        {
            _log.Information("Fetching registrar TLDs for TLD: {TldId}", tldId);
            
            var registrarTlds = await _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .Where(rt => rt.TldId == tldId)
                .OrderBy(rt => rt.Registrar.Name)
                .ToListAsync();

            var registrarTldDtos = registrarTlds.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} registrar TLDs for TLD {TldId}", 
                registrarTlds.Count, tldId);
            return registrarTldDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching registrar TLDs for TLD: {TldId}", tldId);
            throw;
        }
    }

    public async Task<RegistrarTldDto?> GetRegistrarTldByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching registrar TLD with ID: {RegistrarTldId}", id);
            
            var registrarTld = await _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .FirstOrDefaultAsync(rt => rt.Id == id);

            if (registrarTld == null)
            {
                _log.Warning("Registrar TLD with ID {RegistrarTldId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched registrar TLD with ID: {RegistrarTldId}", id);
            return MapToDto(registrarTld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching registrar TLD with ID: {RegistrarTldId}", id);
            throw;
        }
    }

    public async Task<RegistrarTldDto?> GetRegistrarTldByRegistrarAndTldAsync(int registrarId, int tldId)
    {
        try
        {
            _log.Information("Fetching registrar TLD for registrar {RegistrarId} and TLD {TldId}", 
                registrarId, tldId);
            
            var registrarTld = await _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .FirstOrDefaultAsync(rt => rt.RegistrarId == registrarId && rt.TldId == tldId);

            if (registrarTld == null)
            {
                _log.Warning("Registrar TLD for registrar {RegistrarId} and TLD {TldId} not found", 
                    registrarId, tldId);
                return null;
            }

            _log.Information("Successfully fetched registrar TLD for registrar {RegistrarId} and TLD {TldId}", 
                registrarId, tldId);
            return MapToDto(registrarTld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching registrar TLD for registrar {RegistrarId} and TLD {TldId}", 
                registrarId, tldId);
            throw;
        }
    }

    public async Task<RegistrarTldDto> CreateRegistrarTldAsync(CreateRegistrarTldDto createDto)
    {
        try
        {
            _log.Information("Creating new registrar TLD for registrar {RegistrarId} and TLD {TldId}", 
                createDto.RegistrarId, createDto.TldId);

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

            var existingRegistrarTld = await _context.RegistrarTlds
                .FirstOrDefaultAsync(rt => rt.RegistrarId == createDto.RegistrarId && rt.TldId == createDto.TldId);

            if (existingRegistrarTld != null)
            {
                _log.Warning("Registrar TLD for registrar {RegistrarId} and TLD {TldId} already exists", 
                    createDto.RegistrarId, createDto.TldId);
                throw new InvalidOperationException("This registrar-TLD combination already exists");
            }

            var registrarTld = new RegistrarTld
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

            // Reload with navigation properties
            await _context.Entry(registrarTld).Reference(rt => rt.Registrar).LoadAsync();
            await _context.Entry(registrarTld).Reference(rt => rt.Tld).LoadAsync();

            _log.Information("Successfully created registrar TLD with ID: {RegistrarTldId}", registrarTld.Id);
            return MapToDto(registrarTld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating registrar TLD for registrar {RegistrarId} and TLD {TldId}", 
                createDto.RegistrarId, createDto.TldId);
            throw;
        }
    }

    public async Task<RegistrarTldDto?> UpdateRegistrarTldAsync(int id, UpdateRegistrarTldDto updateDto)
    {
        try
        {
            _log.Information("Updating registrar TLD with ID: {RegistrarTldId}", id);

            var registrarTld = await _context.RegistrarTlds
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .FirstOrDefaultAsync(rt => rt.Id == id);

            if (registrarTld == null)
            {
                _log.Warning("Registrar TLD with ID {RegistrarTldId} not found for update", id);
                return null;
            }

            var registrarExists = await _context.Registrars.AnyAsync(r => r.Id == updateDto.RegistrarId);
            if (!registrarExists)
            {
                _log.Warning("Registrar with ID {RegistrarId} does not exist", updateDto.RegistrarId);
                throw new InvalidOperationException($"Registrar with ID {updateDto.RegistrarId} does not exist");
            }

            var tldExists = await _context.Tlds.AnyAsync(t => t.Id == updateDto.TldId);
            if (!tldExists)
            {
                _log.Warning("TLD with ID {TldId} does not exist", updateDto.TldId);
                throw new InvalidOperationException($"TLD with ID {updateDto.TldId} does not exist");
            }

            var duplicateCombination = await _context.RegistrarTlds
                .AnyAsync(rt => rt.RegistrarId == updateDto.RegistrarId && rt.TldId == updateDto.TldId && rt.Id != id);

            if (duplicateCombination)
            {
                _log.Warning("Cannot update registrar TLD {RegistrarTldId}: combination already exists", id);
                throw new InvalidOperationException("This registrar-TLD combination already exists");
            }

            registrarTld.RegistrarId = updateDto.RegistrarId;
            registrarTld.TldId = updateDto.TldId;
            registrarTld.RegistrationCost = updateDto.RegistrationCost;
            registrarTld.RegistrationPrice = updateDto.RegistrationPrice;
            registrarTld.RenewalCost = updateDto.RenewalCost;
            registrarTld.RenewalPrice = updateDto.RenewalPrice;
            registrarTld.TransferCost = updateDto.TransferCost;
            registrarTld.TransferPrice = updateDto.TransferPrice;
            registrarTld.PrivacyCost = updateDto.PrivacyCost;
            registrarTld.PrivacyPrice = updateDto.PrivacyPrice;
            registrarTld.Currency = updateDto.Currency;
            registrarTld.IsAvailable = updateDto.IsAvailable;
            registrarTld.AutoRenew = updateDto.AutoRenew;
            registrarTld.MinRegistrationYears = updateDto.MinRegistrationYears;
            registrarTld.MaxRegistrationYears = updateDto.MaxRegistrationYears;
            registrarTld.Notes = updateDto.Notes;
            registrarTld.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated registrar TLD with ID: {RegistrarTldId}", id);
            return MapToDto(registrarTld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating registrar TLD with ID: {RegistrarTldId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteRegistrarTldAsync(int id)
    {
        try
        {
            _log.Information("Deleting registrar TLD with ID: {RegistrarTldId}", id);

            var registrarTld = await _context.RegistrarTlds.FindAsync(id);

            if (registrarTld == null)
            {
                _log.Warning("Registrar TLD with ID {RegistrarTldId} not found for deletion", id);
                return false;
            }

            var hasDomains = await _context.RegisteredDomains.AnyAsync(d => d.RegistrarTldId == id);
            if (hasDomains)
            {
                _log.Warning("Cannot delete registrar TLD {RegistrarTldId}: has associated domains", id);
                throw new InvalidOperationException("Cannot delete registrar TLD with associated domains");
            }

            _context.RegistrarTlds.Remove(registrarTld);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted registrar TLD with ID: {RegistrarTldId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting registrar TLD with ID: {RegistrarTldId}", id);
            throw;
        }
    }

    private static RegistrarTldDto MapToDto(RegistrarTld registrarTld)
    {
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
}
