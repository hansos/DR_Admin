using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class RegistrarTldService : IRegistrarTldService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarTldService>();

    public RegistrarTldService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RegistrarTldDto>> GetAllRegistrarTldsAsync()
    {
        try
        {
            _log.Information("Fetching all registrar TLDs");
            
            var registrarTlds = await _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .OrderBy(rt => rt.Registrar.Name)
                .ThenBy(rt => rt.Tld.Extension)
                .ToListAsync();

            var registrarTldDtos = registrarTlds.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} registrar TLDs", registrarTlds.Count);
            return registrarTldDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all registrar TLDs");
            throw;
        }
    }

    public async Task<PagedResult<RegistrarTldDto>> GetAllRegistrarTldsPagedAsync(PaginationParameters parameters, bool? isActive = null)
    {
        try
        {
            _log.Information("Fetching paginated registrar TLDs - Page: {PageNumber}, PageSize: {PageSize}, IsActive: {IsActive}", 
                parameters.PageNumber, parameters.PageSize, isActive?.ToString() ?? "null");
            
            // Build query with optional filter
            var query = _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .AsQueryable();

            // Apply filter if specified
            if (isActive.HasValue)
            {
                query = query.Where(rt => rt.IsActive == isActive.Value);
            }

            // Get total count after filtering
            var totalCount = await query.CountAsync();

            // Get paginated data
            var registrarTlds = await query
                .OrderBy(rt => rt.Registrar.Name)
                .ThenBy(rt => rt.Tld.Extension)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var registrarTldDtos = registrarTlds.Select(MapToDto).ToList();
            
            var result = new PagedResult<RegistrarTldDto>(
                registrarTldDtos, 
                totalCount, 
                parameters.PageNumber, 
                parameters.PageSize);

            _log.Information("Successfully fetched page {PageNumber} of registrar TLDs - Returned {Count} of {TotalCount} total", 
                parameters.PageNumber, registrarTldDtos.Count, totalCount);
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching paginated registrar TLDs");
            throw;
        }
    }

    public async Task<IEnumerable<RegistrarTldDto>> GetAvailableRegistrarTldsAsync()
    {
        try
        {
            _log.Information("Fetching available registrar TLDs");
            
            var registrarTlds = await _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .Where(rt => rt.IsActive && rt.Registrar.IsActive && rt.Tld.IsActive)
                .OrderBy(rt => rt.Registrar.Name)
                .ThenBy(rt => rt.Tld.Extension)
                .ToListAsync();

            var registrarTldDtos = registrarTlds.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} available registrar TLDs", registrarTlds.Count);
            return registrarTldDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching available registrar TLDs");
            throw;
        }
    }

    public async Task<IEnumerable<RegistrarTldDto>> GetRegistrarTldsByRegistrarAsync(int registrarId)
    {
        try
        {
            _log.Information("Fetching registrar TLDs for registrar: {RegistrarId}", registrarId);
            
            var registrarTlds = await _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .Where(rt => rt.RegistrarId == registrarId)
                .OrderBy(rt => rt.Tld.Extension)
                .ToListAsync();

            var registrarTldDtos = registrarTlds.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} registrar TLDs for registrar {RegistrarId}", 
                registrarTlds.Count, registrarId);
            return registrarTldDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching registrar TLDs for registrar: {RegistrarId}", registrarId);
            throw;
        }
    }

    public async Task<PagedResult<RegistrarTldDto>> GetRegistrarTldsByRegistrarPagedAsync(int registrarId, PaginationParameters parameters, bool? isActive = null)
    {
        try
        {
            _log.Information("Fetching paginated registrar TLDs for registrar: {RegistrarId} - Page: {PageNumber}, PageSize: {PageSize}, IsActive: {IsActive}", 
                registrarId, parameters.PageNumber, parameters.PageSize, isActive?.ToString() ?? "null");
            
            // Build query with registrar filter
            var query = _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .Where(rt => rt.RegistrarId == registrarId);

            // Apply active filter if specified
            if (isActive.HasValue)
            {
                query = query.Where(rt => rt.IsActive == isActive.Value);
            }

            // Get total count after filtering
            var totalCount = await query.CountAsync();

            // Get paginated data
            var registrarTlds = await query
                .OrderBy(rt => rt.Tld.Extension)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var registrarTldDtos = registrarTlds.Select(MapToDto).ToList();
            
            var result = new PagedResult<RegistrarTldDto>(
                registrarTldDtos, 
                totalCount, 
                parameters.PageNumber, 
                parameters.PageSize);

            _log.Information("Successfully fetched page {PageNumber} of registrar TLDs for registrar {RegistrarId} - Returned {Count} of {TotalCount} total", 
                parameters.PageNumber, registrarId, registrarTldDtos.Count, totalCount);
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching paginated registrar TLDs for registrar: {RegistrarId}", registrarId);
            throw;
        }
    }

    public async Task<IEnumerable<RegistrarTldDto>> GetRegistrarTldsByTldAsync(int tldId)
    {
        try
        {
            _log.Information("Fetching registrar TLDs for TLD: {TldId}", tldId);
            
            var registrarTlds = await _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .Where(rt => rt.TldId == tldId)
                .OrderBy(rt => rt.Registrar.Name)
                .ToListAsync();

            var registrarTldDtos = registrarTlds.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} registrar TLDs for TLD {TldId}", 
                registrarTlds.Count, tldId);
            return registrarTldDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching registrar TLDs for TLD: {TldId}", tldId);
            throw;
        }
    }

    public async Task<RegistrarTldDto?> GetRegistrarTldByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching registrar TLD with ID: {RegistrarTldId}", id);
            
            var registrarTld = await _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .FirstOrDefaultAsync(rt => rt.Id == id);

            if (registrarTld == null)
            {
                _log.Warning("Registrar TLD with ID {RegistrarTldId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched registrar TLD with ID: {RegistrarTldId}", id);
            return MapToDto(registrarTld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching registrar TLD with ID: {RegistrarTldId}", id);
            throw;
        }
    }

    public async Task<RegistrarTldDto?> GetRegistrarTldByRegistrarAndTldAsync(int registrarId, int tldId)
    {
        try
        {
            _log.Information("Fetching registrar TLD for registrar {RegistrarId} and TLD {TldId}", 
                registrarId, tldId);
            
            var registrarTld = await _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .FirstOrDefaultAsync(rt => rt.RegistrarId == registrarId && rt.TldId == tldId);

            if (registrarTld == null)
            {
                _log.Warning("Registrar TLD for registrar {RegistrarId} and TLD {TldId} not found", 
                    registrarId, tldId);
                return null;
            }

            _log.Information("Successfully fetched registrar TLD for registrar {RegistrarId} and TLD {TldId}", 
                registrarId, tldId);
            return MapToDto(registrarTld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching registrar TLD for registrar {RegistrarId} and TLD {TldId}", 
                registrarId, tldId);
            throw;
        }
    }

    public async Task<RegistrarTldDto> CreateRegistrarTldAsync(CreateRegistrarTldDto createDto)
    {
        try
        {
            _log.Information("Creating new registrar TLD for registrar {RegistrarId} and TLD {TldId}", 
                createDto.RegistrarId, createDto.TldId);

            var registrarExists = await _context.Registrars.AnyAsync(r => r.Id == createDto.RegistrarId);
            if (!registrarExists)
            {
                _log.Warning("Registrar with ID {RegistrarId} does not exist", createDto.RegistrarId);
                throw new InvalidOperationException($"Registrar with ID {createDto.RegistrarId} does not exist");
            }

            var tldExists = await _context.Tlds.AnyAsync(t => t.Id == createDto.TldId);
            if (!tldExists)
            {
                _log.Warning("TLD with ID {TldId} does not exist", createDto.TldId);
                throw new InvalidOperationException($"TLD with ID {createDto.TldId} does not exist");
            }

            var existingRegistrarTld = await _context.RegistrarTlds
                .FirstOrDefaultAsync(rt => rt.RegistrarId == createDto.RegistrarId && rt.TldId == createDto.TldId);

            if (existingRegistrarTld != null)
            {
                _log.Warning("Registrar TLD for registrar {RegistrarId} and TLD {TldId} already exists", 
                    createDto.RegistrarId, createDto.TldId);
                throw new InvalidOperationException("This registrar-TLD combination already exists");
            }

            var registrarTld = new RegistrarTld
            {
                RegistrarId = createDto.RegistrarId,
                TldId = createDto.TldId,
                IsActive = createDto.IsActive,
                AutoRenew = createDto.AutoRenew,
                MinRegistrationYears = createDto.MinRegistrationYears,
                MaxRegistrationYears = createDto.MaxRegistrationYears,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.RegistrarTlds.Add(registrarTld);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            await _context.Entry(registrarTld).Reference(rt => rt.Registrar).LoadAsync();
            await _context.Entry(registrarTld).Reference(rt => rt.Tld).LoadAsync();

            _log.Information("Successfully created registrar TLD with ID: {RegistrarTldId}. " +
                "Pricing should be created separately using RegistrarTldCostPricing and TldSalesPricing endpoints.", 
                registrarTld.Id);
            return MapToDto(registrarTld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating registrar TLD for registrar {RegistrarId} and TLD {TldId}", 
                createDto.RegistrarId, createDto.TldId);
            throw;
        }
    }

    public async Task<RegistrarTldDto?> UpdateRegistrarTldAsync(int id, UpdateRegistrarTldDto updateDto)
    {
        try
        {
            _log.Information("Updating registrar TLD with ID: {RegistrarTldId}", id);

            var registrarTld = await _context.RegistrarTlds
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .FirstOrDefaultAsync(rt => rt.Id == id);

            if (registrarTld == null)
            {
                _log.Warning("Registrar TLD with ID {RegistrarTldId} not found for update", id);
                return null;
            }

            registrarTld.IsActive = updateDto.IsActive;
            registrarTld.AutoRenew = updateDto.AutoRenew;
            registrarTld.MinRegistrationYears = updateDto.MinRegistrationYears;
            registrarTld.MaxRegistrationYears = updateDto.MaxRegistrationYears;
            registrarTld.Notes = updateDto.Notes;
            registrarTld.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated registrar TLD with ID: {RegistrarTldId}. " +
                "Pricing should be updated separately using RegistrarTldCostPricing and TldSalesPricing endpoints.", id);
            return MapToDto(registrarTld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating registrar TLD with ID: {RegistrarTldId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteRegistrarTldAsync(int id)
    {
        try
        {
            _log.Information("Deleting registrar TLD with ID: {RegistrarTldId}", id);

            var registrarTld = await _context.RegistrarTlds.FindAsync(id);

            if (registrarTld == null)
            {
                _log.Warning("Registrar TLD with ID {RegistrarTldId} not found for deletion", id);
                return false;
            }

            var hasDomains = await _context.RegisteredDomains.AnyAsync(d => d.RegistrarTldId == id);
            if (hasDomains)
            {
                _log.Warning("Cannot delete registrar TLD {RegistrarTldId}: has associated domains", id);
                throw new InvalidOperationException("Cannot delete registrar TLD with associated domains");
            }

            _context.RegistrarTlds.Remove(registrarTld);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted registrar TLD with ID: {RegistrarTldId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting registrar TLD with ID: {RegistrarTldId}", id);
            throw;
        }
    }

    private static RegistrarTldDto MapToDto(RegistrarTld registrarTld)
    {
        return new RegistrarTldDto
        {
            Id = registrarTld.Id,
            RegistrarId = registrarTld.RegistrarId,
            RegistrarName = registrarTld.Registrar?.Name,
            TldId = registrarTld.TldId,
            TldExtension = registrarTld.Tld?.Extension,
            IsActive = registrarTld.IsActive,
            AutoRenew = registrarTld.AutoRenew,
            MinRegistrationYears = registrarTld.MinRegistrationYears,
            MaxRegistrationYears = registrarTld.MaxRegistrationYears,
            Notes = registrarTld.Notes,
            CreatedAt = registrarTld.CreatedAt,
            UpdatedAt = registrarTld.UpdatedAt
            // Note: CurrentCostPricing and CurrentSalesPricing can be loaded separately if needed
        };
    }

    /// <summary>
    /// Imports TLDs for a registrar from content data
    /// </summary>
    /// <param name="registrarId">The registrar ID</param>
    /// <param name="importDto">The import data containing TLD extensions and pricing</param>
    /// <returns>Import result with statistics</returns>
    public async Task<ImportRegistrarTldsResponseDto> ImportRegistrarTldsAsync(int registrarId, ImportRegistrarTldsDto importDto)
    {
        var importTimestamp = DateTime.UtcNow;
        var tldsAdded = 0;
        var tldsExisting = 0;
        var registrarTldsCreated = 0;
        var registrarTldsExisting = 0;
        var linesSkipped = 0;

        try
        {
            _log.Information("Starting TLD import for registrar {RegistrarId}", registrarId);

            // Verify registrar exists and is active
            var registrar = await _context.Registrars.FindAsync(registrarId);
            if (registrar == null)
            {
                _log.Warning("Registrar with ID {RegistrarId} not found", registrarId);
                return new ImportRegistrarTldsResponseDto
                {
                    Success = false,
                    Message = $"Registrar with ID {registrarId} not found",
                    ImportTimestamp = importTimestamp
                };
            }

            if (!registrar.IsActive)
            {
                _log.Warning("Registrar {RegistrarId} is not active", registrarId);
                return new ImportRegistrarTldsResponseDto
                {
                    Success = false,
                    Message = $"Registrar '{registrar.Name}' is not active",
                    ImportTimestamp = importTimestamp
                };
            }

            // Parse content - CSV format: Tld, Description
            var lines = importDto.Content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            const int batchSize = 100;
            var processedInBatch = 0;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // Skip empty lines, comments, or header line
                if (string.IsNullOrWhiteSpace(trimmedLine) || 
                    trimmedLine.StartsWith("#") || 
                    trimmedLine.StartsWith("//") ||
                    trimmedLine.StartsWith("Tld", StringComparison.OrdinalIgnoreCase))
                {
                    linesSkipped++;
                    continue;
                }

                // Parse CSV format: TLD, Description
                var parts = trimmedLine.Split(',');
                if (parts.Length == 0)
                {
                    linesSkipped++;
                    continue;
                }

                // Normalize extension: remove leading dot and convert to lowercase
                var extension = parts[0].Trim().TrimStart('.').ToLowerInvariant();

                if (string.IsNullOrEmpty(extension))
                {
                    linesSkipped++;
                    continue;
                }

                // Get description from second column if present
                var description = parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1])
                    ? parts[1].Trim()
                    : $"Top-Level Domain: .{extension}";

                // Check if TLD exists in local tracker first, then database
                var tld = _context.Tlds.Local.FirstOrDefault(t => t.Extension == extension);
                if (tld == null)
                {
                    tld = await _context.Tlds.FirstOrDefaultAsync(t => t.Extension == extension);
                }

                if (tld == null)
                {
                    // Add new TLD to Tlds table
                    tld = new Tld
                    {
                        Extension = extension,
                        Description = description,
                        IsActive = importDto.ActivateNewTlds,
                        IsSecondLevel = false,
                        DefaultRegistrationYears = 1,
                        MaxRegistrationYears = 10,
                        RequiresPrivacy = false,
                        RulesUrl = string.Empty,
                        CreatedAt = importTimestamp,
                        UpdatedAt = importTimestamp
                    };

                    _context.Tlds.Add(tld);
                    await _context.SaveChangesAsync(); // Save to get the TLD ID
                    tldsAdded++;
                    _log.Debug("Added new TLD: {Extension} with description: {Description}", extension, description);
                }
                else
                {
                    tldsExisting++;
                }

                // Check if RegistrarTld relationship exists
                var registrarTld = _context.RegistrarTlds.Local
                    .FirstOrDefault(rt => rt.RegistrarId == registrarId && rt.TldId == tld.Id);
                
                if (registrarTld == null)
                {
                    registrarTld = await _context.RegistrarTlds
                        .FirstOrDefaultAsync(rt => rt.RegistrarId == registrarId && rt.TldId == tld.Id);
                }

                if (registrarTld == null)
                {
                    // Create new RegistrarTld relationship (without pricing - that goes in separate tables now)
                    registrarTld = new RegistrarTld
                    {
                        RegistrarId = registrarId,
                        TldId = tld.Id,
                        IsActive = importDto.IsAvailable,
                        AutoRenew = false,
                        MinRegistrationYears = 1,
                        MaxRegistrationYears = 10,
                        Notes = $"Imported on {importTimestamp:yyyy-MM-dd HH:mm:ss} UTC. " +
                                "Pricing should be set via RegistrarTldCostPricing and TldSalesPricing endpoints.",
                        CreatedAt = importTimestamp,
                        UpdatedAt = importTimestamp
                    };

                    _context.RegistrarTlds.Add(registrarTld);
                    registrarTldsCreated++;
                    _log.Debug("Created RegistrarTld for registrar {RegistrarId} and TLD {Extension}", registrarId, extension);
                }
                else
                {
                    registrarTldsExisting++;
                    _log.Debug("RegistrarTld already exists for registrar {RegistrarId} and TLD {Extension}", registrarId, extension);
                }

                processedInBatch++;

                // Save in batches to avoid memory issues
                if (processedInBatch >= batchSize)
                {
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.Clear();
                    processedInBatch = 0;
                }
            }

            // Save remaining changes
            if (processedInBatch > 0)
            {
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
            }

            _log.Information("TLD import completed for registrar {RegistrarId}. TLDs added: {TldsAdded}, RegistrarTlds created: {RegistrarTldsCreated}", 
                registrarId, tldsAdded, registrarTldsCreated);

            return new ImportRegistrarTldsResponseDto
            {
                Success = true,
                Message = "TLD import completed successfully",
                TldsAdded = tldsAdded,
                TldsExisting = tldsExisting,
                RegistrarTldsCreated = registrarTldsCreated,
                RegistrarTldsExisting = registrarTldsExisting,
                LinesSkipped = linesSkipped,
                ImportTimestamp = importTimestamp
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred during TLD import for registrar {RegistrarId}", registrarId);
            return new ImportRegistrarTldsResponseDto
            {
                Success = false,
                Message = $"Error during TLD import: {ex.Message}",
                TldsAdded = tldsAdded,
                TldsExisting = tldsExisting,
                RegistrarTldsCreated = registrarTldsCreated,
                RegistrarTldsExisting = registrarTldsExisting,
                LinesSkipped = linesSkipped,
                ImportTimestamp = importTimestamp
            };
        }
    }

    /// <summary>
    /// Imports TLDs for a registrar from a CSV file stream
    /// </summary>
    /// <param name="registrarId">The registrar ID</param>
    /// <param name="csvStream">The CSV file stream</param>
    /// <param name="defaultRegistrationCost">Default registration cost for imported TLDs</param>
    /// <param name="defaultRegistrationPrice">Default registration price for imported TLDs</param>
    /// <param name="defaultRenewalCost">Default renewal cost for imported TLDs</param>
    /// <param name="defaultRenewalPrice">Default renewal price for imported TLDs</param>
    /// <param name="defaultTransferCost">Default transfer cost for imported TLDs</param>
    /// <param name="defaultTransferPrice">Default transfer price for imported TLDs</param>
    /// <param name="isAvailable">Whether imported TLDs should be marked as available</param>
    /// <param name="activateNewTlds">Whether to activate TLDs that don't exist in the Tlds table</param>
    /// <param name="currency">The currency for pricing</param>
    /// <returns>Import result with statistics</returns>
    public async Task<ImportRegistrarTldsResponseDto> ImportRegistrarTldsFromCsvAsync(
        int registrarId,
        System.IO.Stream csvStream,
        decimal? defaultRegistrationCost,
        decimal? defaultRegistrationPrice,
        decimal? defaultRenewalCost,
        decimal? defaultRenewalPrice,
        decimal? defaultTransferCost,
        decimal? defaultTransferPrice,
        bool isAvailable,
        bool activateNewTlds,
        string currency)
    {
        try
        {
            _log.Information("Reading TLD content from CSV file for registrar {RegistrarId}", registrarId);

            using var reader = new System.IO.StreamReader(csvStream);
            var content = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                _log.Warning("CSV file is empty for registrar {RegistrarId}", registrarId);
                return new ImportRegistrarTldsResponseDto
                {
                    Success = false,
                    Message = "CSV file is empty",
                    ImportTimestamp = DateTime.UtcNow
                };
            }

            var importDto = new ImportRegistrarTldsDto
            {
                Content = content,
                DefaultRegistrationCost = defaultRegistrationCost,
                DefaultRegistrationPrice = defaultRegistrationPrice,
                DefaultRenewalCost = defaultRenewalCost,
                DefaultRenewalPrice = defaultRenewalPrice,
                DefaultTransferCost = defaultTransferCost,
                DefaultTransferPrice = defaultTransferPrice,
                IsAvailable = isAvailable,
                ActivateNewTlds = activateNewTlds,
                Currency = currency
            };

            return await ImportRegistrarTldsAsync(registrarId, importDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while reading CSV file for registrar {RegistrarId}", registrarId);
            return new ImportRegistrarTldsResponseDto
            {
                Success = false,
                Message = $"Error reading CSV file: {ex.Message}",
                ImportTimestamp = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Updates the active status of all registrar-TLD offerings
    /// </summary>
    /// <param name="registrarId">Optional registrar ID to filter by (null updates all registrars)</param>
    /// <param name="isActive">Whether to set all offerings to active or inactive</param>
    /// <returns>Result containing the number of updated records</returns>
    public async Task<BulkUpdateResultDto> BulkUpdateAllRegistrarTldStatusAsync(int? registrarId, bool isActive)
    {
        try
        {
            _log.Information("Bulk updating registrar TLDs for registrar {RegistrarId} to IsActive={IsActive}", 
                registrarId?.ToString() ?? "ALL", isActive);

            var query = _context.RegistrarTlds.AsQueryable();
            
            if (registrarId.HasValue)
            {
                query = query.Where(rt => rt.RegistrarId == registrarId.Value);
            }

            var registrarTlds = await query.ToListAsync();
            var updatedCount = 0;

            foreach (var registrarTld in registrarTlds)
            {
                if (registrarTld.IsActive != isActive)
                {
                    registrarTld.IsActive = isActive;
                    registrarTld.UpdatedAt = DateTime.UtcNow;
                    updatedCount++;
                }
            }

            if (updatedCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            var registrarInfo = registrarId.HasValue ? $" for registrar {registrarId.Value}" : "";
            var message = $"Successfully updated {updatedCount} registrar-TLD offering(s){registrarInfo} to {(isActive ? "active" : "inactive")}";
            _log.Information(message);

            return new BulkUpdateResultDto
            {
                UpdatedCount = updatedCount,
                Message = message
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while bulk updating registrar TLD statuses for registrar {RegistrarId}", 
                registrarId?.ToString() ?? "ALL");
            throw;
        }
    }

    /// <summary>
    /// Updates the active status of registrar-TLD offerings for specific TLD extensions
    /// </summary>
    /// <param name="registrarId">Optional registrar ID to filter by (null updates all registrars)</param>
    /// <param name="tldExtensions">Comma-separated list of TLD extensions</param>
    /// <param name="isActive">Whether to set the offerings to active or inactive</param>
    /// <returns>Result containing the number of updated records</returns>
    public async Task<BulkUpdateResultDto> BulkUpdateRegistrarTldStatusByTldAsync(int? registrarId, string tldExtensions, bool isActive)
    {
        try
        {
            _log.Information("Bulk updating registrar TLDs for registrar {RegistrarId} and extensions '{TldExtensions}' to IsActive={IsActive}", 
                registrarId?.ToString() ?? "ALL", tldExtensions, isActive);

            if (string.IsNullOrWhiteSpace(tldExtensions))
            {
                _log.Warning("TLD extensions list is empty");
                return new BulkUpdateResultDto
                {
                    UpdatedCount = 0,
                    Message = "No TLD extensions provided"
                };
            }

            // Parse and normalize the TLD extensions
            var extensions = tldExtensions
                .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ext => ext.Trim().TrimStart('.').ToLowerInvariant())
                .Where(ext => !string.IsNullOrEmpty(ext))
                .Distinct()
                .ToList();

            if (!extensions.Any())
            {
                _log.Warning("No valid TLD extensions found after parsing");
                return new BulkUpdateResultDto
                {
                    UpdatedCount = 0,
                    Message = "No valid TLD extensions found"
                };
            }

            // Get all TLD IDs for the specified extensions
            var tldIds = await _context.Tlds
                .Where(t => extensions.Contains(t.Extension))
                .Select(t => t.Id)
                .ToListAsync();

            if (!tldIds.Any())
            {
                _log.Warning("No TLDs found matching the provided extensions");
                return new BulkUpdateResultDto
                {
                    UpdatedCount = 0,
                    Message = "No TLDs found matching the provided extensions"
                };
            }

            // Build query with optional registrar filter
            var query = _context.RegistrarTlds.Where(rt => tldIds.Contains(rt.TldId));
            
            if (registrarId.HasValue)
            {
                query = query.Where(rt => rt.RegistrarId == registrarId.Value);
            }

            var registrarTlds = await query.ToListAsync();
            var updatedCount = 0;

            foreach (var registrarTld in registrarTlds)
            {
                if (registrarTld.IsActive != isActive)
                {
                    registrarTld.IsActive = isActive;
                    registrarTld.UpdatedAt = DateTime.UtcNow;
                    updatedCount++;
                }
            }

            if (updatedCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            var registrarInfo = registrarId.HasValue ? $" for registrar {registrarId.Value}" : "";
            var message = $"Successfully updated {updatedCount} registrar-TLD offering(s){registrarInfo} for {extensions.Count} TLD extension(s) to {(isActive ? "active" : "inactive")}";
            _log.Information(message);

            return new BulkUpdateResultDto
            {
                UpdatedCount = updatedCount,
                Message = message
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while bulk updating registrar TLD statuses for registrar {RegistrarId} and extensions '{TldExtensions}'", 
                registrarId?.ToString() ?? "ALL", tldExtensions);
            throw;
        }
    }
}



