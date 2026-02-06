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

    public async Task<IEnumerable<RegistrarTldDto>> GetAvailableRegistrarTldsAsync()
    {
        try
        {
            _log.Information("Fetching available registrar TLDs");
            
            var registrarTlds = await _context.RegistrarTlds
                .AsNoTracking()
                .Include(rt => rt.Registrar)
                .Include(rt => rt.Tld)
                .Where(rt => rt.IsAvailable && rt.Registrar.IsActive && rt.Tld.IsActive)
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
                RegistrationCost = createDto.RegistrationCost,
                RegistrationPrice = createDto.RegistrationPrice,
                RenewalCost = createDto.RenewalCost,
                RenewalPrice = createDto.RenewalPrice,
                TransferCost = createDto.TransferCost,
                TransferPrice = createDto.TransferPrice,
                PrivacyCost = createDto.PrivacyCost,
                PrivacyPrice = createDto.PrivacyPrice,
                Currency = createDto.Currency,
                IsAvailable = createDto.IsAvailable,
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

            _log.Information("Successfully created registrar TLD with ID: {RegistrarTldId}", registrarTld.Id);
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

            var registrarExists = await _context.Registrars.AnyAsync(r => r.Id == updateDto.RegistrarId);
            if (!registrarExists)
            {
                _log.Warning("Registrar with ID {RegistrarId} does not exist", updateDto.RegistrarId);
                throw new InvalidOperationException($"Registrar with ID {updateDto.RegistrarId} does not exist");
            }

            var tldExists = await _context.Tlds.AnyAsync(t => t.Id == updateDto.TldId);
            if (!tldExists)
            {
                _log.Warning("TLD with ID {TldId} does not exist", updateDto.TldId);
                throw new InvalidOperationException($"TLD with ID {updateDto.TldId} does not exist");
            }

            var duplicateCombination = await _context.RegistrarTlds
                .AnyAsync(rt => rt.RegistrarId == updateDto.RegistrarId && rt.TldId == updateDto.TldId && rt.Id != id);

            if (duplicateCombination)
            {
                _log.Warning("Cannot update registrar TLD {RegistrarTldId}: combination already exists", id);
                throw new InvalidOperationException("This registrar-TLD combination already exists");
            }

            registrarTld.RegistrarId = updateDto.RegistrarId;
            registrarTld.TldId = updateDto.TldId;
            registrarTld.RegistrationCost = updateDto.RegistrationCost;
            registrarTld.RegistrationPrice = updateDto.RegistrationPrice;
            registrarTld.RenewalCost = updateDto.RenewalCost;
            registrarTld.RenewalPrice = updateDto.RenewalPrice;
            registrarTld.TransferCost = updateDto.TransferCost;
            registrarTld.TransferPrice = updateDto.TransferPrice;
            registrarTld.PrivacyCost = updateDto.PrivacyCost;
            registrarTld.PrivacyPrice = updateDto.PrivacyPrice;
            registrarTld.Currency = updateDto.Currency;
            registrarTld.IsAvailable = updateDto.IsAvailable;
            registrarTld.AutoRenew = updateDto.AutoRenew;
            registrarTld.MinRegistrationYears = updateDto.MinRegistrationYears;
            registrarTld.MaxRegistrationYears = updateDto.MaxRegistrationYears;
            registrarTld.Notes = updateDto.Notes;
            registrarTld.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated registrar TLD with ID: {RegistrarTldId}", id);
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
            RegistrationCost = registrarTld.RegistrationCost,
            RegistrationPrice = registrarTld.RegistrationPrice,
            RenewalCost = registrarTld.RenewalCost,
            RenewalPrice = registrarTld.RenewalPrice,
            TransferCost = registrarTld.TransferCost,
            TransferPrice = registrarTld.TransferPrice,
            PrivacyCost = registrarTld.PrivacyCost,
            PrivacyPrice = registrarTld.PrivacyPrice,
            Currency = registrarTld.Currency,
            IsAvailable = registrarTld.IsAvailable,
            AutoRenew = registrarTld.AutoRenew,
            MinRegistrationYears = registrarTld.MinRegistrationYears,
            MaxRegistrationYears = registrarTld.MaxRegistrationYears,
            Notes = registrarTld.Notes,
            CreatedAt = registrarTld.CreatedAt,
            UpdatedAt = registrarTld.UpdatedAt
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
                    // Create new RegistrarTld relationship
                    registrarTld = new RegistrarTld
                    {
                        RegistrarId = registrarId,
                        TldId = tld.Id,
                        RegistrationCost = importDto.DefaultRegistrationCost ?? 0,
                        RegistrationPrice = importDto.DefaultRegistrationPrice ?? 0,
                        RenewalCost = importDto.DefaultRenewalCost ?? 0,
                        RenewalPrice = importDto.DefaultRenewalPrice ?? 0,
                        TransferCost = importDto.DefaultTransferCost ?? 0,
                        TransferPrice = importDto.DefaultTransferPrice ?? 0,
                        PrivacyCost = null,
                        PrivacyPrice = null,
                        Currency = importDto.Currency,
                        IsAvailable = importDto.IsAvailable,
                        AutoRenew = false,
                        MinRegistrationYears = 1,
                        MaxRegistrationYears = 10,
                        Notes = $"Imported on {importTimestamp:yyyy-MM-dd HH:mm:ss} UTC",
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
}

