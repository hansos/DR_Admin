using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing DNS zone packages
/// </summary>
public class DnsZonePackageService : IDnsZonePackageService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<DnsZonePackageService>();

    public DnsZonePackageService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all DNS zone packages
    /// </summary>
    /// <returns>Collection of DNS zone package DTOs</returns>
    public async Task<IEnumerable<DnsZonePackageDto>> GetAllDnsZonePackagesAsync()
    {
        try
        {
            _log.Information("Fetching all DNS zone packages");
            
            var packages = await _context.DnsZonePackages
                .AsNoTracking()
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.Name)
                .ToListAsync();

            var packageDtos = packages.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} DNS zone packages", packages.Count);
            return packageDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all DNS zone packages");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all DNS zone packages with their records
    /// </summary>
    /// <returns>Collection of DNS zone package DTOs including records</returns>
    public async Task<IEnumerable<DnsZonePackageDto>> GetAllDnsZonePackagesWithRecordsAsync()
    {
        try
        {
            _log.Information("Fetching all DNS zone packages with records");
            
            var packages = await _context.DnsZonePackages
                .AsNoTracking()
                .Include(p => p.Records)
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.Name)
                .ToListAsync();

            var packageDtos = packages.Select(MapToDtoWithRecords);
            
            _log.Information("Successfully fetched {Count} DNS zone packages with records", packages.Count);
            return packageDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all DNS zone packages with records");
            throw;
        }
    }

    /// <summary>
    /// Retrieves active DNS zone packages only
    /// </summary>
    /// <returns>Collection of active DNS zone package DTOs</returns>
    public async Task<IEnumerable<DnsZonePackageDto>> GetActiveDnsZonePackagesAsync()
    {
        try
        {
            _log.Information("Fetching active DNS zone packages");
            
            var packages = await _context.DnsZonePackages
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.Name)
                .ToListAsync();

            var packageDtos = packages.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} active DNS zone packages", packages.Count);
            return packageDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active DNS zone packages");
            throw;
        }
    }

    /// <summary>
    /// Retrieves the default DNS zone package
    /// </summary>
    /// <returns>The default DNS zone package DTO if found, otherwise null</returns>
    public async Task<DnsZonePackageDto?> GetDefaultDnsZonePackageAsync()
    {
        try
        {
            _log.Information("Fetching default DNS zone package");
            
            var package = await _context.DnsZonePackages
                .AsNoTracking()
                .Include(p => p.Records)
                .FirstOrDefaultAsync(p => p.IsDefault && p.IsActive);

            if (package == null)
            {
                _log.Warning("No default DNS zone package found");
                return null;
            }

            _log.Information("Successfully fetched default DNS zone package with ID: {PackageId}", package.Id);
            return MapToDtoWithRecords(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching default DNS zone package");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a DNS zone package by its unique identifier
    /// </summary>
    /// <param name="id">The DNS zone package ID</param>
    /// <returns>DNS zone package DTO if found, otherwise null</returns>
    public async Task<DnsZonePackageDto?> GetDnsZonePackageByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching DNS zone package with ID: {PackageId}", id);
            
            var package = await _context.DnsZonePackages
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (package == null)
            {
                _log.Warning("DNS zone package with ID {PackageId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched DNS zone package with ID: {PackageId}", id);
            return MapToDto(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching DNS zone package with ID: {PackageId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a DNS zone package with its records by its unique identifier
    /// </summary>
    /// <param name="id">The DNS zone package ID</param>
    /// <returns>DNS zone package DTO with records if found, otherwise null</returns>
    public async Task<DnsZonePackageDto?> GetDnsZonePackageWithRecordsByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching DNS zone package with records, ID: {PackageId}", id);
            
            var package = await _context.DnsZonePackages
                .AsNoTracking()
                .Include(p => p.Records)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (package == null)
            {
                _log.Warning("DNS zone package with ID {PackageId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched DNS zone package with {RecordCount} records, ID: {PackageId}", 
                package.Records.Count, id);
            return MapToDtoWithRecords(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching DNS zone package with records, ID: {PackageId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new DNS zone package
    /// </summary>
    /// <param name="createDto">The DNS zone package creation data</param>
    /// <returns>The created DNS zone package DTO</returns>
    public async Task<DnsZonePackageDto> CreateDnsZonePackageAsync(CreateDnsZonePackageDto createDto)
    {
        try
        {
            _log.Information("Creating new DNS zone package with name: {PackageName}", createDto.Name);

            // If setting as default, unset any existing default
            if (createDto.IsDefault)
            {
                await UnsetCurrentDefaultAsync();
            }

            var package = new DnsZonePackage
            {
                Name = createDto.Name,
                Description = createDto.Description,
                IsActive = createDto.IsActive,
                IsDefault = createDto.IsDefault,
                SortOrder = createDto.SortOrder,
                ResellerCompanyId = createDto.ResellerCompanyId,
                SalesAgentId = createDto.SalesAgentId
            };

            _context.DnsZonePackages.Add(package);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created DNS zone package with ID: {PackageId}", package.Id);
            return MapToDto(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating DNS zone package with name: {PackageName}", createDto.Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing DNS zone package
    /// </summary>
    /// <param name="id">The DNS zone package ID</param>
    /// <param name="updateDto">The DNS zone package update data</param>
    /// <returns>The updated DNS zone package DTO if found, otherwise null</returns>
    public async Task<DnsZonePackageDto?> UpdateDnsZonePackageAsync(int id, UpdateDnsZonePackageDto updateDto)
    {
        try
        {
            _log.Information("Updating DNS zone package with ID: {PackageId}", id);

            var package = await _context.DnsZonePackages.FindAsync(id);

            if (package == null)
            {
                _log.Warning("DNS zone package with ID {PackageId} not found", id);
                return null;
            }

            // If setting as default, unset any existing default
            if (updateDto.IsDefault && !package.IsDefault)
            {
                await UnsetCurrentDefaultAsync();
            }

            package.Name = updateDto.Name;
            package.Description = updateDto.Description;
            package.IsActive = updateDto.IsActive;
            package.IsDefault = updateDto.IsDefault;
            package.SortOrder = updateDto.SortOrder;
            package.ResellerCompanyId = updateDto.ResellerCompanyId;
            package.SalesAgentId = updateDto.SalesAgentId;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated DNS zone package with ID: {PackageId}", id);
            return MapToDto(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating DNS zone package with ID: {PackageId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a DNS zone package
    /// </summary>
    /// <param name="id">The DNS zone package ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    public async Task<bool> DeleteDnsZonePackageAsync(int id)
    {
        try
        {
            _log.Information("Deleting DNS zone package with ID: {PackageId}", id);

            var package = await _context.DnsZonePackages.FindAsync(id);

            if (package == null)
            {
                _log.Warning("DNS zone package with ID {PackageId} not found", id);
                return false;
            }

            _context.DnsZonePackages.Remove(package);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted DNS zone package with ID: {PackageId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting DNS zone package with ID: {PackageId}", id);
            throw;
        }
    }

    /// <summary>
    /// Applies a DNS zone package to a domain by creating DNS records
    /// </summary>
    /// <param name="packageId">The DNS zone package ID</param>
    /// <param name="domainId">The domain ID to apply records to</param>
    /// <returns>True if applied successfully, otherwise false</returns>
    public async Task<bool> ApplyPackageToDomainAsync(int packageId, int domainId)
    {
        try
        {
            _log.Information("Applying DNS zone package {PackageId} to domain {DomainId}", packageId, domainId);

            var package = await _context.DnsZonePackages
                .Include(p => p.Records)
                .FirstOrDefaultAsync(p => p.Id == packageId);

            if (package == null)
            {
                _log.Warning("DNS zone package with ID {PackageId} not found", packageId);
                return false;
            }

            var domain = await _context.RegisteredDomains.FindAsync(domainId);
            if (domain == null)
            {
                _log.Warning("Domain with ID {DomainId} not found", domainId);
                return false;
            }

            // Create DNS records from package template
            foreach (var templateRecord in package.Records)
            {
                var dnsRecord = new DnsRecord
                {
                    DomainId = domainId,
                    DnsRecordTypeId = templateRecord.DnsRecordTypeId,
                    Name = templateRecord.Name,
                    Value = templateRecord.Value,
                    TTL = templateRecord.TTL,
                    Priority = templateRecord.Priority,
                    Weight = templateRecord.Weight,
                    Port = templateRecord.Port
                };

                _context.DnsRecords.Add(dnsRecord);
            }

            await _context.SaveChangesAsync();

            _log.Information("Successfully applied DNS zone package {PackageId} with {RecordCount} records to domain {DomainId}", 
                packageId, package.Records.Count, domainId);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while applying DNS zone package {PackageId} to domain {DomainId}", packageId, domainId);
            throw;
        }
    }

    private async Task UnsetCurrentDefaultAsync()
    {
        var currentDefault = await _context.DnsZonePackages
            .FirstOrDefaultAsync(p => p.IsDefault);

        if (currentDefault != null)
        {
            currentDefault.IsDefault = false;
            _log.Information("Unsetting previous default DNS zone package with ID: {PackageId}", currentDefault.Id);
        }
    }

    private static DnsZonePackageDto MapToDto(DnsZonePackage package)
    {
        return new DnsZonePackageDto
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            IsActive = package.IsActive,
            IsDefault = package.IsDefault,
            SortOrder = package.SortOrder,
            ResellerCompanyId = package.ResellerCompanyId,
            SalesAgentId = package.SalesAgentId,
            CreatedAt = package.CreatedAt,
            UpdatedAt = package.UpdatedAt,
            Records = new List<DnsZonePackageRecordDto>(),
            ControlPanels = new List<DnsZonePackageControlPanelSummaryDto>(),
            Servers = new List<DnsZonePackageServerSummaryDto>()
        };
    }

    private static DnsZonePackageDto MapToDtoWithRecords(DnsZonePackage package)
    {
        return new DnsZonePackageDto
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            IsActive = package.IsActive,
            IsDefault = package.IsDefault,
            SortOrder = package.SortOrder,
            ResellerCompanyId = package.ResellerCompanyId,
            SalesAgentId = package.SalesAgentId,
            CreatedAt = package.CreatedAt,
            UpdatedAt = package.UpdatedAt,
            Records = package.Records.Select(r => new DnsZonePackageRecordDto
            {
                Id = r.Id,
                DnsZonePackageId = r.DnsZonePackageId,
                DnsRecordTypeId = r.DnsRecordTypeId,
                Name = r.Name,
                Value = r.Value,
                TTL = r.TTL,
                Priority = r.Priority,
                Weight = r.Weight,
                Port = r.Port,
                Notes = r.Notes,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList(),
            ControlPanels = new List<DnsZonePackageControlPanelSummaryDto>(),
            Servers = new List<DnsZonePackageServerSummaryDto>()
        };
    }

    private static DnsZonePackageDto MapToDtoWithAssignments(DnsZonePackage package)
    {
        var dto = MapToDtoWithRecords(package);

        dto.ControlPanels = package.ControlPanels.Select(cp => new DnsZonePackageControlPanelSummaryDto
        {
            ControlPanelId = cp.ServerControlPanelId,
            ApiUrl = cp.ServerControlPanel?.ApiUrl ?? string.Empty,
            ServerName = cp.ServerControlPanel?.Server?.Name ?? string.Empty,
            ControlPanelTypeName = cp.ServerControlPanel?.ControlPanelType?.DisplayName ?? string.Empty
        }).ToList();

        dto.Servers = package.Servers.Select(s => new DnsZonePackageServerSummaryDto
        {
            ServerId = s.ServerId,
            ServerName = s.Server?.Name ?? string.Empty,
            Status = s.Server?.Status ?? string.Empty
        }).ToList();

        return dto;
    }

    // ── M2M: GetDnsZonePackageWithAssignmentsAsync ────────────────────────────

    public async Task<DnsZonePackageDto?> GetDnsZonePackageWithAssignmentsAsync(int id)
    {
        try
        {
            _log.Information("Fetching DNS zone package with assignments, ID: {PackageId}", id);

            var package = await _context.DnsZonePackages
                .AsNoTracking()
                .Include(p => p.Records)
                .Include(p => p.ControlPanels)
                    .ThenInclude(cp => cp.ServerControlPanel)
                        .ThenInclude(cp => cp.Server)
                .Include(p => p.ControlPanels)
                    .ThenInclude(cp => cp.ServerControlPanel)
                        .ThenInclude(cp => cp.ControlPanelType)
                .Include(p => p.Servers)
                    .ThenInclude(s => s.Server)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (package == null)
            {
                _log.Warning("DNS zone package with ID {PackageId} not found", id);
                return null;
            }

            return MapToDtoWithAssignments(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error fetching DNS zone package with assignments, ID: {PackageId}", id);
            throw;
        }
    }

    // ── M2M: Control Panel assignments ───────────────────────────────────────

    public async Task<IEnumerable<DnsZonePackageDto>> GetPackagesByControlPanelAsync(int controlPanelId)
    {
        try
        {
            _log.Information("Fetching DNS zone packages for control panel {ControlPanelId}", controlPanelId);

            var packages = await _context.DnsZonePackageControlPanels
                .AsNoTracking()
                .Where(cp => cp.ServerControlPanelId == controlPanelId)
                .Include(cp => cp.DnsZonePackage)
                .Select(cp => cp.DnsZonePackage)
                .OrderBy(p => p.SortOrder).ThenBy(p => p.Name)
                .ToListAsync();

            return packages.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error fetching DNS zone packages for control panel {ControlPanelId}", controlPanelId);
            throw;
        }
    }

    public async Task<bool> AssignControlPanelAsync(int packageId, int controlPanelId)
    {
        try
        {
            _log.Information("Assigning control panel {ControlPanelId} to DNS zone package {PackageId}", controlPanelId, packageId);

            var packageExists = await _context.DnsZonePackages.AnyAsync(p => p.Id == packageId);
            if (!packageExists)
            {
                _log.Warning("DNS zone package {PackageId} not found", packageId);
                return false;
            }

            var panelExists = await _context.ServerControlPanels.AnyAsync(p => p.Id == controlPanelId);
            if (!panelExists)
            {
                _log.Warning("ServerControlPanel {ControlPanelId} not found", controlPanelId);
                return false;
            }

            var alreadyAssigned = await _context.DnsZonePackageControlPanels
                .AnyAsync(x => x.DnsZonePackageId == packageId && x.ServerControlPanelId == controlPanelId);

            if (alreadyAssigned)
            {
                _log.Information("Control panel {ControlPanelId} already assigned to package {PackageId}", controlPanelId, packageId);
                return true;
            }

            _context.DnsZonePackageControlPanels.Add(new Data.Entities.DnsZonePackageControlPanel
            {
                DnsZonePackageId = packageId,
                ServerControlPanelId = controlPanelId
            });

            await _context.SaveChangesAsync();
            _log.Information("Successfully assigned control panel {ControlPanelId} to package {PackageId}", controlPanelId, packageId);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error assigning control panel {ControlPanelId} to package {PackageId}", controlPanelId, packageId);
            throw;
        }
    }

    public async Task<bool> RemoveControlPanelAsync(int packageId, int controlPanelId)
    {
        try
        {
            _log.Information("Removing control panel {ControlPanelId} from DNS zone package {PackageId}", controlPanelId, packageId);

            var assignment = await _context.DnsZonePackageControlPanels
                .FirstOrDefaultAsync(x => x.DnsZonePackageId == packageId && x.ServerControlPanelId == controlPanelId);

            if (assignment == null)
            {
                _log.Warning("Assignment not found: control panel {ControlPanelId} / package {PackageId}", controlPanelId, packageId);
                return false;
            }

            _context.DnsZonePackageControlPanels.Remove(assignment);
            await _context.SaveChangesAsync();
            _log.Information("Successfully removed control panel {ControlPanelId} from package {PackageId}", controlPanelId, packageId);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error removing control panel {ControlPanelId} from package {PackageId}", controlPanelId, packageId);
            throw;
        }
    }

    // ── M2M: Server assignments ───────────────────────────────────────────────

    public async Task<IEnumerable<DnsZonePackageDto>> GetPackagesByServerAsync(int serverId)
    {
        try
        {
            _log.Information("Fetching DNS zone packages for server {ServerId}", serverId);

            var packages = await _context.DnsZonePackageServers
                .AsNoTracking()
                .Where(s => s.ServerId == serverId)
                .Include(s => s.DnsZonePackage)
                .Select(s => s.DnsZonePackage)
                .OrderBy(p => p.SortOrder).ThenBy(p => p.Name)
                .ToListAsync();

            return packages.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error fetching DNS zone packages for server {ServerId}", serverId);
            throw;
        }
    }

    public async Task<bool> AssignServerAsync(int packageId, int serverId)
    {
        try
        {
            _log.Information("Assigning server {ServerId} to DNS zone package {PackageId}", serverId, packageId);

            var packageExists = await _context.DnsZonePackages.AnyAsync(p => p.Id == packageId);
            if (!packageExists)
            {
                _log.Warning("DNS zone package {PackageId} not found", packageId);
                return false;
            }

            var serverExists = await _context.Servers.AnyAsync(s => s.Id == serverId);
            if (!serverExists)
            {
                _log.Warning("Server {ServerId} not found", serverId);
                return false;
            }

            var alreadyAssigned = await _context.DnsZonePackageServers
                .AnyAsync(x => x.DnsZonePackageId == packageId && x.ServerId == serverId);

            if (alreadyAssigned)
            {
                _log.Information("Server {ServerId} already assigned to package {PackageId}", serverId, packageId);
                return true;
            }

            _context.DnsZonePackageServers.Add(new Data.Entities.DnsZonePackageServer
            {
                DnsZonePackageId = packageId,
                ServerId = serverId
            });

            await _context.SaveChangesAsync();
            _log.Information("Successfully assigned server {ServerId} to package {PackageId}", serverId, packageId);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error assigning server {ServerId} to package {PackageId}", serverId, packageId);
            throw;
        }
    }

    public async Task<bool> RemoveServerAsync(int packageId, int serverId)
    {
        try
        {
            _log.Information("Removing server {ServerId} from DNS zone package {PackageId}", serverId, packageId);

            var assignment = await _context.DnsZonePackageServers
                .FirstOrDefaultAsync(x => x.DnsZonePackageId == packageId && x.ServerId == serverId);

            if (assignment == null)
            {
                _log.Warning("Assignment not found: server {ServerId} / package {PackageId}", serverId, packageId);
                return false;
            }

            _context.DnsZonePackageServers.Remove(assignment);
            await _context.SaveChangesAsync();
            _log.Information("Successfully removed server {ServerId} from package {PackageId}", serverId, packageId);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error removing server {ServerId} from package {PackageId}", serverId, packageId);
            throw;
        }
    }
}
