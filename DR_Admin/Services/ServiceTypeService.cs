using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class ServiceTypeService : IServiceTypeService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<ServiceTypeService>();

    public ServiceTypeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ServiceTypeDto>> GetAllServiceTypesAsync()
    {
        try
        {
            _log.Information("Fetching all service types");
            
            var serviceTypes = await _context.ServiceTypes
                .AsNoTracking()
                .ToListAsync();

            var serviceTypeDtos = serviceTypes.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} service types", serviceTypes.Count);
            return serviceTypeDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all service types");
            throw;
        }
    }

    public async Task<ServiceTypeDto?> GetServiceTypeByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching service type with ID: {ServiceTypeId}", id);
            
            var serviceType = await _context.ServiceTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(st => st.Id == id);

            if (serviceType == null)
            {
                _log.Warning("Service type with ID {ServiceTypeId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched service type with ID: {ServiceTypeId}", id);
            return MapToDto(serviceType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching service type with ID: {ServiceTypeId}", id);
            throw;
        }
    }

    public async Task<ServiceTypeDto?> GetServiceTypeByNameAsync(string name)
    {
        var normalizedName = StringNormalizationExtensions.Normalize(name);
        try
        {
            _log.Information("Fetching service type with name: {ServiceTypeName}", name);
            var serviceType = await _context.ServiceTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(st => st.Name == normalizedName);

            if (serviceType == null)
            {
                _log.Warning("Service type with name {ServiceTypeName} not found", normalizedName);
                return null;
            }

            _log.Information("Successfully fetched service type with name: {ServiceTypeName}", normalizedName);
            return MapToDto(serviceType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching service type with name: {ServiceTypeName}", normalizedName);
            throw;
        }
    }

    public async Task<ServiceTypeDto> CreateServiceTypeAsync(CreateServiceTypeDto createDto)
    {
        try
        {
            _log.Information("Creating new service type with name: {ServiceTypeName}", createDto.Name);

            var serviceType = new ServiceType
            {
                Name = StringNormalizationExtensions.Normalize(createDto.Name),
                Description = createDto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ServiceTypes.Add(serviceType);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created service type with ID: {ServiceTypeId}", serviceType.Id);
            return MapToDto(serviceType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating service type with name: {ServiceTypeName}", createDto.Name);
            throw;
        }
    }

    public async Task<ServiceTypeDto?> UpdateServiceTypeAsync(int id, UpdateServiceTypeDto updateDto)
    {
        try
        {
            _log.Information("Updating service type with ID: {ServiceTypeId}", id);

            var serviceType = await _context.ServiceTypes.FindAsync(id);

            if (serviceType == null)
            {
                _log.Warning("Service type with ID {ServiceTypeId} not found for update", id);
                return null;
            }

            serviceType.Name = StringNormalizationExtensions.Normalize(updateDto.Name);
            serviceType.Description = updateDto.Description;
            serviceType.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated service type with ID: {ServiceTypeId}", id);
            return MapToDto(serviceType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating service type with ID: {ServiceTypeId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteServiceTypeAsync(int id)
    {
        try
        {
            _log.Information("Deleting service type with ID: {ServiceTypeId}", id);

            var serviceType = await _context.ServiceTypes.FindAsync(id);

            if (serviceType == null)
            {
                _log.Warning("Service type with ID {ServiceTypeId} not found for deletion", id);
                return false;
            }

            _context.ServiceTypes.Remove(serviceType);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted service type with ID: {ServiceTypeId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting service type with ID: {ServiceTypeId}", id);
            throw;
        }
    }

    private static ServiceTypeDto MapToDto(ServiceType serviceType)
    {
        return new ServiceTypeDto
        {
            Id = serviceType.Id,
            Name = serviceType.Name,
            Description = serviceType.Description,
            CreatedAt = serviceType.CreatedAt,
            UpdatedAt = serviceType.UpdatedAt
        };
    }
}
