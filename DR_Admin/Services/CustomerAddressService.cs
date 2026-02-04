using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing customer address operations
/// </summary>
public class CustomerAddressService : ICustomerAddressService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<CustomerAddressService>();

    public CustomerAddressService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all addresses for a specific customer
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <returns>A collection of all addresses for the customer</returns>
    public async Task<IEnumerable<CustomerAddressDto>> GetCustomerAddressesAsync(int customerId)
    {
        try
        {
            _log.Information("Fetching addresses for customer ID: {CustomerId}", customerId);
            
            var addresses = await _context.CustomerAddresses
                .AsNoTracking()
                .Include(ca => ca.AddressType)
                .Include(ca => ca.PostalCode)
                .Where(ca => ca.CustomerId == customerId)
                .OrderByDescending(ca => ca.IsPrimary)
                .ThenBy(ca => ca.AddressType.SortOrder)
                .ToListAsync();

            var addressDtos = addresses.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} addresses for customer ID: {CustomerId}", addresses.Count, customerId);
            return addressDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching addresses for customer ID: {CustomerId}", customerId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a customer address by its unique identifier
    /// </summary>
    /// <param name="id">The customer address's unique identifier</param>
    /// <returns>The customer address if found; otherwise, null</returns>
    public async Task<CustomerAddressDto?> GetCustomerAddressByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching customer address with ID: {CustomerAddressId}", id);
            
            var address = await _context.CustomerAddresses
                .AsNoTracking()
                .Include(ca => ca.AddressType)
                .Include(ca => ca.PostalCode)
                .FirstOrDefaultAsync(ca => ca.Id == id);

            if (address == null)
            {
                _log.Warning("Customer address with ID {CustomerAddressId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched customer address with ID: {CustomerAddressId}", id);
            return MapToDto(address);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching customer address with ID: {CustomerAddressId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves the primary address for a specific customer
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <returns>The primary customer address if found; otherwise, null</returns>
    public async Task<CustomerAddressDto?> GetPrimaryAddressAsync(int customerId)
    {
        try
        {
            _log.Information("Fetching primary address for customer ID: {CustomerId}", customerId);
            
            var address = await _context.CustomerAddresses
                .AsNoTracking()
                .Include(ca => ca.AddressType)
                .Include(ca => ca.PostalCode)
                .FirstOrDefaultAsync(ca => ca.CustomerId == customerId && ca.IsPrimary);

            if (address == null)
            {
                _log.Warning("Primary address for customer ID {CustomerId} not found", customerId);
                return null;
            }

            _log.Information("Successfully fetched primary address for customer ID: {CustomerId}", customerId);
            return MapToDto(address);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching primary address for customer ID: {CustomerId}", customerId);
            throw;
        }
    }

    /// <summary>
    /// Creates a new customer address
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <param name="createDto">The customer address data for creation</param>
    /// <returns>The newly created customer address</returns>
    public async Task<CustomerAddressDto> CreateCustomerAddressAsync(int customerId, CreateCustomerAddressDto createDto)
    {
        try
        {
            _log.Information("Creating new customer address for customer ID: {CustomerId}", customerId);

            // If this address is set as primary, unset any existing primary address
            if (createDto.IsPrimary)
            {
                await UnsetPrimaryAddressesAsync(customerId);
            }

            var address = new CustomerAddress
            {
                CustomerId = customerId,
                AddressTypeId = createDto.AddressTypeId,
                PostalCodeId = createDto.PostalCodeId,
                AddressLine1 = createDto.AddressLine1,
                AddressLine2 = createDto.AddressLine2,
                AddressLine3 = createDto.AddressLine3,
                AddressLine4 = createDto.AddressLine4,
                IsPrimary = createDto.IsPrimary,
                IsActive = createDto.IsActive,
                Notes = createDto.Notes
            };

            _context.CustomerAddresses.Add(address);
            await _context.SaveChangesAsync();

            // Reload to include AddressType and PostalCode
            var createdAddress = await _context.CustomerAddresses
                .Include(ca => ca.AddressType)
                .Include(ca => ca.PostalCode)
                .FirstAsync(ca => ca.Id == address.Id);

            _log.Information("Successfully created customer address with ID: {CustomerAddressId}", address.Id);
            return MapToDto(createdAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating customer address for customer ID: {CustomerId}", customerId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing customer address
    /// </summary>
    /// <param name="id">The customer address's unique identifier</param>
    /// <param name="updateDto">The updated customer address data</param>
    /// <returns>The updated customer address if found; otherwise, null</returns>
    public async Task<CustomerAddressDto?> UpdateCustomerAddressAsync(int id, UpdateCustomerAddressDto updateDto)
    {
        try
        {
            _log.Information("Updating customer address with ID: {CustomerAddressId}", id);

            var address = await _context.CustomerAddresses.FindAsync(id);

            if (address == null)
            {
                _log.Warning("Customer address with ID {CustomerAddressId} not found for update", id);
                return null;
            }

            // If this address is being set as primary, unset any existing primary address
            if (updateDto.IsPrimary && !address.IsPrimary)
            {
                await UnsetPrimaryAddressesAsync(address.CustomerId);
            }

            address.AddressTypeId = updateDto.AddressTypeId;
            address.PostalCodeId = updateDto.PostalCodeId;
            address.AddressLine1 = updateDto.AddressLine1;
            address.AddressLine2 = updateDto.AddressLine2;
            address.AddressLine3 = updateDto.AddressLine3;
            address.AddressLine4 = updateDto.AddressLine4;
            address.IsPrimary = updateDto.IsPrimary;
            address.IsActive = updateDto.IsActive;
            address.Notes = updateDto.Notes;

            await _context.SaveChangesAsync();

            // Reload to include AddressType and PostalCode
            var updatedAddress = await _context.CustomerAddresses
                .Include(ca => ca.AddressType)
                .Include(ca => ca.PostalCode)
                .FirstAsync(ca => ca.Id == id);

            _log.Information("Successfully updated customer address with ID: {CustomerAddressId}", id);
            return MapToDto(updatedAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating customer address with ID: {CustomerAddressId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a customer address from the database
    /// </summary>
    /// <param name="id">The customer address's unique identifier</param>
    /// <returns>True if the customer address was deleted; otherwise, false</returns>
    public async Task<bool> DeleteCustomerAddressAsync(int id)
    {
        try
        {
            _log.Information("Deleting customer address with ID: {CustomerAddressId}", id);

            var address = await _context.CustomerAddresses.FindAsync(id);

            if (address == null)
            {
                _log.Warning("Customer address with ID {CustomerAddressId} not found for deletion", id);
                return false;
            }

            _context.CustomerAddresses.Remove(address);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted customer address with ID: {CustomerAddressId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting customer address with ID: {CustomerAddressId}", id);
            throw;
        }
    }

    /// <summary>
    /// Sets a customer address as the primary address
    /// </summary>
    /// <param name="id">The customer address's unique identifier</param>
    /// <returns>The updated customer address if found; otherwise, null</returns>
    public async Task<CustomerAddressDto?> SetPrimaryAddressAsync(int id)
    {
        try
        {
            _log.Information("Setting customer address with ID {CustomerAddressId} as primary", id);

            var address = await _context.CustomerAddresses.FindAsync(id);

            if (address == null)
            {
                _log.Warning("Customer address with ID {CustomerAddressId} not found", id);
                return null;
            }

            // Unset any existing primary address for this customer
            await UnsetPrimaryAddressesAsync(address.CustomerId);

            // Set this address as primary
            address.IsPrimary = true;
            await _context.SaveChangesAsync();

            // Reload to include AddressType and PostalCode
            var updatedAddress = await _context.CustomerAddresses
                .Include(ca => ca.AddressType)
                .Include(ca => ca.PostalCode)
                .FirstAsync(ca => ca.Id == id);

            _log.Information("Successfully set customer address with ID {CustomerAddressId} as primary", id);
            return MapToDto(updatedAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while setting customer address with ID {CustomerAddressId} as primary", id);
            throw;
        }
    }

    private async Task UnsetPrimaryAddressesAsync(int customerId)
    {
        var primaryAddresses = await _context.CustomerAddresses
            .Where(ca => ca.CustomerId == customerId && ca.IsPrimary)
            .ToListAsync();

        foreach (var addr in primaryAddresses)
        {
            addr.IsPrimary = false;
        }
    }

    private static CustomerAddressDto MapToDto(CustomerAddress address)
    {
        return new CustomerAddressDto
        {
            Id = address.Id,
            CustomerId = address.CustomerId,
            AddressTypeId = address.AddressTypeId,
            AddressTypeCode = address.AddressType.Code,
            AddressTypeName = address.AddressType.Name,
            PostalCodeId = address.PostalCodeId,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            AddressLine3 = address.AddressLine3,
            AddressLine4 = address.AddressLine4,
            City = address.PostalCode.City,
            State = address.PostalCode.State,
            PostalCode = address.PostalCode.Code,
            CountryCode = address.PostalCode.CountryCode,
            IsPrimary = address.IsPrimary,
            IsActive = address.IsActive,
            Notes = address.Notes,
            CreatedAt = address.CreatedAt,
            UpdatedAt = address.UpdatedAt
        };
    }
}
