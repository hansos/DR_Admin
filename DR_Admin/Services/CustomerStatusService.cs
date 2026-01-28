using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing customer statuses
/// </summary>
public class CustomerStatusService : ICustomerStatusService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<CustomerStatusService>();

    public CustomerStatusService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all customer statuses in the system
    /// </summary>
    /// <returns>A collection of customer status DTOs</returns>
    public async Task<IEnumerable<CustomerStatusDto>> GetAllCustomerStatusesAsync()
    {
        try
        {
            _log.Information("Fetching all customer statuses");
            
            var customerStatuses = await _context.CustomerStatuses
                .AsNoTracking()
                .OrderBy(cs => cs.SortOrder)
                .ThenBy(cs => cs.Name)
                .ToListAsync();

            var customerStatusDtos = customerStatuses.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} customer statuses", customerStatuses.Count);
            return customerStatusDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all customer statuses");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all active customer statuses
    /// </summary>
    /// <returns>A collection of active customer status DTOs</returns>
    public async Task<IEnumerable<CustomerStatusDto>> GetActiveCustomerStatusesAsync()
    {
        try
        {
            _log.Information("Fetching active customer statuses");
            
            var customerStatuses = await _context.CustomerStatuses
                .AsNoTracking()
                .Where(cs => cs.IsActive)
                .OrderBy(cs => cs.SortOrder)
                .ThenBy(cs => cs.Name)
                .ToListAsync();

            var customerStatusDtos = customerStatuses.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} active customer statuses", customerStatuses.Count);
            return customerStatusDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active customer statuses");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific customer status by its unique identifier
    /// </summary>
    /// <param name="id">The customer status ID</param>
    /// <returns>The customer status DTO if found, otherwise null</returns>
    public async Task<CustomerStatusDto?> GetCustomerStatusByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching customer status with ID: {CustomerStatusId}", id);
            
            var customerStatus = await _context.CustomerStatuses
                .AsNoTracking()
                .FirstOrDefaultAsync(cs => cs.Id == id);

            if (customerStatus == null)
            {
                _log.Warning("Customer status with ID {CustomerStatusId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched customer status with ID: {CustomerStatusId}", id);
            return MapToDto(customerStatus);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching customer status with ID: {CustomerStatusId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific customer status by its code
    /// </summary>
    /// <param name="code">The customer status code</param>
    /// <returns>The customer status DTO if found, otherwise null</returns>
    public async Task<CustomerStatusDto?> GetCustomerStatusByCodeAsync(string code)
    {
        try
        {
            _log.Information("Fetching customer status with code: {Code}", code);
            
            var customerStatus = await _context.CustomerStatuses
                .AsNoTracking()
                .FirstOrDefaultAsync(cs => cs.Code == code);

            if (customerStatus == null)
            {
                _log.Warning("Customer status with code {Code} not found", code);
                return null;
            }

            _log.Information("Successfully fetched customer status with code: {Code}", code);
            return MapToDto(customerStatus);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching customer status with code: {Code}", code);
            throw;
        }
    }

    /// <summary>
    /// Retrieves the default customer status
    /// </summary>
    /// <returns>The default customer status DTO if found, otherwise null</returns>
    public async Task<CustomerStatusDto?> GetDefaultCustomerStatusAsync()
    {
        try
        {
            _log.Information("Fetching default customer status");
            
            var customerStatus = await _context.CustomerStatuses
                .AsNoTracking()
                .FirstOrDefaultAsync(cs => cs.IsDefault && cs.IsActive);

            if (customerStatus == null)
            {
                _log.Warning("No default customer status found");
                return null;
            }

            _log.Information("Successfully fetched default customer status");
            return MapToDto(customerStatus);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching default customer status");
            throw;
        }
    }

    /// <summary>
    /// Creates a new customer status
    /// </summary>
    /// <param name="createDto">The customer status data for creation</param>
    /// <returns>The newly created customer status DTO</returns>
    public async Task<CustomerStatusDto> CreateCustomerStatusAsync(CreateCustomerStatusDto createDto)
    {
        try
        {
            _log.Information("Creating new customer status with code: {Code}", createDto.Code);

            // Check if code already exists
            var existingStatus = await _context.CustomerStatuses
                .FirstOrDefaultAsync(cs => cs.Code == createDto.Code);

            if (existingStatus != null)
            {
                _log.Warning("Customer status with code {Code} already exists", createDto.Code);
                throw new InvalidOperationException($"Customer status with code '{createDto.Code}' already exists");
            }

            // If this is set as default, unset other defaults
            if (createDto.IsDefault)
            {
                await UnsetAllDefaultsAsync();
            }

            var customerStatus = new CustomerStatus
            {
                Code = createDto.Code,
                Name = createDto.Name,
                Description = createDto.Description,
                Color = createDto.Color,
                IsActive = createDto.IsActive,
                IsDefault = createDto.IsDefault,
                SortOrder = createDto.SortOrder
            };

            _context.CustomerStatuses.Add(customerStatus);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created customer status with ID: {CustomerStatusId}", customerStatus.Id);
            return MapToDto(customerStatus);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating customer status");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing customer status
    /// </summary>
    /// <param name="id">The customer status ID to update</param>
    /// <param name="updateDto">The updated customer status data</param>
    /// <returns>The updated customer status DTO if successful, otherwise null</returns>
    public async Task<CustomerStatusDto?> UpdateCustomerStatusAsync(int id, UpdateCustomerStatusDto updateDto)
    {
        try
        {
            _log.Information("Updating customer status with ID: {CustomerStatusId}", id);

            var customerStatus = await _context.CustomerStatuses
                .FirstOrDefaultAsync(cs => cs.Id == id);

            if (customerStatus == null)
            {
                _log.Warning("Customer status with ID {CustomerStatusId} not found", id);
                return null;
            }

            // If this is set as default, unset other defaults
            if (updateDto.IsDefault && !customerStatus.IsDefault)
            {
                await UnsetAllDefaultsAsync();
            }

            customerStatus.Name = updateDto.Name;
            customerStatus.Description = updateDto.Description;
            customerStatus.Color = updateDto.Color;
            customerStatus.IsActive = updateDto.IsActive;
            customerStatus.IsDefault = updateDto.IsDefault;
            customerStatus.SortOrder = updateDto.SortOrder;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated customer status with ID: {CustomerStatusId}", id);
            return MapToDto(customerStatus);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating customer status with ID: {CustomerStatusId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a customer status
    /// </summary>
    /// <param name="id">The customer status ID to delete</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    public async Task<bool> DeleteCustomerStatusAsync(int id)
    {
        try
        {
            _log.Information("Deleting customer status with ID: {CustomerStatusId}", id);

            var customerStatus = await _context.CustomerStatuses
                .FirstOrDefaultAsync(cs => cs.Id == id);

            if (customerStatus == null)
            {
                _log.Warning("Customer status with ID {CustomerStatusId} not found", id);
                return false;
            }

            // Check if any customers are using this status
            var customersWithStatus = await _context.Customers
                .AnyAsync(c => c.CustomerStatusId == id);

            if (customersWithStatus)
            {
                _log.Warning("Cannot delete customer status with ID {CustomerStatusId} - it is in use by customers", id);
                throw new InvalidOperationException("Cannot delete customer status that is in use by customers");
            }

            _context.CustomerStatuses.Remove(customerStatus);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted customer status with ID: {CustomerStatusId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting customer status with ID: {CustomerStatusId}", id);
            throw;
        }
    }

    /// <summary>
    /// Unsets the IsDefault flag on all customer statuses
    /// </summary>
    private async Task UnsetAllDefaultsAsync()
    {
        var defaultStatuses = await _context.CustomerStatuses
            .Where(cs => cs.IsDefault)
            .ToListAsync();

        foreach (var status in defaultStatuses)
        {
            status.IsDefault = false;
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Maps a CustomerStatus entity to a CustomerStatusDto
    /// </summary>
    private static CustomerStatusDto MapToDto(CustomerStatus customerStatus)
    {
        return new CustomerStatusDto
        {
            Id = customerStatus.Id,
            Code = customerStatus.Code,
            Name = customerStatus.Name,
            Description = customerStatus.Description,
            Color = customerStatus.Color,
            IsActive = customerStatus.IsActive,
            IsDefault = customerStatus.IsDefault,
            SortOrder = customerStatus.SortOrder,
            CreatedAt = customerStatus.CreatedAt,
            UpdatedAt = customerStatus.UpdatedAt
        };
    }
}
