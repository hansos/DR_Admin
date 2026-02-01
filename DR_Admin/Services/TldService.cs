using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class TldService : ITldService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<TldService>();

    public TldService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TldDto>> GetAllTldsAsync()
    {
        try
        {
            _log.Information("Fetching all TLDs");
            
            var tlds = await _context.Tlds
                .AsNoTracking()
                .OrderBy(t => t.Extension)
                .ToListAsync();

            var tldDtos = tlds.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} TLDs", tlds.Count);
            return tldDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all TLDs");
            throw;
        }
    }

    public async Task<IEnumerable<TldDto>> GetAllTldsAsync(bool isSecondLevel)
    {
        try
        {
            _log.Information("Fetching TLDs filtered by IsSecondLevel: {IsSecondLevel}", isSecondLevel);
            
            var tlds = await _context.Tlds
                .AsNoTracking()
                .Where(t => t.IsSecondLevel == isSecondLevel)
                .OrderBy(t => t.Extension)
                .ToListAsync();

            var tldDtos = tlds.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} TLDs with IsSecondLevel: {IsSecondLevel}", tlds.Count, isSecondLevel);
            return tldDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching TLDs filtered by IsSecondLevel: {IsSecondLevel}", isSecondLevel);
            throw;
        }
    }

    public async Task<IEnumerable<TldDto>> GetActiveTldsAsync()
    {
        try
        {
            _log.Information("Fetching active TLDs");
            
            var tlds = await _context.Tlds
                .AsNoTracking()
                .Where(t => t.IsActive)
                .OrderBy(t => t.Extension)
                .ToListAsync();

            var tldDtos = tlds.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} active TLDs", tlds.Count);
            return tldDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active TLDs");
            throw;
        }
    }

    public async Task<TldDto?> GetTldByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching TLD with ID: {TldId}", id);
            
            var tld = await _context.Tlds
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tld == null)
            {
                _log.Warning("TLD with ID {TldId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched TLD with ID: {TldId}", id);
            return MapToDto(tld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching TLD with ID: {TldId}", id);
            throw;
        }
    }

    public async Task<TldDto?> GetTldByExtensionAsync(string extension)
    {
        try
        {
            // Normalize extension: remove leading dot and surrounding whitespace
            var ext = extension?.Trim().TrimStart('.') ?? string.Empty;
            _log.Information("Fetching TLD with extension: {Extension}", ext);

            var tld = await _context.Tlds
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Extension == ext);

            if (tld == null)
            {
                _log.Warning("TLD with extension {Extension} not found", extension);
                return null;
            }

            _log.Information("Successfully fetched TLD with extension: {Extension}", ext);
            return MapToDto(tld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching TLD with extension: {Extension}", extension);
            throw;
        }
    }

    public async Task<TldDto> CreateTldAsync(CreateTldDto createDto)
    {
        try
        {
            // Normalize extension: remove leading dot and surrounding whitespace
            var extension = createDto.Extension?.Trim().TrimStart('.') ?? string.Empty;

            _log.Information("Creating new TLD with extension: {Extension}", extension);

            if (string.IsNullOrEmpty(extension))
            {
                _log.Warning("TLD extension is empty or invalid");
                throw new InvalidOperationException("TLD extension cannot be empty");
            }

            var existingTld = await _context.Tlds
                .FirstOrDefaultAsync(t => t.Extension == extension);

            if (existingTld != null)
            {
                _log.Warning("TLD with extension {Extension} already exists", extension);
                throw new InvalidOperationException($"TLD with extension {extension} already exists");
            }

            var tld = new Tld
            {
                Extension = extension,
                Description = createDto.Description,
                IsActive = createDto.IsActive,
                DefaultRegistrationYears = createDto.DefaultRegistrationYears,
                MaxRegistrationYears = createDto.MaxRegistrationYears,
                RequiresPrivacy = createDto.RequiresPrivacy,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tlds.Add(tld);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created TLD with ID: {TldId} and extension: {Extension}", tld.Id, tld.Extension);
            return MapToDto(tld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating TLD with extension: {Extension}", createDto.Extension);
            throw;
        }
    }

    public async Task<TldDto?> UpdateTldAsync(int id, UpdateTldDto updateDto)
    {
        try
        {
            _log.Information("Updating TLD with ID: {TldId}", id);

            var tld = await _context.Tlds.FindAsync(id);

            if (tld == null)
            {
                _log.Warning("TLD with ID {TldId} not found for update", id);
                return null;
            }

            // Normalize extension: remove leading dot and surrounding whitespace
            var extension = updateDto.Extension?.Trim().TrimStart('.') ?? string.Empty;

            var duplicateExtension = await _context.Tlds
                .AnyAsync(t => t.Extension == extension && t.Id != id);

            if (duplicateExtension)
            {
                _log.Warning("Cannot update TLD {TldId}: extension {Extension} already exists", id, extension);
                throw new InvalidOperationException($"TLD with extension {extension} already exists");
            }

            tld.Extension = extension;
            tld.Description = updateDto.Description;
            tld.IsActive = updateDto.IsActive;
            tld.DefaultRegistrationYears = updateDto.DefaultRegistrationYears;
            tld.MaxRegistrationYears = updateDto.MaxRegistrationYears;
            tld.RequiresPrivacy = updateDto.RequiresPrivacy;
            tld.Notes = updateDto.Notes;
            tld.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated TLD with ID: {TldId}", id);
            return MapToDto(tld);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating TLD with ID: {TldId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteTldAsync(int id)
    {
        try
        {
            _log.Information("Deleting TLD with ID: {TldId}", id);

            var tld = await _context.Tlds.FindAsync(id);

            if (tld == null)
            {
                _log.Warning("TLD with ID {TldId} not found for deletion", id);
                return false;
            }

            var hasRegistrarTlds = await _context.RegistrarTlds.AnyAsync(rt => rt.TldId == id);
            if (hasRegistrarTlds)
            {
                _log.Warning("Cannot delete TLD {TldId}: has associated registrar TLDs", id);
                throw new InvalidOperationException("Cannot delete TLD with associated registrar offerings");
            }

            _context.Tlds.Remove(tld);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted TLD with ID: {TldId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting TLD with ID: {TldId}", id);
            throw;
        }
    }

    /// <summary>
    /// Synchronizes TLDs from IANA's official TLD list
    /// </summary>
    /// <param name="request">Synchronization configuration options</param>
    /// <returns>Synchronization result with statistics</returns>
    public async Task<TldSyncResponseDto> SyncTldsFromIanaAsync(TldSyncRequestDto request)
    {
        const string ianaUrl = "https://data.iana.org/TLD/tlds-alpha-by-domain.txt";
        var syncTimestamp = DateTime.UtcNow;
        var tldsAdded = 0;
        var tldsUpdated = 0;
        var totalTldsInSource = 0;

        try
        {
            _log.Information("Starting TLD synchronization from IANA source: {IanaUrl}", ianaUrl);

            if (request.MarkAllInactiveBeforeSync)
            {
                _log.Information("Marking all existing TLDs as inactive before sync");
                var allTlds = await _context.Tlds.ToListAsync();
                foreach (var tld in allTlds)
                {
                    if (tld.IsActive)
                    {
                        tld.IsActive = false;
                        tld.UpdatedAt = syncTimestamp;
                    }
                }
                await _context.SaveChangesAsync();
                _log.Information("Marked {Count} TLDs as inactive", allTlds.Count(t => !t.IsActive));
            }

            // Download TLD list from IANA
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            
            _log.Information("Downloading TLD list from IANA");
            var response = await httpClient.GetAsync(ianaUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Process each line
            const int batchSize = 100;
            var processedInBatch = 0;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // Skip comments (lines starting with #)
                if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("#"))
                    continue;

                totalTldsInSource++;

                // Normalize to lowercase for consistency
                var extension = trimmedLine.ToLowerInvariant();

                // Check if TLD exists in local tracker first, then database
                var tld = _context.Tlds.Local.FirstOrDefault(t => t.Extension == extension);
                if (tld == null)
                {
                    tld = await _context.Tlds.FirstOrDefaultAsync(t => t.Extension == extension);
                }

                if (tld == null)
                {
                    // Add new TLD
                    tld = new Tld
                    {
                        Extension = extension,
                        Description = $"Top-Level Domain: .{extension}",
                        IsActive = request.ActivateNewTlds,
                        IsSecondLevel = false,
                        DefaultRegistrationYears = 1,
                        MaxRegistrationYears = 10,
                        RequiresPrivacy = false,
                        RulesUrl = string.Empty,
                        CreatedAt = syncTimestamp,
                        UpdatedAt = syncTimestamp
                    };

                    _context.Tlds.Add(tld);
                    tldsAdded++;
                    _log.Debug("Adding new TLD: {Extension}", extension);
                }
                else
                {
                    // Update existing TLD (reactivate if requested)
                    var changed = false;

                    if (request.MarkAllInactiveBeforeSync && !tld.IsActive)
                    {
                        tld.IsActive = true;
                        changed = true;
                    }

                    if (changed)
                    {
                        tld.UpdatedAt = syncTimestamp;
                        tldsUpdated++;
                        _log.Debug("Updated TLD: {Extension}", extension);
                    }
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

            _log.Information("TLD synchronization completed successfully. Added: {Added}, Updated: {Updated}, Total in source: {Total}", 
                tldsAdded, tldsUpdated, totalTldsInSource);

            return new TldSyncResponseDto
            {
                Success = true,
                Message = "TLD synchronization completed successfully",
                TldsAdded = tldsAdded,
                TldsUpdated = tldsUpdated,
                TotalTldsInSource = totalTldsInSource,
                SyncTimestamp = syncTimestamp
            };
        }
        catch (HttpRequestException ex)
        {
            _log.Error(ex, "HTTP error occurred while downloading TLD list from IANA");
            return new TldSyncResponseDto
            {
                Success = false,
                Message = $"Failed to download TLD list from IANA: {ex.Message}",
                TldsAdded = tldsAdded,
                TldsUpdated = tldsUpdated,
                TotalTldsInSource = totalTldsInSource,
                SyncTimestamp = syncTimestamp
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred during TLD synchronization");
            return new TldSyncResponseDto
            {
                Success = false,
                Message = $"Error during TLD synchronization: {ex.Message}",
                TldsAdded = tldsAdded,
                TldsUpdated = tldsUpdated,
                TotalTldsInSource = totalTldsInSource,
                SyncTimestamp = syncTimestamp
            };
        }
    }

    private static TldDto MapToDto(Tld tld)
    {
        return new TldDto
        {
            Id = tld.Id,
            Extension = tld.Extension,
            Description = tld.Description,
            IsActive = tld.IsActive,
            DefaultRegistrationYears = tld.DefaultRegistrationYears,
            MaxRegistrationYears = tld.MaxRegistrationYears,
            RequiresPrivacy = tld.RequiresPrivacy,
            Notes = tld.Notes,
            CreatedAt = tld.CreatedAt,
            UpdatedAt = tld.UpdatedAt
        };
    }

    public async Task<IEnumerable<RegistrarDto>> GetRegistrarsByTldAsync(int tldId)
    {
        try
        {
            _log.Information("Fetching registrars for TLD {TldId}", tldId);

            var registrars = await _context.RegistrarTlds
                .AsNoTracking()
                .Where(rt => rt.TldId == tldId)
                .Include(rt => rt.Registrar)
                .Select(rt => rt.Registrar)
                .OrderBy(r => r.Name)
                .ToListAsync();

            var registrarDtos = registrars.Select(r => new RegistrarDto
            {
                Id = r.Id,
                Name = r.Name,
                Code = r.Code,
                IsActive = r.IsActive,
                ContactEmail = r.ContactEmail,
                ContactPhone = r.ContactPhone,
                Website = r.Website,
                Notes = r.Notes,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            });

            _log.Information("Successfully fetched {Count} registrars for TLD {TldId}", registrars.Count, tldId);
            return registrarDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching registrars for TLD {TldId}", tldId);
            throw;
        }
    }

    /// <summary>
    /// Synchronizes second-level domains from the Public Suffix List
    /// </summary>
    /// <returns>Synchronization result with statistics</returns>
    public async Task<SecondLevelDomainSyncResponseDto> SyncSecondLevelDomainsAsync()
    {
        const string publicSuffixUrl = "https://publicsuffix.org/list/public_suffix_list.dat";
        var syncTimestamp = DateTime.UtcNow;
        var secondLevelDomainsAdded = 0;
        var parentTldsProcessed = 0;
        var parentTldsSkipped = 0;

        try
        {
            _log.Information("Starting second-level domain synchronization from Public Suffix List: {Url}", publicSuffixUrl);

            // Check if TLD table is empty
            var hasTlds = await _context.Tlds.AnyAsync();
            if (!hasTlds)
            {
                _log.Warning("TLD table is empty. Cannot sync second-level domains without base TLDs.");
                return new SecondLevelDomainSyncResponseDto
                {
                    Success = false,
                    Message = "TLD table is empty. Please import TLDs first using the sync-tlds endpoint.",
                    SecondLevelDomainsAdded = 0,
                    ParentTldsProcessed = 0,
                    ParentTldsSkipped = 0,
                    SyncTimestamp = syncTimestamp
                };
            }

            // Download Public Suffix List
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            
            _log.Information("Downloading Public Suffix List");
            var response = await httpClient.GetAsync(publicSuffixUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Find ICANN DOMAINS section
            var inIcannSection = false;
            string? currentTld = null;
            var secondLevelDomains = new List<string>();
            const int batchSize = 100;
            var processedInBatch = 0;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // Check for section boundaries
                if (trimmedLine.Contains("===BEGIN ICANN DOMAINS==="))
                {
                    inIcannSection = true;
                    _log.Information("Found ICANN DOMAINS section");
                    continue;
                }

                if (trimmedLine.Contains("===END ICANN DOMAINS==="))
                {
                    _log.Information("Reached end of ICANN DOMAINS section");
                    break;
                }

                // Only process lines within ICANN section
                if (!inIcannSection)
                    continue;

                // Skip comments and empty lines
                if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("//"))
                    continue;

                var entry = trimmedLine.ToLowerInvariant();

                // Check if this is a TLD (no dots) or second-level domain (contains dots)
                if (!entry.Contains('.'))
                {
                    // Process previous TLD's second-level domains if any
                    if (currentTld != null && secondLevelDomains.Count > 0)
                    {
                        var result = await ProcessSecondLevelDomainsForTld(currentTld, secondLevelDomains, syncTimestamp);
                        if (result.processed)
                        {
                            parentTldsProcessed++;
                            secondLevelDomainsAdded += result.added;
                        }
                        else
                        {
                            parentTldsSkipped++;
                        }

                        processedInBatch++;
                        if (processedInBatch >= batchSize)
                        {
                            await _context.SaveChangesAsync();
                            _context.ChangeTracker.Clear();
                            processedInBatch = 0;
                        }

                        secondLevelDomains.Clear();
                    }

                    // Set new current TLD
                    currentTld = entry;
                }
                else
                {
                    // This is a second-level domain
                    if (currentTld != null)
                    {
                        secondLevelDomains.Add(entry);
                    }
                }
            }

            // Process the last TLD's second-level domains
            if (currentTld != null && secondLevelDomains.Count > 0)
            {
                var result = await ProcessSecondLevelDomainsForTld(currentTld, secondLevelDomains, syncTimestamp);
                if (result.processed)
                {
                    parentTldsProcessed++;
                    secondLevelDomainsAdded += result.added;
                }
                else
                {
                    parentTldsSkipped++;
                }
            }

            // Save remaining changes
            if (processedInBatch > 0)
            {
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
            }

            _log.Information("Second-level domain synchronization completed. Added: {Added}, Processed: {Processed}, Skipped: {Skipped}", 
                secondLevelDomainsAdded, parentTldsProcessed, parentTldsSkipped);

            return new SecondLevelDomainSyncResponseDto
            {
                Success = true,
                Message = "Second-level domain synchronization completed successfully",
                SecondLevelDomainsAdded = secondLevelDomainsAdded,
                ParentTldsProcessed = parentTldsProcessed,
                ParentTldsSkipped = parentTldsSkipped,
                SyncTimestamp = syncTimestamp
            };
        }
        catch (HttpRequestException ex)
        {
            _log.Error(ex, "HTTP error occurred while downloading Public Suffix List");
            return new SecondLevelDomainSyncResponseDto
            {
                Success = false,
                Message = $"Failed to download Public Suffix List: {ex.Message}",
                SecondLevelDomainsAdded = secondLevelDomainsAdded,
                ParentTldsProcessed = parentTldsProcessed,
                ParentTldsSkipped = parentTldsSkipped,
                SyncTimestamp = syncTimestamp
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred during second-level domain synchronization");
            return new SecondLevelDomainSyncResponseDto
            {
                Success = false,
                Message = $"Error during second-level domain synchronization: {ex.Message}",
                SecondLevelDomainsAdded = secondLevelDomainsAdded,
                ParentTldsProcessed = parentTldsProcessed,
                ParentTldsSkipped = parentTldsSkipped,
                SyncTimestamp = syncTimestamp
            };
        }
    }

    /// <summary>
    /// Processes second-level domains for a specific TLD
    /// </summary>
    private async Task<(bool processed, int added)> ProcessSecondLevelDomainsForTld(
        string tld, 
        List<string> secondLevelDomains, 
        DateTime syncTimestamp)
    {
        // Check if parent TLD exists in database
        var parentTld = _context.Tlds.Local.FirstOrDefault(t => t.Extension == tld);
        if (parentTld == null)
        {
            parentTld = await _context.Tlds.FirstOrDefaultAsync(t => t.Extension == tld);
        }

        if (parentTld == null)
        {
            _log.Debug("Parent TLD {Tld} not found in database, skipping its second-level domains", tld);
            return (false, 0);
        }

        var added = 0;

        foreach (var sld in secondLevelDomains)
        {
            // Check if second-level domain already exists
            var existing = _context.Tlds.Local.FirstOrDefault(t => t.Extension == sld);
            if (existing == null)
            {
                existing = await _context.Tlds.FirstOrDefaultAsync(t => t.Extension == sld);
            }

            if (existing == null)
            {
                // Add new second-level domain
                var newSld = new Tld
                {
                    Extension = sld,
                    Description = $"Second-Level Domain: {sld}",
                    IsActive = false, // Not activated by default
                    IsSecondLevel = true,
                    DefaultRegistrationYears = parentTld.DefaultRegistrationYears ?? 1,
                    MaxRegistrationYears = parentTld.MaxRegistrationYears ?? 10,
                    RequiresPrivacy = parentTld.RequiresPrivacy,
                    RulesUrl = string.Empty,
                    CreatedAt = syncTimestamp,
                    UpdatedAt = syncTimestamp
                };

                _context.Tlds.Add(newSld);
                added++;
                _log.Debug("Adding second-level domain: {Sld} for parent TLD: {Tld}", sld, tld);
            }
        }

        return (true, added);
    }
}
