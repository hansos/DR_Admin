using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing hosting packages
/// </summary>
public class HostingPackageService : IHostingPackageService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostingPackageService>();

    public HostingPackageService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all hosting packages
    /// </summary>
    /// <returns>Collection of hosting package DTOs</returns>
    public async Task<IEnumerable<HostingPackageDto>> GetAllHostingPackagesAsync()
    {
        try
        {
            _log.Information("Fetching all hosting packages");
            
            var packages = await _context.HostingPackages
                .AsNoTracking()
                .ToListAsync();

            var packageDtos = packages.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} hosting packages", packages.Count);
            return packageDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all hosting packages");
            throw;
        }
    }

    /// <summary>
    /// Retrieves active hosting packages only
    /// </summary>
    /// <returns>Collection of active hosting package DTOs</returns>
    public async Task<IEnumerable<HostingPackageDto>> GetActiveHostingPackagesAsync()
    {
        try
        {
            _log.Information("Fetching active hosting packages");
            
            var packages = await _context.HostingPackages
                .AsNoTracking()
                .Where(p => p.IsActive)
                .ToListAsync();

            var packageDtos = packages.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} active hosting packages", packages.Count);
            return packageDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active hosting packages");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a hosting package by its unique identifier
    /// </summary>
    /// <param name="id">The hosting package ID</param>
    /// <returns>Hosting package DTO if found, otherwise null</returns>
    public async Task<HostingPackageDto?> GetHostingPackageByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching hosting package with ID: {PackageId}", id);
            
            var package = await _context.HostingPackages
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (package == null)
            {
                _log.Warning("Hosting package with ID {PackageId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched hosting package with ID: {PackageId}", id);
            return MapToDto(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching hosting package with ID: {PackageId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new hosting package
    /// </summary>
    /// <param name="createDto">The hosting package creation data</param>
    /// <returns>The created hosting package DTO</returns>
    public async Task<HostingPackageDto> CreateHostingPackageAsync(CreateHostingPackageDto createDto)
    {
        try
        {
            _log.Information("Creating new hosting package with name: {PackageName}", createDto.Name);

            var package = new HostingPackage
            {
                Name = createDto.Name,
                Description = createDto.Description,
                DiskSpaceMB = createDto.DiskSpaceMB,
                BandwidthMB = createDto.BandwidthMB,
                EmailAccounts = createDto.EmailAccounts,
                Databases = createDto.Databases,
                Domains = createDto.Domains,
                Subdomains = createDto.Subdomains,
                FtpAccounts = createDto.FtpAccounts,
                SslSupport = createDto.SslSupport,
                BackupSupport = createDto.BackupSupport,
                DedicatedIp = createDto.DedicatedIp,
                MonthlyPrice = createDto.MonthlyPrice,
                YearlyPrice = createDto.YearlyPrice,
                IsActive = createDto.IsActive
            };

            _context.HostingPackages.Add(package);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created hosting package with ID: {PackageId}", package.Id);
            return MapToDto(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating hosting package with name: {PackageName}", createDto.Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing hosting package
    /// </summary>
    /// <param name="id">The hosting package ID</param>
    /// <param name="updateDto">The hosting package update data</param>
    /// <returns>The updated hosting package DTO if found, otherwise null</returns>
    public async Task<HostingPackageDto?> UpdateHostingPackageAsync(int id, UpdateHostingPackageDto updateDto)
    {
        try
        {
            _log.Information("Updating hosting package with ID: {PackageId}", id);

            var package = await _context.HostingPackages.FindAsync(id);

            if (package == null)
            {
                _log.Warning("Hosting package with ID {PackageId} not found", id);
                return null;
            }

            package.Name = updateDto.Name;
            package.Description = updateDto.Description;
            package.DiskSpaceMB = updateDto.DiskSpaceMB;
            package.BandwidthMB = updateDto.BandwidthMB;
            package.EmailAccounts = updateDto.EmailAccounts;
            package.Databases = updateDto.Databases;
            package.Domains = updateDto.Domains;
            package.Subdomains = updateDto.Subdomains;
            package.FtpAccounts = updateDto.FtpAccounts;
            package.SslSupport = updateDto.SslSupport;
            package.BackupSupport = updateDto.BackupSupport;
            package.DedicatedIp = updateDto.DedicatedIp;
            package.MonthlyPrice = updateDto.MonthlyPrice;
            package.YearlyPrice = updateDto.YearlyPrice;
            package.IsActive = updateDto.IsActive;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated hosting package with ID: {PackageId}", id);
            return MapToDto(package);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating hosting package with ID: {PackageId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a hosting package
    /// </summary>
    /// <param name="id">The hosting package ID</param>
    /// <returns>True if deleted successfully, otherwise false</returns>
    public async Task<bool> DeleteHostingPackageAsync(int id)
    {
        try
        {
            _log.Information("Deleting hosting package with ID: {PackageId}", id);

            var package = await _context.HostingPackages.FindAsync(id);

            if (package == null)
            {
                _log.Warning("Hosting package with ID {PackageId} not found", id);
                return false;
            }

            _context.HostingPackages.Remove(package);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted hosting package with ID: {PackageId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting hosting package with ID: {PackageId}", id);
            throw;
        }
    }

    private static HostingPackageDto MapToDto(HostingPackage package)
    {
        return new HostingPackageDto
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            DiskSpaceMB = package.DiskSpaceMB,
            BandwidthMB = package.BandwidthMB,
            EmailAccounts = package.EmailAccounts,
            Databases = package.Databases,
            Domains = package.Domains,
            Subdomains = package.Subdomains,
            FtpAccounts = package.FtpAccounts,
            SslSupport = package.SslSupport,
            BackupSupport = package.BackupSupport,
            DedicatedIp = package.DedicatedIp,
            MonthlyPrice = package.MonthlyPrice,
            YearlyPrice = package.YearlyPrice,
            IsActive = package.IsActive,
            CreatedAt = package.CreatedAt,
            UpdatedAt = package.UpdatedAt
        };
    }
}
