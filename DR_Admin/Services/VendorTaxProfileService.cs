using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing vendor tax profile operations
/// </summary>
public class VendorTaxProfileService : IVendorTaxProfileService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<VendorTaxProfileService>();

    public VendorTaxProfileService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VendorTaxProfileDto>> GetAllVendorTaxProfilesAsync()
    {
        try
        {
            _log.Information("Fetching all vendor tax profiles");
            
            var profiles = await _context.VendorTaxProfiles
                .AsNoTracking()
                .ToListAsync();

            _log.Information("Successfully fetched {Count} vendor tax profiles", profiles.Count);
            return profiles.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all vendor tax profiles");
            throw;
        }
    }

    public async Task<VendorTaxProfileDto?> GetVendorTaxProfileByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching vendor tax profile with ID: {Id}", id);

            var profile = await _context.VendorTaxProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profile == null)
            {
                _log.Warning("Vendor tax profile with ID {Id} not found", id);
                return null;
            }

            return MapToDto(profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching vendor tax profile with ID: {Id}", id);
            throw;
        }
    }

    public async Task<VendorTaxProfileDto?> GetVendorTaxProfileByVendorIdAsync(int vendorId)
    {
        try
        {
            _log.Information("Fetching vendor tax profile for vendor ID: {VendorId}", vendorId);

            var profile = await _context.VendorTaxProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.VendorId == vendorId);

            if (profile == null)
            {
                _log.Warning("Vendor tax profile for vendor ID {VendorId} not found", vendorId);
                return null;
            }

            return MapToDto(profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching vendor tax profile for vendor ID: {VendorId}", vendorId);
            throw;
        }
    }

    public async Task<VendorTaxProfileDto> CreateVendorTaxProfileAsync(CreateVendorTaxProfileDto dto)
    {
        try
        {
            _log.Information("Creating new vendor tax profile for vendor ID: {VendorId}", dto.VendorId);

            var profile = new VendorTaxProfile
            {
                VendorId = dto.VendorId,
                VendorType = dto.VendorType,
                TaxIdNumber = dto.TaxIdNumber,
                TaxResidenceCountry = dto.TaxResidenceCountry,
                Require1099 = dto.Require1099,
                W9OnFile = dto.W9OnFile,
                W9FileUrl = dto.W9FileUrl,
                WithholdingTaxRate = dto.WithholdingTaxRate,
                TaxTreatyExempt = dto.TaxTreatyExempt,
                TaxTreatyCountry = dto.TaxTreatyCountry,
                TaxNotes = dto.TaxNotes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.VendorTaxProfiles.Add(profile);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created vendor tax profile with ID: {Id}", profile.Id);
            return MapToDto(profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating vendor tax profile");
            throw;
        }
    }

    public async Task<VendorTaxProfileDto?> UpdateVendorTaxProfileAsync(int id, UpdateVendorTaxProfileDto dto)
    {
        try
        {
            _log.Information("Updating vendor tax profile with ID: {Id}", id);

            var profile = await _context.VendorTaxProfiles.FindAsync(id);

            if (profile == null)
            {
                _log.Warning("Vendor tax profile with ID {Id} not found for update", id);
                return null;
            }

            profile.TaxIdNumber = dto.TaxIdNumber;
            profile.TaxResidenceCountry = dto.TaxResidenceCountry;
            profile.Require1099 = dto.Require1099;
            profile.W9OnFile = dto.W9OnFile;
            profile.W9FileUrl = dto.W9FileUrl;
            profile.WithholdingTaxRate = dto.WithholdingTaxRate;
            profile.TaxTreatyExempt = dto.TaxTreatyExempt;
            profile.TaxTreatyCountry = dto.TaxTreatyCountry;
            profile.TaxNotes = dto.TaxNotes;
            profile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated vendor tax profile with ID: {Id}", id);
            return MapToDto(profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating vendor tax profile with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteVendorTaxProfileAsync(int id)
    {
        try
        {
            _log.Information("Deleting vendor tax profile with ID: {Id}", id);

            var profile = await _context.VendorTaxProfiles.FindAsync(id);

            if (profile == null)
            {
                _log.Warning("Vendor tax profile with ID {Id} not found for deletion", id);
                return false;
            }

            _context.VendorTaxProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted vendor tax profile with ID: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting vendor tax profile with ID: {Id}", id);
            throw;
        }
    }

    private static VendorTaxProfileDto MapToDto(VendorTaxProfile profile)
    {
        return new VendorTaxProfileDto
        {
            Id = profile.Id,
            VendorId = profile.VendorId,
            VendorType = profile.VendorType,
            TaxIdNumber = profile.TaxIdNumber,
            TaxResidenceCountry = profile.TaxResidenceCountry,
            Require1099 = profile.Require1099,
            W9OnFile = profile.W9OnFile,
            W9FileUrl = profile.W9FileUrl,
            WithholdingTaxRate = profile.WithholdingTaxRate,
            TaxTreatyExempt = profile.TaxTreatyExempt,
            TaxTreatyCountry = profile.TaxTreatyCountry,
            TaxNotes = profile.TaxNotes,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };
    }
}
