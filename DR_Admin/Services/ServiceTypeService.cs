using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class ServiceTypeService : IServiceTypeService
{
    private readonly ApplicationDbContext _context;
    private readonly Serilog.ILogger _logger;

    public ServiceTypeService(ApplicationDbContext context, Serilog.ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ServiceTypeDto>> GetAllServiceTypesAsync()
    {
        try
        {
            _logger.Information("Fetching all service types");
            
            var serviceTypes = await _context.ServiceTypes
                .AsNoTracking()
                .ToListAsync();

            var serviceTypeDtos = serviceTypes.Select(MapToDto);
            
            _logger.Information("Successfully fetched {Count} service types", serviceTypes.Count);
            return serviceTypeDtos;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching all service types");
            throw;
        }
    }

    public async Task<ServiceTypeDto?> GetServiceTypeByIdAsync(int id)
    {
        try
        {
            _logger.Information("Fetching service type with ID: {ServiceTypeId}", id);
            
            var serviceType = await _context.ServiceTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(st => st.Id == id);

            if (serviceType == null)
            {
                _logger.Warning("Service type with ID {ServiceTypeId} not found", id);
                return null;
            }

            _logger.Information("Successfully fetched service type with ID: {ServiceTypeId}", id);
            return MapToDto(serviceType);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching service type with ID: {ServiceTypeId}", id);
            throw;
        }
    }

    public async Task<ServiceTypeDto> CreateServiceTypeAsync(CreateServiceTypeDto createDto)
    {
        try
        {
            _logger.Information("Creating new service type with name: {ServiceTypeName}", createDto.Name);

            var serviceType = new ServiceType
            {
                Name = createDto.Name,
                Description = createDto.Description
            };

            _context.ServiceTypes.Add(serviceType);
            await _context.SaveChangesAsync();

            _logger.Information("Successfully created service type with ID: {ServiceTypeId}", serviceType.Id);
            return MapToDto(serviceType);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while creating service type with name: {ServiceTypeName}", createDto.Name);
            throw;
        }
    }

    public async Task<ServiceTypeDto?> UpdateServiceTypeAsync(int id, UpdateServiceTypeDto updateDto)
    {
        try
        {
            _logger.Information("Updating service type with ID: {ServiceTypeId}", id);

            var serviceType = await _context.ServiceTypes.FindAsync(id);

            if (serviceType == null)
            {
                _logger.Warning("Service type with ID {ServiceTypeId} not found for update", id);
                return null;
            }

            serviceType.Name = updateDto.Name;
            serviceType.Description = updateDto.Description;

            await _context.SaveChangesAsync();

            _logger.Information("Successfully updated service type with ID: {ServiceTypeId}", id);
            return MapToDto(serviceType);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while updating service type with ID: {ServiceTypeId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteServiceTypeAsync(int id)
    {
        try
        {
            _logger.Information("Deleting service type with ID: {ServiceTypeId}", id);

            var serviceType = await _context.ServiceTypes.FindAsync(id);

            if (serviceType == null)
            {
                _logger.Warning("Service type with ID {ServiceTypeId} not found for deletion", id);
                return false;
            }

            _context.ServiceTypes.Remove(serviceType);
            await _context.SaveChangesAsync();

            _logger.Information("Successfully deleted service type with ID: {ServiceTypeId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while deleting service type with ID: {ServiceTypeId}", id);
            throw;
        }
    }

    private static ServiceTypeDto MapToDto(ServiceType serviceType)
    {
        return new ServiceTypeDto
        {
            Id = serviceType.Id,
            Name = serviceType.Name,
            Description = serviceType.Description
        };
    }
}
