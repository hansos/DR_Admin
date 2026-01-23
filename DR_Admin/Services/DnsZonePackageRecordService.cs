using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing DNS zone package records
/// </summary>
public class DnsZonePackageRecordService : IDnsZonePackageRecordService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<DnsZonePackageRecordService>();

    public DnsZonePackageRecordService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all DNS zone package records
    /// </summary>
    /// <returns>Collection of DNS zone package record DTOs</returns>
    public async Task<IEnumerable<DnsZonePackageRecordDto>> GetAllDnsZonePackageRecordsAsync()
    {
        try
        {
            _log.Information("Fetching all DNS zone package records");
            
            var records = await _context.DnsZonePackageRecords
                .AsNoTracking()
                .ToListAsync();

            var recordDtos = records.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} DNS zone package records", records.Count);
            return recordDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all DNS zone package records");
            throw;
        }
    }

    /// <summary>
    /// Retrieves DNS zone package records for a specific package
    /// </summary>
    /// <param name="packageId">The DNS zone package ID</param>
    /// <returns>Collection of DNS zone package record DTOs</returns>
    public async Task<IEnumerable<DnsZonePackageRecordDto>> GetRecordsByPackageIdAsync(int packageId)
    {
        try
        {
            _log.Information("Fetching DNS zone package records for package ID: {PackageId}", packageId);
            
            var records = await _context.DnsZonePackageRecords
                .AsNoTracking()
                .Where(r => r.DnsZonePackageId == packageId)
                .ToListAsync();

            var recordDtos = records.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} DNS zone package records for package ID: {PackageId}", 
                records.Count, packageId);
            return recordDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching DNS zone package records for package ID: {PackageId}", packageId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a DNS zone package record by its unique identifier
    /// </summary>
    /// <param name="id">The DNS zone package record ID</param>
    /// <returns>DNS zone package record DTO if found, otherwise null</returns>
    public async Task<DnsZonePackageRecordDto?> GetDnsZonePackageRecordByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching DNS zone package record with ID: {RecordId}", id);
            
            var record = await _context.DnsZonePackageRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (record == null)
            {
                _log.Warning("DNS zone package record with ID {RecordId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched DNS zone package record with ID: {RecordId}", id);
            return MapToDto(record);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching DNS zone package record with ID: {RecordId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new DNS zone package record
    /// </summary>
    /// <param name="createDto">The DNS zone package record creation data</param>
    /// <returns>The created DNS zone package record DTO</returns>
    public async Task<DnsZonePackageRecordDto> CreateDnsZonePackageRecordAsync(CreateDnsZonePackageRecordDto createDto)
    {
        try
        {
            _log.Information("Creating new DNS zone package record for package ID: {PackageId}", createDto.DnsZonePackageId);

            var record = new DnsZonePackageRecord
            {
                DnsZonePackageId = createDto.DnsZonePackageId,
                DnsRecordTypeId = createDto.DnsRecordTypeId,
                Name = createDto.Name,
                Value = createDto.Value,
                TTL = createDto.TTL,
                Priority = createDto.Priority,
                Weight = createDto.Weight,
                Port = createDto.Port,
                Notes = createDto.Notes
            };

            _context.DnsZonePackageRecords.Add(record);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created DNS zone package record with ID: {RecordId}", record.Id);
            return MapToDto(record);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating DNS zone package record for package ID: {PackageId}", 
                createDto.DnsZonePackageId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing DNS zone package record
    /// </summary>
    /// <param name="id">The DNS zone package record ID</param>
    /// <param name="updateDto">The DNS zone package record update data</param>
    /// <returns>The updated DNS zone package record DTO if found, otherwise null</returns>
    public async Task<DnsZonePackageRecordDto?> UpdateDnsZonePackageRecordAsync(int id, UpdateDnsZonePackageRecordDto updateDto)
    {
        try
        {
            _log.Information("Updating DNS zone package record with ID: {RecordId}", id);

            var record = await _context.DnsZonePackageRecords.FindAsync(id);

            if (record == null)
            {
                _log.Warning("DNS zone package record with ID {RecordId} not found", id);
                return null;
            }

            record.DnsRecordTypeId = updateDto.DnsRecordTypeId;
            record.Name = updateDto.Name;
            record.Value = updateDto.Value;
            record.TTL = updateDto.TTL;
            record.Priority = updateDto.Priority;
            record.Weight = updateDto.Weight;
            record.Port = updateDto.Port;
            record.Notes = updateDto.Notes;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated DNS zone package record with ID: {RecordId}", id);
            return MapToDto(record);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating DNS zone package record with ID: {RecordId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a DNS zone package record
    /// </summary>
    /// <param name="id">The DNS zone package record ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    public async Task<bool> DeleteDnsZonePackageRecordAsync(int id)
    {
        try
        {
            _log.Information("Deleting DNS zone package record with ID: {RecordId}", id);

            var record = await _context.DnsZonePackageRecords.FindAsync(id);

            if (record == null)
            {
                _log.Warning("DNS zone package record with ID {RecordId} not found", id);
                return false;
            }

            _context.DnsZonePackageRecords.Remove(record);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted DNS zone package record with ID: {RecordId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting DNS zone package record with ID: {RecordId}", id);
            throw;
        }
    }

    private static DnsZonePackageRecordDto MapToDto(DnsZonePackageRecord record)
    {
        return new DnsZonePackageRecordDto
        {
            Id = record.Id,
            DnsZonePackageId = record.DnsZonePackageId,
            DnsRecordTypeId = record.DnsRecordTypeId,
            Name = record.Name,
            Value = record.Value,
            TTL = record.TTL,
            Priority = record.Priority,
            Weight = record.Weight,
            Port = record.Port,
            Notes = record.Notes,
            CreatedAt = record.CreatedAt,
            UpdatedAt = record.UpdatedAt
        };
    }
}
