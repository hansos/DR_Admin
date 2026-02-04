using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing address type operations
/// </summary>
public class AddressTypeService : IAddressTypeService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<AddressTypeService>();

    public AddressTypeService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all address types from the database
    /// </summary>
    /// <returns>A collection of all address types</returns>
    public async Task<IEnumerable<AddressTypeDto>> GetAllAddressTypesAsync()
    {
        try
        {
            _log.Information("Fetching all address types");
            
            var addressTypes = await _context.AddressTypes
                .AsNoTracking()
                .OrderBy(at => at.SortOrder)
                .ThenBy(at => at.Name)
                .ToListAsync();

            var addressTypeDtos = addressTypes.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} address types", addressTypes.Count);
            return addressTypeDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all address types");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a paginated list of address types
    /// </summary>
    /// <param name="parameters">Pagination parameters including page number and page size</param>
    /// <returns>A paged result containing address types and pagination metadata</returns>
    public async Task<PagedResult<AddressTypeDto>> GetAllAddressTypesPagedAsync(PaginationParameters parameters)
    {
        try
        {
            _log.Information("Fetching paginated address types - Page: {PageNumber}, PageSize: {PageSize}", 
                parameters.PageNumber, parameters.PageSize);
            
            var totalCount = await _context.AddressTypes
                .AsNoTracking()
                .CountAsync();

            var addressTypes = await _context.AddressTypes
                .AsNoTracking()
                .OrderBy(at => at.SortOrder)
                .ThenBy(at => at.Name)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var addressTypeDtos = addressTypes.Select(MapToDto).ToList();
            
            var result = new PagedResult<AddressTypeDto>(
                addressTypeDtos, 
                totalCount, 
                parameters.PageNumber, 
                parameters.PageSize);

            _log.Information("Successfully fetched page {PageNumber} of address types - Returned {Count} of {TotalCount} total", 
                parameters.PageNumber, addressTypeDtos.Count, totalCount);
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching paginated address types");
            throw;
        }
    }

    /// <summary>
    /// Retrieves an address type by its unique identifier
    /// </summary>
    /// <param name="id">The address type's unique identifier</param>
    /// <returns>The address type if found; otherwise, null</returns>
    public async Task<AddressTypeDto?> GetAddressTypeByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching address type with ID: {AddressTypeId}", id);
            
            var addressType = await _context.AddressTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(at => at.Id == id);

            if (addressType == null)
            {
                _log.Warning("Address type with ID {AddressTypeId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched address type with ID: {AddressTypeId}", id);
            return MapToDto(addressType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching address type with ID: {AddressTypeId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves an address type by its code
    /// </summary>
    /// <param name="code">The address type code to search for</param>
    /// <returns>The address type if found; otherwise, null</returns>
    public async Task<AddressTypeDto?> GetAddressTypeByCodeAsync(string code)
    {
        try
        {
            _log.Information("Fetching address type with code: {Code}", code);
            
            var addressType = await _context.AddressTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(at => at.Code == code);

            if (addressType == null)
            {
                _log.Warning("Address type with code {Code} not found", code);
                return null;
            }

            _log.Information("Successfully fetched address type with code: {Code}", code);
            return MapToDto(addressType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching address type with code: {Code}", code);
            throw;
        }
    }

    /// <summary>
    /// Creates a new address type
    /// </summary>
    /// <param name="createDto">The address type data for creation</param>
    /// <returns>The newly created address type</returns>
    public async Task<AddressTypeDto> CreateAddressTypeAsync(CreateAddressTypeDto createDto)
    {
        try
        {
            _log.Information("Creating new address type with code: {Code}", createDto.Code);

            var addressType = new AddressType
            {
                Code = createDto.Code,
                Name = createDto.Name,
                Description = createDto.Description,
                IsActive = createDto.IsActive,
                IsDefault = createDto.IsDefault,
                SortOrder = createDto.SortOrder
            };

            _context.AddressTypes.Add(addressType);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created address type with ID: {AddressTypeId}", addressType.Id);
            return MapToDto(addressType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating address type with code: {Code}", createDto.Code);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing address type
    /// </summary>
    /// <param name="id">The address type's unique identifier</param>
    /// <param name="updateDto">The updated address type data</param>
    /// <returns>The updated address type if found; otherwise, null</returns>
    public async Task<AddressTypeDto?> UpdateAddressTypeAsync(int id, UpdateAddressTypeDto updateDto)
    {
        try
        {
            _log.Information("Updating address type with ID: {AddressTypeId}", id);

            var addressType = await _context.AddressTypes.FindAsync(id);

            if (addressType == null)
            {
                _log.Warning("Address type with ID {AddressTypeId} not found for update", id);
                return null;
            }

            addressType.Code = updateDto.Code;
            addressType.Name = updateDto.Name;
            addressType.Description = updateDto.Description;
            addressType.IsActive = updateDto.IsActive;
            addressType.IsDefault = updateDto.IsDefault;
            addressType.SortOrder = updateDto.SortOrder;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated address type with ID: {AddressTypeId}", id);
            return MapToDto(addressType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating address type with ID: {AddressTypeId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes an address type from the database
    /// </summary>
    /// <param name="id">The address type's unique identifier</param>
    /// <returns>True if the address type was deleted; otherwise, false</returns>
    public async Task<bool> DeleteAddressTypeAsync(int id)
    {
        try
        {
            _log.Information("Deleting address type with ID: {AddressTypeId}", id);

            var addressType = await _context.AddressTypes.FindAsync(id);

            if (addressType == null)
            {
                _log.Warning("Address type with ID {AddressTypeId} not found for deletion", id);
                return false;
            }

            _context.AddressTypes.Remove(addressType);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted address type with ID: {AddressTypeId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting address type with ID: {AddressTypeId}", id);
            throw;
        }
    }

    private static AddressTypeDto MapToDto(AddressType addressType)
    {
        return new AddressTypeDto
        {
            Id = addressType.Id,
            Code = addressType.Code,
            Name = addressType.Name,
            Description = addressType.Description,
            IsActive = addressType.IsActive,
            IsDefault = addressType.IsDefault,
            SortOrder = addressType.SortOrder,
            CreatedAt = addressType.CreatedAt,
            UpdatedAt = addressType.UpdatedAt
        };
    }
}
