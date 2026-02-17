using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;

namespace ISPAdmin.Services.Helpers;

/// <summary>
/// Helper service for syncing domain contacts FROM registrar into the hybrid contact system
/// Handles inserting/updating ContactPerson, DomainContactAssignment, and DomainContact records
/// </summary>
public class RegistrarContactSyncHelper
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarContactSyncHelper>();

    public RegistrarContactSyncHelper(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Syncs a single contact from registrar data into the hybrid system
    /// </summary>
    /// <param name="request">The sync request containing registrar contact data</param>
    /// <returns>The sync result with created/updated entity IDs</returns>
    public async Task<ContactSyncResult> SyncContactFromRegistrarAsync(RegistrarContactSyncRequest request)
    {
        var result = new ContactSyncResult
        {
            DomainId = request.RegisteredDomainId,
            RoleType = request.RoleType
        };

        try
        {
            _log.Information("Starting sync for {RoleType} contact on domain {DomainId} from registrar {Registrar}",
                request.RoleType, request.RegisteredDomainId, request.RegistrarType);

            // Step 1: Find or create ContactPerson (master data)
            var contactPerson = await FindOrCreateContactPersonAsync(request);
            result.ContactPersonId = contactPerson.Id;
            result.ContactPersonCreated = contactPerson.Id == 0;

            // Step 2: Find or create DomainContactAssignment (bridge)
            var assignment = await FindOrCreateAssignmentAsync(request.RegisteredDomainId, contactPerson.Id, request.RoleType);
            result.AssignmentId = assignment.Id;
            result.AssignmentCreated = assignment.Id == 0;

            // Step 3: Create or update DomainContact (snapshot)
            var domainContact = await CreateOrUpdateDomainContactAsync(request, contactPerson.Id);
            result.DomainContactId = domainContact.Id;
            result.DomainContactCreated = domainContact.Id == 0;

            await _context.SaveChangesAsync();

            result.Success = true;
            _log.Information("Successfully synced {RoleType} contact for domain {DomainId}. " +
                           "ContactPerson: {ContactPersonId} (Created: {CPCreated}), " +
                           "Assignment: {AssignmentId} (Created: {ACreated}), " +
                           "DomainContact: {DomainContactId} (Created: {DCCreated})",
                request.RoleType, request.RegisteredDomainId,
                result.ContactPersonId, result.ContactPersonCreated,
                result.AssignmentId, result.AssignmentCreated,
                result.DomainContactId, result.DomainContactCreated);

            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error syncing {RoleType} contact for domain {DomainId} from registrar",
                request.RoleType, request.RegisteredDomainId);
            result.Success = false;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    /// <summary>
    /// Syncs all contacts for a domain from registrar data
    /// </summary>
    /// <param name="registeredDomainId">The domain ID</param>
    /// <param name="registrarContacts">List of contacts from registrar</param>
    /// <param name="registrarType">The registrar identifier</param>
    /// <returns>Batch sync result</returns>
    public async Task<BatchContactSyncResult> SyncAllContactsForDomainAsync(
        int registeredDomainId,
        List<RegistrarContactSyncRequest> registrarContacts,
        string registrarType)
    {
        var batchResult = new BatchContactSyncResult
        {
            DomainId = registeredDomainId,
            TotalContacts = registrarContacts.Count
        };

        _log.Information("Starting batch sync of {Count} contacts for domain {DomainId} from registrar {Registrar}",
            registrarContacts.Count, registeredDomainId, registrarType);

        foreach (var contact in registrarContacts)
        {
            contact.RegisteredDomainId = registeredDomainId;
            contact.RegistrarType = registrarType;

            var result = await SyncContactFromRegistrarAsync(contact);
            batchResult.Results.Add(result);

            if (result.Success)
            {
                batchResult.SuccessCount++;
                if (result.ContactPersonCreated) batchResult.ContactPersonsCreated++;
                if (result.AssignmentCreated) batchResult.AssignmentsCreated++;
                if (result.DomainContactCreated) batchResult.DomainContactsCreated++;
            }
            else
            {
                batchResult.FailureCount++;
            }
        }

        batchResult.Success = batchResult.FailureCount == 0;

        _log.Information("Batch sync completed for domain {DomainId}. " +
                       "Success: {SuccessCount}/{TotalContacts}, " +
                       "Created: ContactPersons={CPCreated}, Assignments={ACreated}, DomainContacts={DCCreated}",
            registeredDomainId, batchResult.SuccessCount, batchResult.TotalContacts,
            batchResult.ContactPersonsCreated, batchResult.AssignmentsCreated, batchResult.DomainContactsCreated);

        return batchResult;
    }

    /// <summary>
    /// Finds or creates a ContactPerson based on email address
    /// </summary>
    private async Task<ContactPerson> FindOrCreateContactPersonAsync(RegistrarContactSyncRequest request)
    {
        // Try to find existing ContactPerson by email
        var existingPerson = await _context.ContactPersons
            .FirstOrDefaultAsync(cp => cp.Email.ToLower() == request.Email.ToLower());

        if (existingPerson != null)
        {
            // Update existing if data differs
            var needsUpdate = false;

            if (request.UpdateExistingContactPerson)
            {
                if (existingPerson.FirstName != request.FirstName)
                {
                    existingPerson.FirstName = request.FirstName;
                    needsUpdate = true;
                }
                if (existingPerson.LastName != request.LastName)
                {
                    existingPerson.LastName = request.LastName;
                    needsUpdate = true;
                }
                if (!string.IsNullOrWhiteSpace(request.Phone) && existingPerson.Phone != request.Phone)
                {
                    existingPerson.Phone = request.Phone;
                    needsUpdate = true;
                }
                if (!string.IsNullOrWhiteSpace(request.Organization) && existingPerson.Department != request.Organization)
                {
                    existingPerson.Department = request.Organization;
                    needsUpdate = true;
                }

                if (needsUpdate)
                {
                    _log.Information("Updating existing ContactPerson {ContactPersonId} with data from registrar",
                        existingPerson.Id);
                }
            }

            return existingPerson;
        }

        // Create new ContactPerson
        var newPerson = new ContactPerson
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone ?? string.Empty,
            Department = request.Organization,
            CustomerId = request.CustomerId,
            IsActive = true,
            IsPrimary = false,
            Notes = $"Created from {request.RegistrarType} registrar sync on {DateTime.UtcNow:yyyy-MM-dd HH:mm}"
        };

        _context.ContactPersons.Add(newPerson);
        await _context.SaveChangesAsync(); // Save to get ID

        _log.Information("Created new ContactPerson with email {Email}", request.Email);

        return newPerson;
    }

    /// <summary>
    /// Finds or creates a DomainContactAssignment
    /// </summary>
    private async Task<DomainContactAssignment> FindOrCreateAssignmentAsync(
        int registeredDomainId, 
        int contactPersonId, 
        ContactRoleType roleType)
    {
        var existingAssignment = await _context.DomainContactAssignments
            .FirstOrDefaultAsync(a =>
                a.RegisteredDomainId == registeredDomainId &&
                a.ContactPersonId == contactPersonId &&
                a.RoleType == roleType);

        if (existingAssignment != null)
        {
            // Ensure it's active
            if (!existingAssignment.IsActive)
            {
                existingAssignment.IsActive = true;
                _log.Information("Reactivated assignment {AssignmentId}", existingAssignment.Id);
            }
            return existingAssignment;
        }

        // Check if there's an assignment for this domain/role with a different contact
        var existingRoleAssignment = await _context.DomainContactAssignments
            .FirstOrDefaultAsync(a =>
                a.RegisteredDomainId == registeredDomainId &&
                a.RoleType == roleType &&
                a.IsActive);

        if (existingRoleAssignment != null)
        {
            // Deactivate old assignment
            existingRoleAssignment.IsActive = false;
            _log.Information("Deactivated old assignment {AssignmentId} - contact changed for {RoleType}",
                existingRoleAssignment.Id, roleType);
        }

        // Create new assignment
        var newAssignment = new DomainContactAssignment
        {
            RegisteredDomainId = registeredDomainId,
            ContactPersonId = contactPersonId,
            RoleType = roleType,
            AssignedAt = DateTime.UtcNow,
            IsActive = true,
            Notes = "Created from registrar sync"
        };

        _context.DomainContactAssignments.Add(newAssignment);
        await _context.SaveChangesAsync(); // Save to get ID

        _log.Information("Created new DomainContactAssignment for domain {DomainId}, role {RoleType}",
            registeredDomainId, roleType);

        return newAssignment;
    }

    /// <summary>
    /// Creates or updates a DomainContact snapshot
    /// </summary>
    private async Task<DomainContact> CreateOrUpdateDomainContactAsync(
        RegistrarContactSyncRequest request,
        int sourceContactPersonId)
    {
        // Find existing current version
        var existingContact = await _context.DomainContacts
            .FirstOrDefaultAsync(dc =>
                dc.DomainId == request.RegisteredDomainId &&
                dc.RoleType == request.RoleType &&
                dc.IsCurrentVersion);

        if (existingContact != null)
        {
            // Check if data has changed
            bool hasChanged = HasContactDataChanged(existingContact, request);

            if (hasChanged && request.CreateHistoricalVersions)
            {
                // Mark old version as not current
                existingContact.IsCurrentVersion = false;
                _log.Information("Marked DomainContact {DomainContactId} as historical version", existingContact.Id);

                // Create new version (handled below)
            }
            else if (!hasChanged)
            {
                // Just update sync metadata
                existingContact.LastSyncedAt = DateTime.UtcNow;
                existingContact.NeedsSync = false;
                existingContact.RegistrarSnapshot = request.RawRegistrarData != null 
                    ? JsonSerializer.Serialize(request.RawRegistrarData) 
                    : existingContact.RegistrarSnapshot;
                
                return existingContact;
            }
            else
            {
                // Update in place (no versioning)
                UpdateDomainContactData(existingContact, request, sourceContactPersonId);
                return existingContact;
            }
        }

        // Create new DomainContact
        var newContact = new DomainContact
        {
            DomainId = request.RegisteredDomainId,
            RoleType = request.RoleType,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Organization = request.Organization,
            Email = request.Email,
            Phone = request.Phone ?? string.Empty,
            Fax = request.Fax,
            Address1 = request.Address1 ?? string.Empty,
            Address2 = request.Address2,
            City = request.City ?? string.Empty,
            State = request.State,
            PostalCode = request.PostalCode ?? string.Empty,
            CountryCode = request.CountryCode ?? string.Empty,
            IsActive = true,
            IsCurrentVersion = true,
            SourceContactPersonId = sourceContactPersonId,
            LastSyncedAt = DateTime.UtcNow,
            NeedsSync = false,
            RegistrarContactId = request.RegistrarContactId,
            RegistrarType = request.RegistrarType,
            IsPrivacyProtected = request.IsPrivacyProtected,
            RegistrarSnapshot = request.RawRegistrarData != null 
                ? JsonSerializer.Serialize(request.RawRegistrarData) 
                : null,
            Notes = $"Synced from {request.RegistrarType} on {DateTime.UtcNow:yyyy-MM-dd HH:mm}"
        };

        _context.DomainContacts.Add(newContact);

        _log.Information("Created new DomainContact for domain {DomainId}, role {RoleType}",
            request.RegisteredDomainId, request.RoleType);

        return newContact;
    }

    /// <summary>
    /// Checks if contact data has changed
    /// </summary>
    private bool HasContactDataChanged(DomainContact existing, RegistrarContactSyncRequest request)
    {
        return existing.FirstName != request.FirstName ||
               existing.LastName != request.LastName ||
               existing.Email != request.Email ||
               existing.Phone != (request.Phone ?? string.Empty) ||
               existing.Organization != request.Organization ||
               existing.Address1 != (request.Address1 ?? string.Empty) ||
               existing.City != (request.City ?? string.Empty) ||
               existing.PostalCode != (request.PostalCode ?? string.Empty) ||
               existing.CountryCode != (request.CountryCode ?? string.Empty) ||
               existing.IsPrivacyProtected != request.IsPrivacyProtected;
    }

    /// <summary>
    /// Updates DomainContact data in place
    /// </summary>
    private void UpdateDomainContactData(
        DomainContact contact, 
        RegistrarContactSyncRequest request, 
        int sourceContactPersonId)
    {
        contact.FirstName = request.FirstName;
        contact.LastName = request.LastName;
        contact.Organization = request.Organization;
        contact.Email = request.Email;
        contact.Phone = request.Phone ?? string.Empty;
        contact.Fax = request.Fax;
        contact.Address1 = request.Address1 ?? string.Empty;
        contact.Address2 = request.Address2;
        contact.City = request.City ?? string.Empty;
        contact.State = request.State;
        contact.PostalCode = request.PostalCode ?? string.Empty;
        contact.CountryCode = request.CountryCode ?? string.Empty;
        contact.SourceContactPersonId = sourceContactPersonId;
        contact.LastSyncedAt = DateTime.UtcNow;
        contact.NeedsSync = false;
        contact.RegistrarContactId = request.RegistrarContactId;
        contact.IsPrivacyProtected = request.IsPrivacyProtected;
        contact.RegistrarSnapshot = request.RawRegistrarData != null 
            ? JsonSerializer.Serialize(request.RawRegistrarData) 
            : contact.RegistrarSnapshot;

        _log.Information("Updated DomainContact {DomainContactId} with new data from registrar", contact.Id);
    }

    /// <summary>
    /// Detects drift between ContactPerson and DomainContact
    /// </summary>
    public async Task<List<ContactDriftReport>> DetectDriftForDomainAsync(int registeredDomainId)
    {
        var reports = new List<ContactDriftReport>();

        var domainContacts = await _context.DomainContacts
            .Include(dc => dc.SourceContactPerson)
            .Where(dc => dc.DomainId == registeredDomainId && 
                        dc.IsCurrentVersion && 
                        dc.SourceContactPersonId != null)
            .ToListAsync();

        foreach (var dc in domainContacts)
        {
            if (dc.SourceContactPerson == null) continue;

            var hasDrift = dc.FirstName != dc.SourceContactPerson.FirstName ||
                          dc.LastName != dc.SourceContactPerson.LastName ||
                          dc.Email != dc.SourceContactPerson.Email ||
                          dc.Phone != dc.SourceContactPerson.Phone;

            if (hasDrift)
            {
                reports.Add(new ContactDriftReport
                {
                    DomainId = registeredDomainId,
                    DomainContactId = dc.Id,
                    ContactPersonId = dc.SourceContactPersonId.Value,
                    RoleType = dc.RoleType,
                    MasterFirstName = dc.SourceContactPerson.FirstName,
                    RegistrarFirstName = dc.FirstName,
                    MasterLastName = dc.SourceContactPerson.LastName,
                    RegistrarLastName = dc.LastName,
                    MasterEmail = dc.SourceContactPerson.Email,
                    RegistrarEmail = dc.Email,
                    MasterPhone = dc.SourceContactPerson.Phone,
                    RegistrarPhone = dc.Phone,
                    LastSyncedAt = dc.LastSyncedAt
                });
            }
        }

        return reports;
    }

    /// <summary>
    /// Ensures a billing contact exists for a domain. If not, creates one from the Administrative contact.
    /// This is useful for registrars like AWS Route 53 that don't provide separate billing contacts.
    /// </summary>
    /// <param name="registeredDomainId">The domain ID</param>
    /// <returns>True if billing contact was created or already exists</returns>
    public async Task<bool> EnsureBillingContactAsync(int registeredDomainId)
    {
        try
        {
            // Check if billing contact already exists
            var hasBilling = await _context.DomainContactAssignments
                .AnyAsync(a => 
                    a.RegisteredDomainId == registeredDomainId && 
                    a.RoleType == ContactRoleType.Billing);

            if (hasBilling)
            {
                _log.Debug("Billing contact already exists for domain {DomainId}", registeredDomainId);
                return true;
            }

            // Find administrative contact to use as template
            var adminAssignment = await _context.DomainContactAssignments
                .FirstOrDefaultAsync(a => 
                    a.RegisteredDomainId == registeredDomainId && 
                    a.RoleType == ContactRoleType.Administrative);

            if (adminAssignment == null)
            {
                _log.Warning("Cannot create billing contact for domain {DomainId} - no Administrative contact found", 
                    registeredDomainId);
                return false;
            }

            // Create billing assignment pointing to same contact person
            var billingAssignment = new DomainContactAssignment
            {
                RegisteredDomainId = registeredDomainId,
                ContactPersonId = adminAssignment.ContactPersonId,
                RoleType = ContactRoleType.Billing,
                AssignedAt = DateTime.UtcNow,
                IsActive = true,
                Notes = "Auto-generated from Administrative contact (registrar doesn't provide separate billing)"
            };

            _context.DomainContactAssignments.Add(billingAssignment);

            // Also create DomainContact snapshot for billing
            var adminDomainContact = await _context.DomainContacts
                .FirstOrDefaultAsync(dc => 
                    dc.DomainId == registeredDomainId && 
                    dc.RoleType == ContactRoleType.Administrative &&
                    dc.IsCurrentVersion);

            if (adminDomainContact != null)
            {
                var billingDomainContact = new DomainContact
                {
                    DomainId = registeredDomainId,
                    RoleType = ContactRoleType.Billing,
                    FirstName = adminDomainContact.FirstName,
                    LastName = adminDomainContact.LastName,
                    Organization = adminDomainContact.Organization,
                    Email = adminDomainContact.Email,
                    Phone = adminDomainContact.Phone,
                    Fax = adminDomainContact.Fax,
                    Address1 = adminDomainContact.Address1,
                    Address2 = adminDomainContact.Address2,
                    City = adminDomainContact.City,
                    State = adminDomainContact.State,
                    PostalCode = adminDomainContact.PostalCode,
                    CountryCode = adminDomainContact.CountryCode,
                    IsActive = true,
                    IsCurrentVersion = true,
                    SourceContactPersonId = adminAssignment.ContactPersonId,
                    LastSyncedAt = DateTime.UtcNow,
                    NeedsSync = false,
                    RegistrarType = adminDomainContact.RegistrarType,
                    Notes = "Auto-generated from Administrative contact"
                };

                _context.DomainContacts.Add(billingDomainContact);
            }

            await _context.SaveChangesAsync();

            _log.Information("Auto-created Billing contact for domain {DomainId} from Administrative contact", 
                registeredDomainId);

            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error ensuring billing contact for domain {DomainId}", registeredDomainId);
            return false;
        }
    }
}

/// <summary>
/// Request object for syncing a contact from registrar
/// </summary>
public class RegistrarContactSyncRequest
{
    public int RegisteredDomainId { get; set; }
    public ContactRoleType RoleType { get; set; }
    
    // Contact data from registrar
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Organization { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? CountryCode { get; set; }
    
    // Registrar metadata
    public string RegistrarType { get; set; } = string.Empty; // e.g., "AWS_Route53", "GoDaddy"
    public string? RegistrarContactId { get; set; }
    public bool IsPrivacyProtected { get; set; }
    public object? RawRegistrarData { get; set; } // Store original response
    
    // Optional customer association
    public int? CustomerId { get; set; }
    
    // Behavior flags
    public bool UpdateExistingContactPerson { get; set; } = false; // If true, updates existing ContactPerson
    public bool CreateHistoricalVersions { get; set; } = false; // If true, creates new version instead of updating
}

/// <summary>
/// Result of syncing a single contact
/// </summary>
public class ContactSyncResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    
    public int DomainId { get; set; }
    public ContactRoleType RoleType { get; set; }
    
    public int ContactPersonId { get; set; }
    public bool ContactPersonCreated { get; set; }
    
    public int AssignmentId { get; set; }
    public bool AssignmentCreated { get; set; }
    
    public int DomainContactId { get; set; }
    public bool DomainContactCreated { get; set; }
}

/// <summary>
/// Result of syncing multiple contacts in a batch
/// </summary>
public class BatchContactSyncResult
{
    public bool Success { get; set; }
    public int DomainId { get; set; }
    public int TotalContacts { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    
    public int ContactPersonsCreated { get; set; }
    public int AssignmentsCreated { get; set; }
    public int DomainContactsCreated { get; set; }
    
    public List<ContactSyncResult> Results { get; set; } = new();
}

/// <summary>
/// Report showing drift between master and registrar data
/// </summary>
public class ContactDriftReport
{
    public int DomainId { get; set; }
    public int DomainContactId { get; set; }
    public int ContactPersonId { get; set; }
    public ContactRoleType RoleType { get; set; }
    
    public string MasterFirstName { get; set; } = string.Empty;
    public string RegistrarFirstName { get; set; } = string.Empty;
    
    public string MasterLastName { get; set; } = string.Empty;
    public string RegistrarLastName { get; set; } = string.Empty;
    
    public string MasterEmail { get; set; } = string.Empty;
    public string RegistrarEmail { get; set; } = string.Empty;
    
    public string MasterPhone { get; set; } = string.Empty;
    public string RegistrarPhone { get; set; } = string.Empty;
    
    public DateTime? LastSyncedAt { get; set; }
}
