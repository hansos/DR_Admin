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

    /// <inheritdoc/>
    public async Task<IEnumerable<DnsRecordDto>> GetAllDnsRecordsAsync()
    {
        try
        {
            _log.Information("Fetching all DNS records");

            var dnsRecords = await _context.DnsRecords
                .AsNoTracking()
                .Include(d => d.Domain)
                .Include(d => d.DnsRecordType)
                .Where(d => !d.IsDeleted)
                .ToListAsync();

            _log.Information("Successfully fetched {Count} DNS records", dnsRecords.Count);
            return dnsRecords.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all DNS records");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<PagedResult<DnsRecordDto>> GetAllDnsRecordsPagedAsync(PaginationParameters parameters)
    {
        try
        {
            _log.Information("Fetching paginated DNS records - Page: {PageNumber}, PageSize: {PageSize}",
                parameters.PageNumber, parameters.PageSize);

            var query = _context.DnsRecords
                .AsNoTracking()
                .Where(d => !d.IsDeleted);

            var totalCount = await query.CountAsync();

            var dnsRecords = await query
                .Include(d => d.Domain)
                .Include(d => d.DnsRecordType)
                .OrderBy(d => d.Name)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var dnsRecordDtos = dnsRecords.Select(MapToDto).ToList();

            var result = new PagedResult<DnsRecordDto>(
                dnsRecordDtos,
                totalCount,
                parameters.PageNumber,
                parameters.PageSize);

            _log.Information("Successfully fetched page {PageNumber} of DNS records - Returned {Count} of {TotalCount} total",
                parameters.PageNumber, dnsRecordDtos.Count, totalCount);

            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching paginated DNS records");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<DnsRecordDto?> GetDnsRecordByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching DNS record with ID: {DnsRecordId}", id);

            var dnsRecord = await _context.DnsRecords
                .AsNoTracking()
                .Include(d => d.Domain)
                .Include(d => d.DnsRecordType)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

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

    /// <inheritdoc/>
    public async Task<IEnumerable<DnsRecordDto>> GetDnsRecordsByDomainIdAsync(int domainId)
    {
        try
        {
            _log.Information("Fetching DNS records for domain ID: {DomainId}", domainId);

            var dnsRecords = await _context.DnsRecords
                .AsNoTracking()
                .Include(d => d.Domain)
                .Include(d => d.DnsRecordType)
                .Where(d => d.DomainId == domainId && !d.IsDeleted)
                .ToListAsync();

            _log.Information("Successfully fetched {Count} DNS records for domain ID: {DomainId}", dnsRecords.Count, domainId);
            return dnsRecords.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching DNS records for domain ID: {DomainId}", domainId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DnsRecordDto>> GetDnsRecordsByTypeAsync(string type)
    {
        try
        {
            _log.Information("Fetching DNS records with type: {Type}", type);

            var dnsRecords = await _context.DnsRecords
                .AsNoTracking()
                .Include(d => d.Domain)
                .Include(d => d.DnsRecordType)
                .Where(d => !d.IsDeleted && d.DnsRecordType.Type.ToUpper() == type.ToUpper())
                .ToListAsync();

            _log.Information("Successfully fetched {Count} DNS records with type: {Type}", dnsRecords.Count, type);
            return dnsRecords.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching DNS records with type: {Type}", type);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DnsRecordDto>> GetPendingSyncRecordsAsync(int domainId)
    {
        try
        {
            _log.Information("Fetching pending-sync DNS records for domain ID: {DomainId}", domainId);

            var dnsRecords = await _context.DnsRecords
                .AsNoTracking()
                .Include(d => d.Domain)
                .Include(d => d.DnsRecordType)
                .Where(d => d.DomainId == domainId && d.IsPendingSync)
                .ToListAsync();

            _log.Information("Successfully fetched {Count} pending-sync DNS records for domain ID: {DomainId}", dnsRecords.Count, domainId);
            return dnsRecords.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching pending-sync DNS records for domain ID: {DomainId}", domainId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DnsRecordDto>> GetDeletedDnsRecordsAsync(int domainId)
    {
        try
        {
            _log.Information("Fetching soft-deleted DNS records for domain ID: {DomainId}", domainId);

            var dnsRecords = await _context.DnsRecords
                .AsNoTracking()
                .Include(d => d.Domain)
                .Include(d => d.DnsRecordType)
                .Where(d => d.DomainId == domainId && d.IsDeleted)
                .ToListAsync();

            _log.Information("Successfully fetched {Count} soft-deleted DNS records for domain ID: {DomainId}", dnsRecords.Count, domainId);
            return dnsRecords.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching soft-deleted DNS records for domain ID: {DomainId}", domainId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<DnsRecordDto> CreateDnsRecordAsync(CreateDnsRecordDto createDto)
    {
        try
        {
            _log.Information("Creating new DNS record of type ID {DnsRecordTypeId} for domain ID: {DomainId}", createDto.DnsRecordTypeId, createDto.DomainId);

            var domainExists = await _context.RegisteredDomains.AnyAsync(d => d.Id == createDto.DomainId);
            if (!domainExists)
                throw new InvalidOperationException($"Domain with ID {createDto.DomainId} not found");

            var dnsRecordType = await _context.DnsRecordTypes.FindAsync(createDto.DnsRecordTypeId);
            if (dnsRecordType == null)
                throw new InvalidOperationException($"DNS record type with ID {createDto.DnsRecordTypeId} not found");

            if (!dnsRecordType.IsActive)
                throw new InvalidOperationException($"DNS record type '{dnsRecordType.Type}' is not active");

            if (dnsRecordType.HasPriority && !createDto.Priority.HasValue)
                throw new InvalidOperationException($"Priority is required for DNS record type '{dnsRecordType.Type}'");

            if (dnsRecordType.HasWeight && !createDto.Weight.HasValue)
                throw new InvalidOperationException($"Weight is required for DNS record type '{dnsRecordType.Type}'");

            if (dnsRecordType.HasPort && !createDto.Port.HasValue)
                throw new InvalidOperationException($"Port is required for DNS record type '{dnsRecordType.Type}'");

            var dnsRecord = new DnsRecord
            {
                DomainId = createDto.DomainId,
                DnsRecordTypeId = createDto.DnsRecordTypeId,
                Name = createDto.Name,
                Value = createDto.Value,
                TTL = createDto.TTL > 0 ? createDto.TTL : dnsRecordType.DefaultTTL,
                Priority = createDto.Priority,
                Weight = createDto.Weight,
                Port = createDto.Port,
                IsPendingSync = createDto.IsPendingSync,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.DnsRecords.Add(dnsRecord);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created DNS record with ID: {DnsRecordId}", dnsRecord.Id);

            var createdRecord = await _context.DnsRecords
                .AsNoTracking()
                .Include(d => d.Domain)
                .Include(d => d.DnsRecordType)
                .FirstOrDefaultAsync(d => d.Id == dnsRecord.Id);

            return MapToDto(createdRecord!);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating DNS record");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<DnsRecordDto?> UpdateDnsRecordAsync(int id, UpdateDnsRecordDto updateDto)
    {
        try
        {
            _log.Information("Updating DNS record with ID: {DnsRecordId}", id);

            var dnsRecord = await _context.DnsRecords
                .Include(d => d.Domain)
                .Include(d => d.DnsRecordType)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (dnsRecord == null)
            {
                _log.Warning("DNS record with ID {DnsRecordId} not found for update", id);
                return null;
            }

            if (dnsRecord.DomainId != updateDto.DomainId)
            {
                var domainExists = await _context.RegisteredDomains.AnyAsync(d => d.Id == updateDto.DomainId);
                if (!domainExists)
                    throw new InvalidOperationException($"Domain with ID {updateDto.DomainId} not found");
            }

            if (dnsRecord.DnsRecordTypeId != updateDto.DnsRecordTypeId)
            {
                var dnsRecordType = await _context.DnsRecordTypes.FindAsync(updateDto.DnsRecordTypeId);
                if (dnsRecordType == null)
                    throw new InvalidOperationException($"DNS record type with ID {updateDto.DnsRecordTypeId} not found");

                if (!dnsRecordType.IsActive)
                    throw new InvalidOperationException($"DNS record type '{dnsRecordType.Type}' is not active");

                if (dnsRecordType.HasPriority && !updateDto.Priority.HasValue)
                    throw new InvalidOperationException($"Priority is required for DNS record type '{dnsRecordType.Type}'");

                if (dnsRecordType.HasWeight && !updateDto.Weight.HasValue)
                    throw new InvalidOperationException($"Weight is required for DNS record type '{dnsRecordType.Type}'");

                if (dnsRecordType.HasPort && !updateDto.Port.HasValue)
                    throw new InvalidOperationException($"Port is required for DNS record type '{dnsRecordType.Type}'");

                dnsRecord.DnsRecordType = dnsRecordType;
            }

            dnsRecord.DomainId = updateDto.DomainId;
            dnsRecord.DnsRecordTypeId = updateDto.DnsRecordTypeId;
            dnsRecord.Name = updateDto.Name;
            dnsRecord.Value = updateDto.Value;
            dnsRecord.TTL = updateDto.TTL;
            dnsRecord.Priority = updateDto.Priority;
            dnsRecord.Weight = updateDto.Weight;
            dnsRecord.Port = updateDto.Port;
            dnsRecord.IsPendingSync = true;
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

    /// <inheritdoc/>
    public async Task<bool> SoftDeleteDnsRecordAsync(int id)
    {
        try
        {
            _log.Information("Soft-deleting DNS record with ID: {DnsRecordId}", id);

            var dnsRecord = await _context.DnsRecords
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (dnsRecord == null)
            {
                _log.Warning("DNS record with ID {DnsRecordId} not found for soft-delete", id);
                return false;
            }

            dnsRecord.IsDeleted = true;
            dnsRecord.IsPendingSync = true;
            dnsRecord.DeletedAt = DateTime.UtcNow;
            dnsRecord.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully soft-deleted DNS record with ID: {DnsRecordId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while soft-deleting DNS record with ID: {DnsRecordId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> HardDeleteDnsRecordAsync(int id)
    {
        try
        {
            _log.Information("Hard-deleting DNS record with ID: {DnsRecordId}", id);

            var dnsRecord = await _context.DnsRecords.FindAsync(id);

            if (dnsRecord == null)
            {
                _log.Warning("DNS record with ID {DnsRecordId} not found for hard-delete", id);
                return false;
            }

            _context.DnsRecords.Remove(dnsRecord);
            await _context.SaveChangesAsync();

            _log.Information("Successfully hard-deleted DNS record with ID: {DnsRecordId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while hard-deleting DNS record with ID: {DnsRecordId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<DnsRecordDto?> RestoreDnsRecordAsync(int id)
    {
        try
        {
            _log.Information("Restoring soft-deleted DNS record with ID: {DnsRecordId}", id);

            var dnsRecord = await _context.DnsRecords
                .Include(d => d.Domain)
                .Include(d => d.DnsRecordType)
                .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted);

            if (dnsRecord == null)
            {
                _log.Warning("DNS record with ID {DnsRecordId} not found or is not soft-deleted", id);
                return null;
            }

            dnsRecord.IsDeleted = false;
            dnsRecord.DeletedAt = null;
            dnsRecord.IsPendingSync = true;
            dnsRecord.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully restored DNS record with ID: {DnsRecordId}", id);
            return MapToDto(dnsRecord);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while restoring DNS record with ID: {DnsRecordId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<DnsRecordDto?> MarkAsSyncedAsync(int id)
    {
        try
        {
            _log.Information("Marking DNS record with ID {DnsRecordId} as synced", id);

            var dnsRecord = await _context.DnsRecords
                .Include(d => d.Domain)
                .Include(d => d.DnsRecordType)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dnsRecord == null)
            {
                _log.Warning("DNS record with ID {DnsRecordId} not found for mark-as-synced", id);
                return null;
            }

            dnsRecord.IsPendingSync = false;
            dnsRecord.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully marked DNS record with ID {DnsRecordId} as synced", id);
            return MapToDto(dnsRecord);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while marking DNS record with ID {DnsRecordId} as synced", id);
            throw;
        }
    }

    private static DnsRecordDto MapToDto(DnsRecord dnsRecord)
    {
        return new DnsRecordDto
        {
            Id = dnsRecord.Id,
            DomainId = dnsRecord.DomainId,
            DnsRecordTypeId = dnsRecord.DnsRecordTypeId,
            Type = dnsRecord.DnsRecordType.Type,
            Name = dnsRecord.Name,
            Value = dnsRecord.Value,
            TTL = dnsRecord.TTL,
            Priority = dnsRecord.Priority,
            Weight = dnsRecord.Weight,
            Port = dnsRecord.Port,
            IsEditableByUser = dnsRecord.DnsRecordType.IsEditableByUser,
            HasPriority = dnsRecord.DnsRecordType.HasPriority,
            HasWeight = dnsRecord.DnsRecordType.HasWeight,
            HasPort = dnsRecord.DnsRecordType.HasPort,
            IsPendingSync = dnsRecord.IsPendingSync,
            IsDeleted = dnsRecord.IsDeleted,
            DeletedAt = dnsRecord.DeletedAt,
            CreatedAt = dnsRecord.CreatedAt,
            UpdatedAt = dnsRecord.UpdatedAt
        };
    }
}

