using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing registrar mail address operations
/// </summary>
public class RegistrarMailAddressService : IRegistrarMailAddressService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarMailAddressService>();

    public RegistrarMailAddressService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all mail addresses for a specific customer
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <returns>A collection of all mail addresses for the customer</returns>
    public async Task<IEnumerable<RegistrarMailAddressDto>> GetRegistrarMailAddressesAsync(int customerId)
    {
        try
        {
            _log.Information("Fetching mail addresses for customer ID: {CustomerId}", customerId);
            
            var mailAddresses = await _context.RegistrarMailAddresses
                .AsNoTracking()
                .Where(rma => rma.CustomerId == customerId)
                .OrderByDescending(rma => rma.IsDefault)
                .ThenByDescending(rma => rma.IsActive)
                .ThenBy(rma => rma.MailAddress)
                .ToListAsync();

            var mailAddressDtos = mailAddresses.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} mail addresses for customer ID: {CustomerId}", mailAddresses.Count, customerId);
            return mailAddressDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching mail addresses for customer ID: {CustomerId}", customerId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a registrar mail address by its unique identifier
    /// </summary>
    /// <param name="id">The registrar mail address's unique identifier</param>
    /// <returns>The registrar mail address if found; otherwise, null</returns>
    public async Task<RegistrarMailAddressDto?> GetRegistrarMailAddressByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching registrar mail address with ID: {MailAddressId}", id);
            
            var mailAddress = await _context.RegistrarMailAddresses
                .AsNoTracking()
                .FirstOrDefaultAsync(rma => rma.Id == id);

            if (mailAddress == null)
            {
                _log.Warning("Registrar mail address with ID {MailAddressId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched registrar mail address with ID: {MailAddressId}", id);
            return MapToDto(mailAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching registrar mail address with ID: {MailAddressId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves the default mail address for a specific customer
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <returns>The default registrar mail address if found; otherwise, null</returns>
    public async Task<RegistrarMailAddressDto?> GetDefaultMailAddressAsync(int customerId)
    {
        try
        {
            _log.Information("Fetching default mail address for customer ID: {CustomerId}", customerId);
            
            var mailAddress = await _context.RegistrarMailAddresses
                .AsNoTracking()
                .FirstOrDefaultAsync(rma => rma.CustomerId == customerId && rma.IsDefault);

            if (mailAddress == null)
            {
                _log.Warning("Default mail address for customer ID {CustomerId} not found", customerId);
                return null;
            }

            _log.Information("Successfully fetched default mail address for customer ID: {CustomerId}", customerId);
            return MapToDto(mailAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching default mail address for customer ID: {CustomerId}", customerId);
            throw;
        }
    }

    /// <summary>
    /// Creates a new registrar mail address
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <param name="createDto">The registrar mail address data for creation</param>
    /// <returns>The newly created registrar mail address</returns>
    public async Task<RegistrarMailAddressDto> CreateRegistrarMailAddressAsync(int customerId, CreateRegistrarMailAddressDto createDto)
    {
        try
        {
            _log.Information("Creating new registrar mail address for customer ID: {CustomerId}", customerId);

            // If this mail address is set as default, unset any existing default mail address
            if (createDto.IsDefault)
            {
                await UnsetDefaultMailAddressesAsync(customerId);
            }

            var mailAddress = new RegistrarMailAddress
            {
                CustomerId = customerId,
                MailAddress = createDto.MailAddress,
                IsDefault = createDto.IsDefault,
                IsActive = createDto.IsActive
            };

            _context.RegistrarMailAddresses.Add(mailAddress);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created registrar mail address with ID: {MailAddressId}", mailAddress.Id);
            return MapToDto(mailAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating registrar mail address for customer ID: {CustomerId}", customerId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing registrar mail address
    /// </summary>
    /// <param name="id">The registrar mail address's unique identifier</param>
    /// <param name="updateDto">The updated registrar mail address data</param>
    /// <returns>The updated registrar mail address if found; otherwise, null</returns>
    public async Task<RegistrarMailAddressDto?> UpdateRegistrarMailAddressAsync(int id, UpdateRegistrarMailAddressDto updateDto)
    {
        try
        {
            _log.Information("Updating registrar mail address with ID: {MailAddressId}", id);

            var mailAddress = await _context.RegistrarMailAddresses.FindAsync(id);

            if (mailAddress == null)
            {
                _log.Warning("Registrar mail address with ID {MailAddressId} not found for update", id);
                return null;
            }

            // If this mail address is being set as default, unset any existing default mail address
            if (updateDto.IsDefault && !mailAddress.IsDefault)
            {
                await UnsetDefaultMailAddressesAsync(mailAddress.CustomerId);
            }

            mailAddress.MailAddress = updateDto.MailAddress;
            mailAddress.IsDefault = updateDto.IsDefault;
            mailAddress.IsActive = updateDto.IsActive;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated registrar mail address with ID: {MailAddressId}", id);
            return MapToDto(mailAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating registrar mail address with ID: {MailAddressId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a registrar mail address from the database
    /// </summary>
    /// <param name="id">The registrar mail address's unique identifier</param>
    /// <returns>True if the registrar mail address was deleted; otherwise, false</returns>
    public async Task<bool> DeleteRegistrarMailAddressAsync(int id)
    {
        try
        {
            _log.Information("Deleting registrar mail address with ID: {MailAddressId}", id);

            var mailAddress = await _context.RegistrarMailAddresses.FindAsync(id);

            if (mailAddress == null)
            {
                _log.Warning("Registrar mail address with ID {MailAddressId} not found for deletion", id);
                return false;
            }

            _context.RegistrarMailAddresses.Remove(mailAddress);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted registrar mail address with ID: {MailAddressId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting registrar mail address with ID: {MailAddressId}", id);
            throw;
        }
    }

    /// <summary>
    /// Sets a registrar mail address as the default mail address
    /// </summary>
    /// <param name="id">The registrar mail address's unique identifier</param>
    /// <returns>The updated registrar mail address if found; otherwise, null</returns>
    public async Task<RegistrarMailAddressDto?> SetDefaultMailAddressAsync(int id)
    {
        try
        {
            _log.Information("Setting registrar mail address with ID {MailAddressId} as default", id);

            var mailAddress = await _context.RegistrarMailAddresses.FindAsync(id);

            if (mailAddress == null)
            {
                _log.Warning("Registrar mail address with ID {MailAddressId} not found", id);
                return null;
            }

            // Unset any existing default mail address for this customer
            await UnsetDefaultMailAddressesAsync(mailAddress.CustomerId);

            // Set this mail address as default
            mailAddress.IsDefault = true;
            await _context.SaveChangesAsync();

            _log.Information("Successfully set registrar mail address with ID {MailAddressId} as default", id);
            return MapToDto(mailAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while setting registrar mail address with ID {MailAddressId} as default", id);
            throw;
        }
    }

    private async Task UnsetDefaultMailAddressesAsync(int customerId)
    {
        var defaultMailAddresses = await _context.RegistrarMailAddresses
            .Where(rma => rma.CustomerId == customerId && rma.IsDefault)
            .ToListAsync();

        foreach (var mailAddr in defaultMailAddresses)
        {
            mailAddr.IsDefault = false;
        }
    }

    private static RegistrarMailAddressDto MapToDto(RegistrarMailAddress mailAddress)
    {
        return new RegistrarMailAddressDto
        {
            Id = mailAddress.Id,
            CustomerId = mailAddress.CustomerId,
            MailAddress = mailAddress.MailAddress,
            IsDefault = mailAddress.IsDefault,
            IsActive = mailAddress.IsActive,
            CreatedAt = mailAddress.CreatedAt,
            UpdatedAt = mailAddress.UpdatedAt
        };
    }
}
