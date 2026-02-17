using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services.Helpers;

/// <summary>
/// Helper service for migrating existing DomainContact data to the new hybrid system
/// </summary>
public class DomainContactMigrationHelper
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<DomainContactMigrationHelper>();

    public DomainContactMigrationHelper(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Migrates existing DomainContact records to the hybrid system by:
    /// 1. Extracting unique contacts to ContactPerson table
    /// 2. Creating DomainContactAssignment entries
    /// 3. Linking existing DomainContacts to their source ContactPerson
    /// </summary>
    /// <returns>Migration statistics</returns>
    public async Task<MigrationResult> MigrateToHybridSystemAsync()
    {
        var result = new MigrationResult();
        
        try
        {
            _log.Information("Starting migration to hybrid domain contact system");

            // Step 1: Extract unique contacts from DomainContacts
            _log.Information("Step 1: Extracting unique contacts to ContactPerson table");
            result.ContactPersonsCreated = await ExtractUniqueContactsAsync();

            // Step 2: Create DomainContactAssignments
            _log.Information("Step 2: Creating domain contact assignments");
            result.AssignmentsCreated = await CreateAssignmentsAsync();

            // Step 3: Link DomainContacts to ContactPersons
            _log.Information("Step 3: Linking domain contacts to source contact persons");
            result.DomainContactsLinked = await LinkDomainContactsAsync();

            // Step 4: Mark all as current versions
            _log.Information("Step 4: Marking domain contacts as current versions");
            result.DomainContactsUpdated = await MarkAsCurrentVersionAsync();

            _log.Information("Migration completed successfully. ContactPersons created: {ContactPersonsCreated}, " +
                           "Assignments created: {AssignmentsCreated}, DomainContacts linked: {DomainContactsLinked}, " +
                           "DomainContacts updated: {DomainContactsUpdated}",
                result.ContactPersonsCreated, result.AssignmentsCreated, 
                result.DomainContactsLinked, result.DomainContactsUpdated);

            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred during migration to hybrid system");
            result.Success = false;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    /// <summary>
    /// Extracts unique contacts from DomainContacts and creates ContactPerson records
    /// </summary>
    private async Task<int> ExtractUniqueContactsAsync()
    {
        var existingEmails = await _context.ContactPersons
            .Select(cp => cp.Email.ToLower())
            .ToListAsync();

        var uniqueContacts = await _context.DomainContacts
            .Where(dc => !existingEmails.Contains(dc.Email.ToLower()))
            .GroupBy(dc => new { dc.Email, dc.FirstName, dc.LastName })
            .Select(g => g.First())
            .ToListAsync();

        var contactPersons = uniqueContacts.Select(dc => new ContactPerson
        {
            FirstName = dc.FirstName,
            LastName = dc.LastName,
            Email = dc.Email,
            Phone = dc.Phone,
            Department = dc.Organization,
            IsActive = dc.IsActive,
            IsPrimary = false,
            Notes = $"Migrated from DomainContact on {DateTime.UtcNow:yyyy-MM-dd}"
        }).ToList();

        if (contactPersons.Any())
        {
            _context.ContactPersons.AddRange(contactPersons);
            await _context.SaveChangesAsync();
        }

        return contactPersons.Count;
    }

    /// <summary>
    /// Creates DomainContactAssignment entries for all existing DomainContacts
    /// </summary>
    private async Task<int> CreateAssignmentsAsync()
    {
        var domainContacts = await _context.DomainContacts
            .Include(dc => dc.Domain)
            .ToListAsync();

        var assignments = new List<DomainContactAssignment>();

        foreach (var dc in domainContacts)
        {
            var contactPerson = await _context.ContactPersons
                .FirstOrDefaultAsync(cp => cp.Email.ToLower() == dc.Email.ToLower());

            if (contactPerson != null)
            {
                // Check if assignment already exists
                var existingAssignment = await _context.DomainContactAssignments
                    .AnyAsync(a => a.RegisteredDomainId == dc.DomainId && 
                                  a.ContactPersonId == contactPerson.Id && 
                                  a.RoleType == dc.RoleType);

                if (!existingAssignment)
                {
                    assignments.Add(new DomainContactAssignment
                    {
                        RegisteredDomainId = dc.DomainId,
                        ContactPersonId = contactPerson.Id,
                        RoleType = dc.RoleType,
                        AssignedAt = dc.CreatedAt,
                        IsActive = dc.IsActive,
                        Notes = "Migrated from existing DomainContact",
                    });
                }
            }
        }

        if (assignments.Any())
        {
            _context.DomainContactAssignments.AddRange(assignments);
            await _context.SaveChangesAsync();
        }

        return assignments.Count;
    }

    /// <summary>
    /// Links existing DomainContacts to their source ContactPerson
    /// </summary>
    private async Task<int> LinkDomainContactsAsync()
    {
        var domainContacts = await _context.DomainContacts
            .Where(dc => dc.SourceContactPersonId == null)
            .ToListAsync();

        var updated = 0;

        foreach (var dc in domainContacts)
        {
            var contactPerson = await _context.ContactPersons
                .FirstOrDefaultAsync(cp => cp.Email.ToLower() == dc.Email.ToLower());

            if (contactPerson != null)
            {
                dc.SourceContactPersonId = contactPerson.Id;
                updated++;
            }
        }

        if (updated > 0)
        {
            await _context.SaveChangesAsync();
        }

        return updated;
    }

    /// <summary>
    /// Marks all existing DomainContacts as current versions
    /// </summary>
    private async Task<int> MarkAsCurrentVersionAsync()
    {
        var domainContacts = await _context.DomainContacts
            .Where(dc => !dc.IsCurrentVersion)
            .ToListAsync();

        foreach (var dc in domainContacts)
        {
            dc.IsCurrentVersion = true;
        }

        if (domainContacts.Any())
        {
            await _context.SaveChangesAsync();
        }

        return domainContacts.Count;
    }

    /// <summary>
    /// Checks if migration has already been performed
    /// </summary>
    public async Task<bool> IsMigrationNeededAsync()
    {
        // Check if there are any DomainContacts without SourceContactPersonId
        var unmigrated = await _context.DomainContacts
            .AnyAsync(dc => dc.SourceContactPersonId == null);

        // Check if there are any DomainContactAssignments
        var hasAssignments = await _context.DomainContactAssignments.AnyAsync();

        return unmigrated || !hasAssignments;
    }

    /// <summary>
    /// Gets a preview of what the migration would do without actually performing it
    /// </summary>
    public async Task<MigrationPreview> GetMigrationPreviewAsync()
    {
        var preview = new MigrationPreview();

        // Count existing contact persons
        preview.ExistingContactPersons = await _context.ContactPersons.CountAsync();

        // Count unique contacts in DomainContacts
        var existingEmails = await _context.ContactPersons
            .Select(cp => cp.Email.ToLower())
            .ToListAsync();

        preview.UniqueContactsToMigrate = await _context.DomainContacts
            .Where(dc => !existingEmails.Contains(dc.Email.ToLower()))
            .Select(dc => dc.Email)
            .Distinct()
            .CountAsync();

        // Count domain contacts
        preview.TotalDomainContacts = await _context.DomainContacts.CountAsync();

        // Count potential assignments
        preview.AssignmentsToCreate = await _context.DomainContacts
            .Select(dc => new { dc.DomainId, dc.Email, dc.RoleType })
            .Distinct()
            .CountAsync();

        // Count unlinked domain contacts
        preview.UnlinkedDomainContacts = await _context.DomainContacts
            .CountAsync(dc => dc.SourceContactPersonId == null);

        return preview;
    }
}

/// <summary>
/// Result of the migration operation
/// </summary>
public class MigrationResult
{
    public bool Success { get; set; } = true;
    public int ContactPersonsCreated { get; set; }
    public int AssignmentsCreated { get; set; }
    public int DomainContactsLinked { get; set; }
    public int DomainContactsUpdated { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Preview of what the migration would do
/// </summary>
public class MigrationPreview
{
    public int ExistingContactPersons { get; set; }
    public int UniqueContactsToMigrate { get; set; }
    public int TotalDomainContacts { get; set; }
    public int AssignmentsToCreate { get; set; }
    public int UnlinkedDomainContacts { get; set; }
}
