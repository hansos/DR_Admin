using ISPAdmin.Data;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ServiceEntity = ISPAdmin.Data.Entities.Service;

namespace ISPAdmin.Services;

public class ServiceService : IServiceService
{
    private readonly ApplicationDbContext _context;
    private readonly Serilog.ILogger _logger;

    public ServiceService(ApplicationDbContext context, Serilog.ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync()
    {
        try
        {
            _logger.Information("Fetching all services");
            
            var services = await _context.Services
                .AsNoTracking()
                .ToListAsync();

            var serviceDtos = services.Select(MapToDto);
            
            _logger.Information("Successfully fetched {Count} services", services.Count);
            return serviceDtos;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching all services");
            throw;
        }
    }

    public async Task<ServiceDto?> GetServiceByIdAsync(int id)
    {
        try
        {
            _logger.Information("Fetching service with ID: {ServiceId}", id);
            
            var service = await _context.Services
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
            {
                _logger.Warning("Service with ID {ServiceId} not found", id);
                return null;
            }

            _logger.Information("Successfully fetched service with ID: {ServiceId}", id);
            return MapToDto(service);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching service with ID: {ServiceId}", id);
            throw;
        }
    }

    public async Task<ServiceDto> CreateServiceAsync(CreateServiceDto createDto)
    {
        try
        {
            _logger.Information("Creating new service with name: {ServiceName}", createDto.Name);

            var service = new ServiceEntity
            {
                Name = createDto.Name,
                Description = createDto.Description,
                ServiceTypeId = createDto.ServiceTypeId,
                BillingCycleId = createDto.BillingCycleId,
                Price = createDto.Price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            _logger.Information("Successfully created service with ID: {ServiceId}", service.Id);
            return MapToDto(service);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while creating service with name: {ServiceName}", createDto.Name);
            throw;
        }
    }

    public async Task<ServiceDto?> UpdateServiceAsync(int id, UpdateServiceDto updateDto)
    {
        try
        {
            _logger.Information("Updating service with ID: {ServiceId}", id);

            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                _logger.Warning("Service with ID {ServiceId} not found for update", id);
                return null;
            }

            service.Name = updateDto.Name;
            service.Description = updateDto.Description;
            service.ServiceTypeId = updateDto.ServiceTypeId;
            service.BillingCycleId = updateDto.BillingCycleId;
            service.Price = updateDto.Price;
            service.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.Information("Successfully updated service with ID: {ServiceId}", id);
            return MapToDto(service);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while updating service with ID: {ServiceId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteServiceAsync(int id)
    {
        try
        {
            _logger.Information("Deleting service with ID: {ServiceId}", id);

            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                _logger.Warning("Service with ID {ServiceId} not found for deletion", id);
                return false;
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            _logger.Information("Successfully deleted service with ID: {ServiceId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while deleting service with ID: {ServiceId}", id);
            throw;
        }
    }

    private static ServiceDto MapToDto(ServiceEntity service)
    {
        return new ServiceDto
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            ServiceTypeId = service.ServiceTypeId,
            BillingCycleId = service.BillingCycleId,
            Price = service.Price,
            CreatedAt = service.CreatedAt,
            UpdatedAt = service.UpdatedAt
        };
    }
}
