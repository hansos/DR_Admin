using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class DnsRecordService : IDnsRecordService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<DnsRecordService>();

    public DnsRecordService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DnsRecordDto>> GetAllDnsRecordsAsync()
    {
        try
        {
            _log.Information("Fetching all DNS records");
            
            var dnsRecords = await _context.DnsRecords
                .AsNoTracking()
                .Include(d => d.Domain)
                .ToListAsync();

            var dnsRecordDtos = dnsRecords.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} DNS records", dnsRecords.Count);
            return dnsRecordDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all DNS records");
            throw;
        }
    }

    public async Task<DnsRecordDto?> GetDnsRecordByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching DNS record with ID: {DnsRecordId}", id);
            
            var dnsRecord = await _context.DnsRecords
                .AsNoTracking()
                .Include(d => d.Domain)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dnsRecord == null)
            {
                _log.Warning("DNS record with ID {DnsRecordId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched DNS record with ID: {DnsRecordId}", id);
            return MapToDto(dnsRecord);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching DNS record with ID: {DnsRecordId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<DnsRecordDto>> GetDnsRecordsByDomainIdAsync(int domainId)
    {
        try
        {
            _log.Information("Fetching DNS records for domain ID: {DomainId}", domainId);
            
            var dnsRecords = await _context.DnsRecords
                .AsNoTracking()
                .Include(d => d.Domain)
                .Where(d => d.DomainId == domainId)
                .ToListAsync();

            var dnsRecordDtos = dnsRecords.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} DNS records for domain ID: {DomainId}", dnsRecords.Count, domainId);
            return dnsRecordDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching DNS records for domain ID: {DomainId}", domainId);
            throw;
        }
    }

    public async Task<IEnumerable<DnsRecordDto>> GetDnsRecordsByTypeAsync(string type)
    {
        try
        {
            _log.Information("Fetching DNS records with type: {Type}", type);
            
            var dnsRecords = await _context.DnsRecords
                .AsNoTracking()
                .Include(d => d.Domain)
                .Where(d => d.Type.ToUpper() == type.ToUpper())
                .ToListAsync();

            var dnsRecordDtos = dnsRecords.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} DNS records with type: {Type}", dnsRecords.Count, type);
            return dnsRecordDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching DNS records with type: {Type}", type);
            throw;
        }
    }

    public async Task<DnsRecordDto> CreateDnsRecordAsync(CreateDnsRecordDto createDto)
    {
        try
        {
            _log.Information("Creating new DNS record of type {Type} for domain ID: {DomainId}", createDto.Type, createDto.DomainId);

            // Validate that the domain exists
            var domainExists = await _context.Domains.AnyAsync(d => d.Id == createDto.DomainId);
            if (!domainExists)
            {
                throw new InvalidOperationException($"Domain with ID {createDto.DomainId} not found");
            }

            var dnsRecord = new DnsRecord
            {
                DomainId = createDto.DomainId,
                Type = createDto.Type.ToUpper(),
                Name = createDto.Name,
                Value = createDto.Value,
                TTL = createDto.TTL,
                Priority = createDto.Priority,
                Weight = createDto.Weight,
                Port = createDto.Port,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.DnsRecords.Add(dnsRecord);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created DNS record with ID: {DnsRecordId}", dnsRecord.Id);

            // Fetch the created record with domain navigation property
            var createdRecord = await _context.DnsRecords
                .AsNoTracking()
                .Include(d => d.Domain)
                .FirstOrDefaultAsync(d => d.Id == dnsRecord.Id);

            return MapToDto(createdRecord!);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating DNS record");
            throw;
        }
    }

    public async Task<DnsRecordDto?> UpdateDnsRecordAsync(int id, UpdateDnsRecordDto updateDto)
    {
        try
        {
            _log.Information("Updating DNS record with ID: {DnsRecordId}", id);

            var dnsRecord = await _context.DnsRecords
                .Include(d => d.Domain)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dnsRecord == null)
            {
                _log.Warning("DNS record with ID {DnsRecordId} not found for update", id);
                return null;
            }

            // Validate that the domain exists if domain ID is being changed
            if (dnsRecord.DomainId != updateDto.DomainId)
            {
                var domainExists = await _context.Domains.AnyAsync(d => d.Id == updateDto.DomainId);
                if (!domainExists)
                {
                    throw new InvalidOperationException($"Domain with ID {updateDto.DomainId} not found");
                }
            }

            dnsRecord.DomainId = updateDto.DomainId;
            dnsRecord.Type = updateDto.Type.ToUpper();
            dnsRecord.Name = updateDto.Name;
            dnsRecord.Value = updateDto.Value;
            dnsRecord.TTL = updateDto.TTL;
            dnsRecord.Priority = updateDto.Priority;
            dnsRecord.Weight = updateDto.Weight;
            dnsRecord.Port = updateDto.Port;
            dnsRecord.IsEditableByUser = updateDto.IsEditableByUser;
            dnsRecord.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated DNS record with ID: {DnsRecordId}", id);
            return MapToDto(dnsRecord);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating DNS record with ID: {DnsRecordId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteDnsRecordAsync(int id)
    {
        try
        {
            _log.Information("Deleting DNS record with ID: {DnsRecordId}", id);

            var dnsRecord = await _context.DnsRecords.FindAsync(id);

            if (dnsRecord == null)
            {
                _log.Warning("DNS record with ID {DnsRecordId} not found for deletion", id);
                return false;
            }

            _context.DnsRecords.Remove(dnsRecord);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted DNS record with ID: {DnsRecordId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting DNS record with ID: {DnsRecordId}", id);
            throw;
        }
    }

    private static DnsRecordDto MapToDto(DnsRecord dnsRecord)
    {
        return new DnsRecordDto
        {
            Id = dnsRecord.Id,
            DomainId = dnsRecord.DomainId,
            Type = dnsRecord.Type,
            Name = dnsRecord.Name,
            Value = dnsRecord.Value,
            TTL = dnsRecord.TTL,
            Priority = dnsRecord.Priority,
            Weight = dnsRecord.Weight,
            Port = dnsRecord.Port,
            CreatedAt = dnsRecord.CreatedAt,
            UpdatedAt = dnsRecord.UpdatedAt
        };
    }
}
