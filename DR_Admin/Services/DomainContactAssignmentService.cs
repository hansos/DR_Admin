using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing domain contact assignments
/// </summary>
public class DomainContactAssignmentService : IDomainContactAssignmentService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<DomainContactAssignmentService>();

    public DomainContactAssignmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all contact assignments for a specific domain
    /// </summary>
    public async Task<IEnumerable<DomainContactAssignmentDto>> GetAssignmentsByDomainAsync(int registeredDomainId)
    {
        try
        {
            _log.Information("Fetching contact assignments for domain ID: {DomainId}", registeredDomainId);

            var assignments = await _context.DomainContactAssignments
                .AsNoTracking()
                .Include(a => a.ContactPerson)
                .Where(a => a.RegisteredDomainId == registeredDomainId)
                .ToListAsync();

            var assignmentDtos = assignments.Select(MapToDto);

            _log.Information("Successfully fetched {Count} assignments for domain ID: {DomainId}", 
                assignments.Count, registeredDomainId);
            return assignmentDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching assignments for domain ID: {DomainId}", registeredDomainId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all domains assigned to a specific contact person
    /// </summary>
    public async Task<IEnumerable<DomainContactAssignmentDto>> GetAssignmentsByContactPersonAsync(int contactPersonId)
    {
        try
        {
            _log.Information("Fetching domain assignments for contact person ID: {ContactPersonId}", contactPersonId);

            var assignments = await _context.DomainContactAssignments
                .AsNoTracking()
                .Include(a => a.ContactPerson)
                .Where(a => a.ContactPersonId == contactPersonId)
                .ToListAsync();

            var assignmentDtos = assignments.Select(MapToDto);

            _log.Information("Successfully fetched {Count} assignments for contact person ID: {ContactPersonId}", 
                assignments.Count, contactPersonId);
            return assignmentDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching assignments for contact person ID: {ContactPersonId}", contactPersonId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific assignment by ID
    /// </summary>
    public async Task<DomainContactAssignmentDto?> GetAssignmentByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching assignment with ID: {AssignmentId}", id);

            var assignment = await _context.DomainContactAssignments
                .AsNoTracking()
                .Include(a => a.ContactPerson)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null)
            {
                _log.Warning("Assignment with ID {AssignmentId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched assignment with ID: {AssignmentId}", id);
            return MapToDto(assignment);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching assignment with ID: {AssignmentId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new domain contact assignment
    /// </summary>
    public async Task<DomainContactAssignmentDto> CreateAssignmentAsync(CreateDomainContactAssignmentDto createDto)
    {
        try
        {
            _log.Information("Creating new assignment for domain ID: {DomainId} and contact person ID: {ContactPersonId}", 
                createDto.RegisteredDomainId, createDto.ContactPersonId);

            if (!Enum.TryParse<ContactRoleType>(createDto.RoleType, true, out var roleType))
            {
                throw new ArgumentException($"Invalid role type: {createDto.RoleType}", nameof(createDto));
            }

            var assignment = new DomainContactAssignment
            {
                RegisteredDomainId = createDto.RegisteredDomainId,
                ContactPersonId = createDto.ContactPersonId,
                RoleType = roleType,
                AssignedAt = DateTime.UtcNow,
                IsActive = createDto.IsActive,
                Notes = createDto.Notes
            };

            _context.DomainContactAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created assignment with ID: {AssignmentId}", assignment.Id);
            
            // Reload with contact person
            assignment = await _context.DomainContactAssignments
                .Include(a => a.ContactPerson)
                .FirstAsync(a => a.Id == assignment.Id);
            
            return MapToDto(assignment);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating assignment");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing domain contact assignment
    /// </summary>
    public async Task<DomainContactAssignmentDto?> UpdateAssignmentAsync(int id, UpdateDomainContactAssignmentDto updateDto)
    {
        try
        {
            _log.Information("Updating assignment with ID: {AssignmentId}", id);

            var assignment = await _context.DomainContactAssignments
                .Include(a => a.ContactPerson)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null)
            {
                _log.Warning("Assignment with ID {AssignmentId} not found for update", id);
                return null;
            }

            if (!Enum.TryParse<ContactRoleType>(updateDto.RoleType, true, out var roleType))
            {
                throw new ArgumentException($"Invalid role type: {updateDto.RoleType}", nameof(updateDto));
            }

            assignment.ContactPersonId = updateDto.ContactPersonId;
            assignment.RoleType = roleType;
            assignment.IsActive = updateDto.IsActive;
            assignment.Notes = updateDto.Notes;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated assignment with ID: {AssignmentId}", id);
            return MapToDto(assignment);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating assignment with ID: {AssignmentId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a domain contact assignment
    /// </summary>
    public async Task<bool> DeleteAssignmentAsync(int id)
    {
        try
        {
            _log.Information("Deleting assignment with ID: {AssignmentId}", id);

            var assignment = await _context.DomainContactAssignments.FindAsync(id);

            if (assignment == null)
            {
                _log.Warning("Assignment with ID {AssignmentId} not found for deletion", id);
                return false;
            }

            _context.DomainContactAssignments.Remove(assignment);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted assignment with ID: {AssignmentId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting assignment with ID: {AssignmentId}", id);
            throw;
        }
    }

    /// <summary>
    /// Assigns a contact person to a domain with a specific role
    /// </summary>
    public async Task<DomainContactAssignmentDto> AssignContactToDomainAsync(int registeredDomainId, int contactPersonId, string roleType)
    {
        var createDto = new CreateDomainContactAssignmentDto
        {
            RegisteredDomainId = registeredDomainId,
            ContactPersonId = contactPersonId,
            RoleType = roleType,
            IsActive = true
        };

        return await CreateAssignmentAsync(createDto);
    }

    /// <summary>
    /// Synchronizes contact person data to domain contacts for a specific assignment
    /// </summary>
    public async Task<bool> SyncContactPersonToDomainContactAsync(int assignmentId)
    {
        try
        {
            _log.Information("Syncing contact person to domain contact for assignment ID: {AssignmentId}", assignmentId);

            var assignment = await _context.DomainContactAssignments
                .Include(a => a.ContactPerson)
                .Include(a => a.RegisteredDomain)
                .FirstOrDefaultAsync(a => a.Id == assignmentId);

            if (assignment == null)
            {
                _log.Warning("Assignment with ID {AssignmentId} not found", assignmentId);
                return false;
            }

            if (assignment.ContactPerson == null)
            {
                _log.Warning("Contact person not found for assignment ID {AssignmentId}", assignmentId);
                return false;
            }

            // Find or create the corresponding DomainContact
            var domainContact = await _context.DomainContacts
                .FirstOrDefaultAsync(dc => 
                    dc.DomainId == assignment.RegisteredDomainId && 
                    dc.RoleType == assignment.RoleType &&
                    dc.IsCurrentVersion);

            if (domainContact == null)
            {
                // Create new domain contact
                domainContact = new DomainContact
                {
                    DomainId = assignment.RegisteredDomainId,
                    RoleType = assignment.RoleType,
                    SourceContactPersonId = assignment.ContactPersonId,
                    IsCurrentVersion = true,
                    NeedsSync = true
                };
                _context.DomainContacts.Add(domainContact);
            }

            // Copy data from ContactPerson to DomainContact
            domainContact.FirstName = assignment.ContactPerson.FirstName;
            domainContact.LastName = assignment.ContactPerson.LastName;
            domainContact.Email = assignment.ContactPerson.Email;
            domainContact.Phone = assignment.ContactPerson.Phone;
            domainContact.Organization = assignment.ContactPerson.Department;
            
            // Set empty/default values for required address fields
            domainContact.Address1 = string.Empty;
            domainContact.City = string.Empty;
            domainContact.PostalCode = string.Empty;
            domainContact.CountryCode = string.Empty;
            
            domainContact.IsActive = assignment.ContactPerson.IsActive;
            domainContact.Notes = $"Synced from ContactPerson ID {assignment.ContactPersonId}";
            domainContact.NeedsSync = true;
            domainContact.SourceContactPersonId = assignment.ContactPersonId;

            await _context.SaveChangesAsync();

            _log.Information("Successfully synced contact person to domain contact for assignment ID: {AssignmentId}", assignmentId);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while syncing contact person to domain contact for assignment ID: {AssignmentId}", assignmentId);
            throw;
        }
    }

    /// <summary>
    /// Marks all domain contacts linked to a contact person as needing sync
    /// </summary>
    public async Task<int> MarkContactsNeedingSyncAsync(int contactPersonId)
    {
        try
        {
            _log.Information("Marking domain contacts as needing sync for contact person ID: {ContactPersonId}", contactPersonId);

            var domainContacts = await _context.DomainContacts
                .Where(dc => dc.SourceContactPersonId == contactPersonId)
                .ToListAsync();

            foreach (var contact in domainContacts)
            {
                contact.NeedsSync = true;
            }

            await _context.SaveChangesAsync();

            _log.Information("Successfully marked {Count} domain contacts as needing sync for contact person ID: {ContactPersonId}", 
                domainContacts.Count, contactPersonId);
            
            return domainContacts.Count;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while marking domain contacts for sync for contact person ID: {ContactPersonId}", contactPersonId);
            throw;
        }
    }

    /// <summary>
    /// Maps a DomainContactAssignment entity to a DTO
    /// </summary>
    private static DomainContactAssignmentDto MapToDto(DomainContactAssignment assignment)
    {
        var dto = new DomainContactAssignmentDto
        {
            Id = assignment.Id,
            RegisteredDomainId = assignment.RegisteredDomainId,
            ContactPersonId = assignment.ContactPersonId,
            RoleType = assignment.RoleType.ToString(),
            AssignedAt = assignment.AssignedAt,
            IsActive = assignment.IsActive,
            Notes = assignment.Notes,
            CreatedAt = assignment.CreatedAt,
            UpdatedAt = assignment.UpdatedAt
        };

        // Include contact person details if loaded
        if (assignment.ContactPerson != null)
        {
            dto.ContactPerson = new ContactPersonDto
            {
                Id = assignment.ContactPerson.Id,
                FirstName = assignment.ContactPerson.FirstName,
                LastName = assignment.ContactPerson.LastName,
                Email = assignment.ContactPerson.Email,
                Phone = assignment.ContactPerson.Phone,
                Position = assignment.ContactPerson.Position,
                Department = assignment.ContactPerson.Department,
                IsPrimary = assignment.ContactPerson.IsPrimary,
                IsActive = assignment.ContactPerson.IsActive,
                Notes = assignment.ContactPerson.Notes,
                CustomerId = assignment.ContactPerson.CustomerId,
                CreatedAt = assignment.ContactPerson.CreatedAt,
                UpdatedAt = assignment.ContactPerson.UpdatedAt
            };
        }

        return dto;
    }
}
