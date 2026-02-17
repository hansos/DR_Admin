# Hybrid Domain Contact Management System

## Overview

This document describes the three-layer hybrid contact management system for domain registrations. The system provides a clean separation between normalized master contact data, domain-to-contact assignments, and registrar-specific contact snapshots.

## Architecture

The system consists of three main components:

### 1. ContactPerson (Master/Normalized Data)
**Location:** `Data/Entities/ContactPerson.cs`

This is your **single source of truth** for contact information. It stores normalized, structured contact data that is independent of any specific domain or registrar.

**Key Properties:**
- `FirstName`, `LastName` - Contact name
- `Email`, `Phone` - Contact information
- `Position`, `Department` - Organizational info
- `IsPrimary`, `IsActive` - Status flags
- `CustomerId` - Links to customer (optional)

**Navigation Properties:**
- `DomainContactAssignments` - Which domains use this contact
- `SourcedDomainContacts` - Domain contacts derived from this record

### 2. DomainContactAssignment (Bridge/Mapping Layer)
**Location:** `Data/Entities/DomainContactAssignment.cs`

This is the **relationship table** that links ContactPerson to RegisteredDomain with a specific role.

**Key Properties:**
- `RegisteredDomainId` - Which domain
- `ContactPersonId` - Which contact person
- `RoleType` - What role (Registrant, Administrative, Technical, Billing)
- `IsActive` - Whether this assignment is active
- `AssignedAt` - When assigned

**Purpose:**
- Supports many-to-many relationships (one contact can have different roles on different domains)
- Allows easy bulk updates (change a contact, update all their domains)
- Clean separation of concerns

### 3. DomainContact (Snapshot/Cache/Audit Trail)
**Location:** `Data/Entities/DomainContact.cs`

This is the **registrar-specific snapshot** of what was actually sent to or received from the domain registrar.

**Key Properties:**
- `RoleType` - Contact role (enum: Registrant, Administrative, Technical, Billing)
- `FirstName`, `LastName`, `Email`, `Phone` - Contact details
- `Address1`, `Address2`, `City`, `State`, `PostalCode`, `CountryCode` - Full address
- `Organization`, `Fax` - Additional registrar fields
- `SourceContactPersonId` - Links back to master ContactPerson (if managed)
- `LastSyncedAt` - When last synchronized with registrar
- `NeedsSync` - Flag indicating sync required
- `RegistrarContactId` - External ID from registrar
- `RegistrarType` - Which registrar (AWS Route 53, GoDaddy, etc.)
- `IsPrivacyProtected` - WHOIS privacy flag
- `RegistrarSnapshot` - JSON of raw registrar response
- `IsCurrentVersion` - For historical versioning (append-only pattern)

**Purpose:**
- Stores exactly what the registrar has on file
- Audit trail for compliance and debugging
- Handles registrar-specific quirks and requirements
- Supports historical tracking of changes

## ContactRoleType Enum
**Location:** `Data/Entities/ContactRoleType.cs`

```csharp
public enum ContactRoleType
{
    Registrant = 1,      // Domain owner
    Administrative = 2,  // Administrative contact
    Technical = 3,       // Technical contact
    Billing = 4          // Billing contact
}
```

## Workflow Patterns

### Pattern 1: New Domain Registration

```
1. User selects/creates ContactPerson records for each role
2. Create DomainContactAssignment entries linking domain to contacts
3. System copies ContactPerson → DomainContact (creates snapshots)
4. Send DomainContact data to registrar
5. Update DomainContact with registrar response + LastSyncedAt
```

**Implementation:**
```csharp
// Step 1-2: Assign contacts to domain
await assignmentService.AssignContactToDomainAsync(domainId, contactPersonId, "Registrant");
await assignmentService.AssignContactToDomainAsync(domainId, adminContactId, "Administrative");

// Step 3: Sync to DomainContact
await assignmentService.SyncContactPersonToDomainContactAsync(assignmentId);

// Step 4-5: Send to registrar (your existing code)
// Update LastSyncedAt and RegistrarSnapshot after successful sync
```

### Pattern 2: Update Contact → Propagate to Domains

```
1. User updates ContactPerson
2. Find all DomainContactAssignments for that person
3. Mark related DomainContact records as NeedsSync = true
4. Background job processes sync queue
5. Update registrars with new data
6. Update DomainContact with LastSyncedAt
```

**Implementation:**
```csharp
// After updating ContactPerson
var markedCount = await assignmentService.MarkContactsNeedingSyncAsync(contactPersonId);

// Background job queries:
var needsSync = await context.DomainContacts
    .Where(dc => dc.NeedsSync && dc.IsCurrentVersion)
    .ToListAsync();

// After successful registrar update:
domainContact.NeedsSync = false;
domainContact.LastSyncedAt = DateTime.UtcNow;
```

### Pattern 3: Sync FROM Registrar (Import/Refresh)

```
1. Fetch domain contacts from registrar
2. Create/update DomainContact records with raw data
3. Update RegistrarSnapshot with JSON response
4. If SourceContactPersonId exists and data differs:
   - Flag for review or auto-update ContactPerson
5. If no SourceContactPersonId:
   - Optionally create ContactPerson from registrar data
```

**Implementation:**
```csharp
// After fetching from registrar
var domainContact = new DomainContact
{
    DomainId = domainId,
    RoleType = ContactRoleType.Registrant,
    FirstName = registrarData.FirstName,
    // ... other fields
    LastSyncedAt = DateTime.UtcNow,
    RegistrarSnapshot = JsonSerializer.Serialize(registrarData),
    RegistrarContactId = registrarData.ExternalId,
    RegistrarType = "AWS_Route53"
};

// Check for drift
if (domainContact.SourceContactPersonId.HasValue)
{
    var master = await context.ContactPersons.FindAsync(domainContact.SourceContactPersonId);
    if (master.Email != domainContact.Email)
    {
        // Log drift, notify admin, or auto-update
    }
}
```

### Pattern 4: Detect Drift Between Master and Registrar

```csharp
public async Task<List<ContactDriftReport>> DetectDriftAsync()
{
    var driftReports = new List<ContactDriftReport>();
    
    var domainContacts = await _context.DomainContacts
        .Include(dc => dc.SourceContactPerson)
        .Where(dc => dc.SourceContactPersonId.HasValue && dc.IsCurrentVersion)
        .ToListAsync();
    
    foreach (var dc in domainContacts)
    {
        if (dc.SourceContactPerson == null) continue;
        
        if (dc.Email != dc.SourceContactPerson.Email ||
            dc.FirstName != dc.SourceContactPerson.FirstName ||
            dc.LastName != dc.SourceContactPerson.LastName)
        {
            driftReports.Add(new ContactDriftReport
            {
                DomainId = dc.DomainId,
                ContactRole = dc.RoleType,
                MasterEmail = dc.SourceContactPerson.Email,
                RegistrarEmail = dc.Email,
                LastSyncedAt = dc.LastSyncedAt
            });
        }
    }
    
    return driftReports;
}
```

## Service Layer

### DomainContactAssignmentService
**Location:** `Services/DomainContactAssignmentService.cs`

**Key Methods:**
- `GetAssignmentsByDomainAsync(domainId)` - Get all contacts for a domain
- `GetAssignmentsByContactPersonAsync(contactPersonId)` - Get all domains for a contact
- `CreateAssignmentAsync(dto)` - Link contact to domain with role
- `AssignContactToDomainAsync(domainId, contactId, role)` - Shortcut method
- `SyncContactPersonToDomainContactAsync(assignmentId)` - Push master data to snapshot
- `MarkContactsNeedingSyncAsync(contactPersonId)` - Flag all related contacts for sync

### Updated DomainContactService
**Location:** `Services/DomainContactService.cs`

Now handles `RoleType` enum instead of string `ContactType`:
- Parses string input to `ContactRoleType` enum
- Stores enum in entity
- Converts back to string in DTO

## Database Configuration

### ApplicationDbContext Changes
**Location:** `Data/ApplicationDbContext.cs`

Added:
1. `DbSet<DomainContactAssignment>`
2. Entity configurations with proper indexes and relationships
3. Normalization for DomainContact fields

**Key Indexes:**
- `DomainContact`: (DomainId, RoleType, IsCurrentVersion)
- `DomainContactAssignment`: (RegisteredDomainId, RoleType, IsActive)
- `DomainContact`: SourceContactPersonId, NeedsSync

## DTOs

### DomainContactAssignmentDto
**Location:** `DTOs/DomainContactAssignmentDto.cs`

- `DomainContactAssignmentDto` - Full assignment details
- `CreateDomainContactAssignmentDto` - For creating assignments
- `UpdateDomainContactAssignmentDto` - For updating assignments

### Updated DomainContactDto
**Location:** `DTOs/DomainContactDto.cs`

Added fields:
- `SourceContactPersonId`
- `LastSyncedAt`
- `NeedsSync`
- `RegistrarContactId`
- `RegistrarType`
- `IsPrivacyProtected`
- `IsCurrentVersion`

## Migration Guide

### If you have existing DomainContacts:

```sql
-- Step 1: Extract unique contacts from DomainContacts
INSERT INTO ContactPersons (FirstName, LastName, Email, Phone, IsActive, CreatedAt, UpdatedAt)
SELECT DISTINCT FirstName, LastName, Email, Phone, IsActive, CreatedAt, UpdatedAt
FROM DomainContacts
WHERE Email NOT IN (SELECT Email FROM ContactPersons);

-- Step 2: Create assignments
INSERT INTO DomainContactAssignments (RegisteredDomainId, ContactPersonId, RoleType, AssignedAt, IsActive, CreatedAt, UpdatedAt)
SELECT dc.DomainId, cp.Id, dc.RoleType, dc.CreatedAt, dc.IsActive, dc.CreatedAt, dc.UpdatedAt
FROM DomainContacts dc
INNER JOIN ContactPersons cp ON dc.Email = cp.Email;

-- Step 3: Update DomainContacts with SourceContactPersonId
UPDATE DomainContacts
SET SourceContactPersonId = cp.Id
FROM ContactPersons cp
WHERE DomainContacts.Email = cp.Email;

-- Step 4: Mark as current versions
UPDATE DomainContacts SET IsCurrentVersion = 1;
```

## Best Practices

### 1. Privacy Protection Handling
```csharp
if (domain.PrivacyProtection)
{
    // DomainContact shows privacy service
    domainContact.Organization = "Privacy Protection Service";
    domainContact.IsPrivacyProtected = true;
    // SourceContactPerson has real data
}
```

### 2. Historical Versioning (Optional Append-Only Pattern)
```csharp
// Instead of updating, create new version
var oldVersion = await context.DomainContacts
    .FirstAsync(dc => dc.DomainId == domainId && dc.RoleType == role && dc.IsCurrentVersion);
    
oldVersion.IsCurrentVersion = false;

var newVersion = new DomainContact
{
    // Copy fields from oldVersion
    // Update changed fields
    IsCurrentVersion = true
};
context.DomainContacts.Add(newVersion);
```

### 3. Conflict Resolution
Define strategy in configuration:
- **Master Wins**: ContactPerson always propagates (default)
- **Registrar Wins**: Sync updates ContactPerson
- **Manual Review**: Admin resolves conflicts

### 4. Registrar-Specific Validation
```csharp
if (registrarType == "AWS_Route53")
{
    // AWS has specific country code requirements
    ValidateRoute53ContactData(domainContact);
}
```

## Advantages of This Approach

1. **Single Source of Truth**: ContactPerson is authoritative
2. **Audit Trail**: Complete history of what registrars have
3. **Flexibility**: Handles registrar quirks without polluting master data
4. **Scalability**: Easy to add new registrars
5. **Compliance**: Full tracking for legal requirements
6. **Drift Detection**: Identify when registrar data diverges
7. **Bulk Updates**: Change contact once, update all domains
8. **Privacy Support**: Separate real data from privacy-protected data
9. **Historical Tracking**: Optional versioning for complete audit trail

## Future Enhancements

1. **Automated Sync Queue**: Background service processing NeedsSync flags
2. **Conflict Resolution UI**: Admin dashboard for drift management
3. **Registrar-Specific Adapters**: Plugin architecture for new registrars
4. **Contact Validation**: Pre-sync validation rules per TLD/registrar
5. **Bulk Assignment**: Assign same contact to multiple domains at once
6. **Contact Templates**: Pre-defined contact sets for common scenarios
