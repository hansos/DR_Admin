using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class DnsRecordTypeService : IDnsRecordTypeService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<DnsRecordTypeService>();

    public DnsRecordTypeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DnsRecordTypeDto>> GetAllDnsRecordTypesAsync()
    {
        try
        {
            _log.Information("Fetching all DNS record types");
            
            var dnsRecordTypes = await _context.DnsRecordTypes
                .AsNoTracking()
                .OrderBy(t => t.Type)
                .ToListAsync();

            var dnsRecordTypeDtos = dnsRecordTypes.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} DNS record types", dnsRecordTypes.Count);
            return dnsRecordTypeDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all DNS record types");
            throw;
        }
    }

    public async Task<IEnumerable<DnsRecordTypeDto>> GetActiveDnsRecordTypesAsync()
    {
        try
        {
            _log.Information("Fetching active DNS record types");
            
            var dnsRecordTypes = await _context.DnsRecordTypes
                .AsNoTracking()
                .Where(t => t.IsActive)
                .OrderBy(t => t.Type)
                .ToListAsync();

            var dnsRecordTypeDtos = dnsRecordTypes.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} active DNS record types", dnsRecordTypes.Count);
            return dnsRecordTypeDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active DNS record types");
            throw;
        }
    }

    public async Task<DnsRecordTypeDto?> GetDnsRecordTypeByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching DNS record type with ID: {DnsRecordTypeId}", id);
            
            var dnsRecordType = await _context.DnsRecordTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (dnsRecordType == null)
            {
                _log.Warning("DNS record type with ID {DnsRecordTypeId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched DNS record type with ID: {DnsRecordTypeId}", id);
            return MapToDto(dnsRecordType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching DNS record type with ID: {DnsRecordTypeId}", id);
            throw;
        }
    }

    public async Task<DnsRecordTypeDto?> GetDnsRecordTypeByTypeAsync(string type)
    {
        try
        {
            _log.Information("Fetching DNS record type: {Type}", type);
            
            var dnsRecordType = await _context.DnsRecordTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Type.ToUpper() == type.ToUpper());

            if (dnsRecordType == null)
            {
                _log.Warning("DNS record type {Type} not found", type);
                return null;
            }

            _log.Information("Successfully fetched DNS record type: {Type}", type);
            return MapToDto(dnsRecordType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching DNS record type: {Type}", type);
            throw;
        }
    }

    public async Task<DnsRecordTypeDto> CreateDnsRecordTypeAsync(CreateDnsRecordTypeDto createDto)
    {
        try
        {
            _log.Information("Creating new DNS record type: {Type}", createDto.Type);

            // Check if type already exists
            var existingType = await _context.DnsRecordTypes
                .FirstOrDefaultAsync(t => t.Type.ToUpper() == createDto.Type.ToUpper());

            if (existingType != null)
            {
                throw new InvalidOperationException($"DNS record type '{createDto.Type}' already exists");
            }

            var dnsRecordType = new DnsRecordType
            {
                Type = createDto.Type.ToUpper(),
                Description = createDto.Description,
                HasPriority = createDto.HasPriority,
                HasWeight = createDto.HasWeight,
                HasPort = createDto.HasPort,
                IsEditableByUser = createDto.IsEditableByUser,
                IsActive = createDto.IsActive,
                DefaultTTL = createDto.DefaultTTL,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.DnsRecordTypes.Add(dnsRecordType);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created DNS record type with ID: {DnsRecordTypeId}", dnsRecordType.Id);
            return MapToDto(dnsRecordType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating DNS record type");
            throw;
        }
    }

    public async Task<DnsRecordTypeDto?> UpdateDnsRecordTypeAsync(int id, UpdateDnsRecordTypeDto updateDto)
    {
        try
        {
            _log.Information("Updating DNS record type with ID: {DnsRecordTypeId}", id);

            var dnsRecordType = await _context.DnsRecordTypes.FindAsync(id);

            if (dnsRecordType == null)
            {
                _log.Warning("DNS record type with ID {DnsRecordTypeId} not found for update", id);
                return null;
            }

            // Check if new type name conflicts with existing type
            if (dnsRecordType.Type.ToUpper() != updateDto.Type.ToUpper())
            {
                var existingType = await _context.DnsRecordTypes
                    .FirstOrDefaultAsync(t => t.Type.ToUpper() == updateDto.Type.ToUpper() && t.Id != id);

                if (existingType != null)
                {
                    throw new InvalidOperationException($"DNS record type '{updateDto.Type}' already exists");
                }
            }

            dnsRecordType.Type = updateDto.Type.ToUpper();
            dnsRecordType.Description = updateDto.Description;
            dnsRecordType.HasPriority = updateDto.HasPriority;
            dnsRecordType.HasWeight = updateDto.HasWeight;
            dnsRecordType.HasPort = updateDto.HasPort;
            dnsRecordType.IsEditableByUser = updateDto.IsEditableByUser;
            dnsRecordType.IsActive = updateDto.IsActive;
            dnsRecordType.DefaultTTL = updateDto.DefaultTTL;
            dnsRecordType.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated DNS record type with ID: {DnsRecordTypeId}", id);
            return MapToDto(dnsRecordType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating DNS record type with ID: {DnsRecordTypeId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteDnsRecordTypeAsync(int id)
    {
        try
        {
            _log.Information("Deleting DNS record type with ID: {DnsRecordTypeId}", id);

            var dnsRecordType = await _context.DnsRecordTypes
                .Include(t => t.DnsRecords)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (dnsRecordType == null)
            {
                _log.Warning("DNS record type with ID {DnsRecordTypeId} not found for deletion", id);
                return false;
            }

            // Check if there are DNS records using this type
            if (dnsRecordType.DnsRecords.Any())
            {
                throw new InvalidOperationException($"Cannot delete DNS record type '{dnsRecordType.Type}' because it is being used by {dnsRecordType.DnsRecords.Count} DNS record(s)");
            }

            _context.DnsRecordTypes.Remove(dnsRecordType);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted DNS record type with ID: {DnsRecordTypeId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting DNS record type with ID: {DnsRecordTypeId}", id);
            throw;
        }
    }

    private static DnsRecordTypeDto MapToDto(DnsRecordType dnsRecordType)
    {
        return new DnsRecordTypeDto
        {
            Id = dnsRecordType.Id,
            Type = dnsRecordType.Type,
            Description = dnsRecordType.Description,
            HasPriority = dnsRecordType.HasPriority,
            HasWeight = dnsRecordType.HasWeight,
            HasPort = dnsRecordType.HasPort,
            IsEditableByUser = dnsRecordType.IsEditableByUser,
            IsActive = dnsRecordType.IsActive,
            DefaultTTL = dnsRecordType.DefaultTTL,
            CreatedAt = dnsRecordType.CreatedAt,
            UpdatedAt = dnsRecordType.UpdatedAt
        };
    }
}
