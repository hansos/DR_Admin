using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing customer tax profile operations
/// </summary>
public class CustomerTaxProfileService : ICustomerTaxProfileService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<CustomerTaxProfileService>();

    public CustomerTaxProfileService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerTaxProfileDto>> GetAllCustomerTaxProfilesAsync()
    {
        try
        {
            _log.Information("Fetching all customer tax profiles");
            
            var profiles = await _context.CustomerTaxProfiles
                .AsNoTracking()
                .ToListAsync();

            var dtos = profiles.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} customer tax profiles", profiles.Count);
            return dtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all customer tax profiles");
            throw;
        }
    }

    public async Task<CustomerTaxProfileDto?> GetCustomerTaxProfileByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching customer tax profile with ID: {Id}", id);

            var profile = await _context.CustomerTaxProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profile == null)
            {
                _log.Warning("Customer tax profile with ID {Id} not found", id);
                return null;
            }

            _log.Information("Successfully fetched customer tax profile with ID: {Id}", id);
            return MapToDto(profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching customer tax profile with ID: {Id}", id);
            throw;
        }
    }

    public async Task<CustomerTaxProfileDto?> GetCustomerTaxProfileByCustomerIdAsync(int customerId)
    {
        try
        {
            _log.Information("Fetching customer tax profile for customer ID: {CustomerId}", customerId);

            var profile = await _context.CustomerTaxProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.CustomerId == customerId);

            if (profile == null)
            {
                _log.Warning("Customer tax profile for customer ID {CustomerId} not found", customerId);
                return null;
            }

            _log.Information("Successfully fetched customer tax profile for customer ID: {CustomerId}", customerId);
            return MapToDto(profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching customer tax profile for customer ID: {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<CustomerTaxProfileDto> CreateCustomerTaxProfileAsync(CreateCustomerTaxProfileDto dto)
    {
        try
        {
            _log.Information("Creating new customer tax profile for customer ID: {CustomerId}", dto.CustomerId);

            var profile = new CustomerTaxProfile
            {
                CustomerId = dto.CustomerId,
                TaxIdNumber = dto.TaxIdNumber,
                TaxIdType = dto.TaxIdType,
                TaxResidenceCountry = dto.TaxResidenceCountry,
                CustomerType = dto.CustomerType,
                TaxExempt = dto.TaxExempt,
                TaxExemptionReason = dto.TaxExemptionReason,
                TaxExemptionCertificateUrl = dto.TaxExemptionCertificateUrl,
                TaxIdValidated = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.CustomerTaxProfiles.Add(profile);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created customer tax profile with ID: {Id}", profile.Id);
            return MapToDto(profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating customer tax profile");
            throw;
        }
    }

    public async Task<CustomerTaxProfileDto?> UpdateCustomerTaxProfileAsync(int id, UpdateCustomerTaxProfileDto dto)
    {
        try
        {
            _log.Information("Updating customer tax profile with ID: {Id}", id);

            var profile = await _context.CustomerTaxProfiles.FindAsync(id);

            if (profile == null)
            {
                _log.Warning("Customer tax profile with ID {Id} not found for update", id);
                return null;
            }

            profile.TaxIdNumber = dto.TaxIdNumber;
            profile.TaxIdType = dto.TaxIdType;
            profile.TaxResidenceCountry = dto.TaxResidenceCountry;
            profile.CustomerType = dto.CustomerType;
            profile.TaxExempt = dto.TaxExempt;
            profile.TaxExemptionReason = dto.TaxExemptionReason;
            profile.TaxExemptionCertificateUrl = dto.TaxExemptionCertificateUrl;
            profile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated customer tax profile with ID: {Id}", id);
            return MapToDto(profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating customer tax profile with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteCustomerTaxProfileAsync(int id)
    {
        try
        {
            _log.Information("Deleting customer tax profile with ID: {Id}", id);

            var profile = await _context.CustomerTaxProfiles.FindAsync(id);

            if (profile == null)
            {
                _log.Warning("Customer tax profile with ID {Id} not found for deletion", id);
                return false;
            }

            _context.CustomerTaxProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted customer tax profile with ID: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting customer tax profile with ID: {Id}", id);
            throw;
        }
    }

    public async Task<TaxIdValidationResultDto?> ValidateTaxIdAsync(ValidateTaxIdDto dto)
    {
        try
        {
            _log.Information("Validating tax ID for customer tax profile ID: {Id}", dto.CustomerTaxProfileId);

            var profile = await _context.CustomerTaxProfiles.FindAsync(dto.CustomerTaxProfileId);

            if (profile == null)
            {
                _log.Warning("Customer tax profile with ID {Id} not found for validation", dto.CustomerTaxProfileId);
                return null;
            }

            if (profile.TaxIdValidated && !dto.ForceRevalidation)
            {
                _log.Information("Tax ID already validated for profile ID: {Id}", dto.CustomerTaxProfileId);
                return new TaxIdValidationResultDto
                {
                    IsValid = true,
                    ValidationDate = profile.TaxIdValidationDate ?? DateTime.UtcNow,
                    ValidationService = "Cached",
                    RawResponse = "Previously validated"
                };
            }

            // TODO: Implement actual tax ID validation logic with external service
            // This is a placeholder implementation
            var isValid = !string.IsNullOrWhiteSpace(profile.TaxIdNumber);

            profile.TaxIdValidated = isValid;
            profile.TaxIdValidationDate = DateTime.UtcNow;
            profile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully validated tax ID for profile ID: {Id}, Result: {IsValid}", 
                dto.CustomerTaxProfileId, isValid);

            return new TaxIdValidationResultDto
            {
                IsValid = isValid,
                ValidationDate = DateTime.UtcNow,
                ValidationService = "PlaceholderValidator",
                RawResponse = "{\"validated\": true}"
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while validating tax ID for profile ID: {Id}", dto.CustomerTaxProfileId);
            throw;
        }
    }

    private static CustomerTaxProfileDto MapToDto(CustomerTaxProfile profile)
    {
        return new CustomerTaxProfileDto
        {
            Id = profile.Id,
            CustomerId = profile.CustomerId,
            TaxIdNumber = profile.TaxIdNumber,
            TaxIdType = profile.TaxIdType,
            TaxIdValidated = profile.TaxIdValidated,
            TaxIdValidationDate = profile.TaxIdValidationDate,
            TaxResidenceCountry = profile.TaxResidenceCountry,
            CustomerType = profile.CustomerType,
            TaxExempt = profile.TaxExempt,
            TaxExemptionReason = profile.TaxExemptionReason,
            TaxExemptionCertificateUrl = profile.TaxExemptionCertificateUrl,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };
    }
}
