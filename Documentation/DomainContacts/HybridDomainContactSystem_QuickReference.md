# Hybrid Domain Contact System - Quick Reference

## Common Operations

### 1. Assign Contact to Domain

```csharp
// Using the service
var assignment = await domainContactAssignmentService.AssignContactToDomainAsync(
    registeredDomainId: 123,
    contactPersonId: 456,
    roleType: "Registrant" // or "Administrative", "Technical", "Billing"
);

// Sync to DomainContact
await domainContactAssignmentService.SyncContactPersonToDomainContactAsync(assignment.Id);
```

### 2. Get All Contacts for a Domain

```csharp
var assignments = await domainContactAssignmentService.GetAssignmentsByDomainAsync(domainId);

foreach (var assignment in assignments)
{
    Console.WriteLine($"{assignment.RoleType}: {assignment.ContactPerson.FirstName} {assignment.ContactPerson.LastName}");
}
```

### 3. Update Contact and Propagate Changes

```csharp
// Update the master ContactPerson
var contactPerson = await context.ContactPersons.FindAsync(contactPersonId);
contactPerson.Email = "newemail@example.com";
await context.SaveChangesAsync();

// Mark all related domain contacts for sync
var count = await domainContactAssignmentService.MarkContactsNeedingSyncAsync(contactPersonId);
Console.WriteLine($"Marked {count} domain contacts for sync");

// Process sync (in background job or manually)
var needsSync = await context.DomainContacts
    .Where(dc => dc.NeedsSync && dc.IsCurrentVersion)
    .ToListAsync();

foreach (var dc in needsSync)
{
    // Send to registrar
    await registrarClient.UpdateContactAsync(dc);
    
    // Mark as synced
    dc.NeedsSync = false;
    dc.LastSyncedAt = DateTime.UtcNow;
}
await context.SaveChangesAsync();
```

### 4. Import Domain Contacts from Registrar

```csharp
// Fetch from registrar
var registrarContacts = await registrarClient.GetDomainContactsAsync(domainName);

foreach (var rc in registrarContacts)
{
    // Parse role type
    var roleType = rc.Type switch
    {
        "REGISTRANT" => ContactRoleType.Registrant,
        "ADMIN" => ContactRoleType.Administrative,
        "TECH" => ContactRoleType.Technical,
        "BILLING" => ContactRoleType.Billing,
        _ => ContactRoleType.Administrative
    };
    
    // Create or update DomainContact
    var existing = await context.DomainContacts
        .FirstOrDefaultAsync(dc => 
            dc.DomainId == domainId && 
            dc.RoleType == roleType &&
            dc.IsCurrentVersion);
    
    if (existing != null)
    {
        // Update existing
        existing.FirstName = rc.FirstName;
        existing.LastName = rc.LastName;
        existing.Email = rc.Email;
        // ... other fields
        existing.LastSyncedAt = DateTime.UtcNow;
        existing.RegistrarSnapshot = JsonSerializer.Serialize(rc);
    }
    else
    {
        // Create new
        var domainContact = new DomainContact
        {
            DomainId = domainId,
            RoleType = roleType,
            FirstName = rc.FirstName,
            LastName = rc.LastName,
            Email = rc.Email,
            Phone = rc.Phone,
            Organization = rc.Organization,
            Address1 = rc.Address1 ?? string.Empty,
            Address2 = rc.Address2,
            City = rc.City ?? string.Empty,
            State = rc.State,
            PostalCode = rc.PostalCode ?? string.Empty,
            CountryCode = rc.CountryCode ?? string.Empty,
            IsActive = true,
            IsCurrentVersion = true,
            LastSyncedAt = DateTime.UtcNow,
            RegistrarContactId = rc.ExternalId,
            RegistrarType = "AWS_Route53",
            RegistrarSnapshot = JsonSerializer.Serialize(rc)
        };
        
        // Try to link to existing ContactPerson by email
        var master = await context.ContactPersons
            .FirstOrDefaultAsync(cp => cp.Email.ToLower() == rc.Email.ToLower());
        
        if (master != null)
        {
            domainContact.SourceContactPersonId = master.Id;
        }
        
        context.DomainContacts.Add(domainContact);
    }
}

await context.SaveChangesAsync();
```

### 5. Detect Drift Between Master and Registrar

```csharp
var drifted = await context.DomainContacts
    .Include(dc => dc.SourceContactPerson)
    .Include(dc => dc.Domain)
    .Where(dc => dc.SourceContactPersonId != null && dc.IsCurrentVersion)
    .AsEnumerable()
    .Where(dc => 
        dc.Email != dc.SourceContactPerson.Email ||
        dc.FirstName != dc.SourceContactPerson.FirstName ||
        dc.LastName != dc.SourceContactPerson.LastName ||
        dc.Phone != dc.SourceContactPerson.Phone)
    .Select(dc => new
    {
        Domain = dc.Domain.Name,
        Role = dc.RoleType,
        MasterEmail = dc.SourceContactPerson.Email,
        RegistrarEmail = dc.Email,
        LastSynced = dc.LastSyncedAt
    })
    .ToList();

foreach (var drift in drifted)
{
    Console.WriteLine($"DRIFT: {drift.Domain} ({drift.Role})");
    Console.WriteLine($"  Master:    {drift.MasterEmail}");
    Console.WriteLine($"  Registrar: {drift.RegistrarEmail}");
    Console.WriteLine($"  Last Sync: {drift.LastSynced}");
}
```

### 6. Get Domain Contact Snapshot for Registrar Sync

```csharp
// Get current version of all contacts for a domain
var domainContacts = await context.DomainContacts
    .Where(dc => dc.DomainId == domainId && dc.IsCurrentVersion)
    .ToListAsync();

// Convert to registrar format
var registrantContact = domainContacts.FirstOrDefault(dc => dc.RoleType == ContactRoleType.Registrant);
var adminContact = domainContacts.FirstOrDefault(dc => dc.RoleType == ContactRoleType.Administrative);

var registrarRequest = new DomainRegistrationRequest
{
    DomainName = domain.Name,
    RegistrantContact = MapToRegistrarContact(registrantContact),
    AdminContact = MapToRegistrarContact(adminContact),
    // ...
};
```

### 7. Migrate Existing Data

```csharp
var migrationHelper = new DomainContactMigrationHelper(context);

// Check if migration is needed
if (await migrationHelper.IsMigrationNeededAsync())
{
    // Get preview
    var preview = await migrationHelper.GetMigrationPreviewAsync();
    Console.WriteLine($"Migration Preview:");
    Console.WriteLine($"  Existing ContactPersons: {preview.ExistingContactPersons}");
    Console.WriteLine($"  New ContactPersons to create: {preview.UniqueContactsToMigrate}");
    Console.WriteLine($"  Assignments to create: {preview.AssignmentsToCreate}");
    Console.WriteLine($"  DomainContacts to link: {preview.UnlinkedDomainContacts}");
    
    // Perform migration
    var result = await migrationHelper.MigrateToHybridSystemAsync();
    
    if (result.Success)
    {
        Console.WriteLine($"✓ Migration successful!");
        Console.WriteLine($"  ContactPersons created: {result.ContactPersonsCreated}");
        Console.WriteLine($"  Assignments created: {result.AssignmentsCreated}");
        Console.WriteLine($"  DomainContacts linked: {result.DomainContactsLinked}");
        Console.WriteLine($"  DomainContacts updated: {result.DomainContactsUpdated}");
    }
    else
    {
        Console.WriteLine($"✗ Migration failed: {result.ErrorMessage}");
    }
}
else
{
    Console.WriteLine("No migration needed - system is up to date");
}
```

### 8. Create Historical Version (Append-Only Pattern)

```csharp
// When updating a contact, create new version instead of updating
var currentVersion = await context.DomainContacts
    .FirstOrDefaultAsync(dc => 
        dc.DomainId == domainId && 
        dc.RoleType == ContactRoleType.Registrant &&
        dc.IsCurrentVersion);

if (currentVersion != null)
{
    // Mark old version as not current
    currentVersion.IsCurrentVersion = false;
    
    // Create new version with changes
    var newVersion = new DomainContact
    {
        DomainId = currentVersion.DomainId,
        RoleType = currentVersion.RoleType,
        FirstName = updatedData.FirstName, // Updated field
        LastName = updatedData.LastName,   // Updated field
        Email = currentVersion.Email,
        Phone = currentVersion.Phone,
        // ... copy other fields
        SourceContactPersonId = currentVersion.SourceContactPersonId,
        IsCurrentVersion = true,
        LastSyncedAt = DateTime.UtcNow,
        RegistrarSnapshot = JsonSerializer.Serialize(updatedData)
    };
    
    context.DomainContacts.Add(newVersion);
    await context.SaveChangesAsync();
}
```

### 9. Query Historical Contact Changes

```csharp
// Get all versions of a contact for a domain/role
var history = await context.DomainContacts
    .Where(dc => dc.DomainId == domainId && dc.RoleType == ContactRoleType.Registrant)
    .OrderByDescending(dc => dc.CreatedAt)
    .Select(dc => new
    {
        dc.FirstName,
        dc.LastName,
        dc.Email,
        dc.CreatedAt,
        dc.LastSyncedAt,
        dc.IsCurrentVersion
    })
    .ToListAsync();

Console.WriteLine($"Contact History for Domain {domainId} (Registrant):");
foreach (var version in history)
{
    var current = version.IsCurrentVersion ? " [CURRENT]" : "";
    Console.WriteLine($"{version.CreatedAt:yyyy-MM-dd HH:mm}: {version.FirstName} {version.LastName} <{version.Email}>{current}");
}
```

### 10. Find All Domains Using a Contact

```csharp
var assignments = await domainContactAssignmentService
    .GetAssignmentsByContactPersonAsync(contactPersonId);

Console.WriteLine($"Domains using contact {contactPersonId}:");
foreach (var assignment in assignments)
{
    var domain = await context.RegisteredDomains.FindAsync(assignment.RegisteredDomainId);
    Console.WriteLine($"  {domain.Name} - {assignment.RoleType} (Active: {assignment.IsActive})");
}
```

## Useful Queries

### Find Contacts Needing Sync
```csharp
var needsSync = await context.DomainContacts
    .Include(dc => dc.Domain)
    .Where(dc => dc.NeedsSync && dc.IsCurrentVersion)
    .Select(dc => new { dc.Domain.Name, dc.RoleType, dc.LastSyncedAt })
    .ToListAsync();
```

### Find Orphaned DomainContacts (No Source)
```csharp
var orphaned = await context.DomainContacts
    .Include(dc => dc.Domain)
    .Where(dc => dc.SourceContactPersonId == null && dc.IsCurrentVersion)
    .ToListAsync();
```

### Find Domains with Incomplete Contacts
```csharp
var domains = await context.RegisteredDomains
    .Include(d => d.DomainContacts.Where(dc => dc.IsCurrentVersion))
    .Where(d => d.DomainContacts.Count < 4) // Should have 4 role types
    .ToListAsync();
```

### Find Most Recent Sync Time
```csharp
var oldestSync = await context.DomainContacts
    .Where(dc => dc.IsCurrentVersion && dc.LastSyncedAt != null)
    .OrderBy(dc => dc.LastSyncedAt)
    .Select(dc => new { dc.Domain.Name, dc.LastSyncedAt })
    .FirstOrDefaultAsync();
```

## Error Handling

### Invalid Role Type
```csharp
try
{
    await domainContactAssignmentService.CreateAssignmentAsync(createDto);
}
catch (ArgumentException ex) when (ex.ParamName == "createDto")
{
    Console.WriteLine($"Invalid role type: {ex.Message}");
}
```

### Sync Failure
```csharp
try
{
    await SyncToRegistrarAsync(domainContact);
    domainContact.NeedsSync = false;
    domainContact.LastSyncedAt = DateTime.UtcNow;
}
catch (Exception ex)
{
    _log.Error(ex, "Failed to sync contact {ContactId} to registrar", domainContact.Id);
    // Keep NeedsSync = true for retry
}
finally
{
    await context.SaveChangesAsync();
}
```

## Performance Tips

1. **Use AsNoTracking for read-only queries**
```csharp
var contacts = await context.DomainContacts
    .AsNoTracking()
    .Where(dc => dc.IsCurrentVersion)
    .ToListAsync();
```

2. **Include related entities when needed**
```csharp
var assignments = await context.DomainContactAssignments
    .Include(a => a.ContactPerson)
    .Include(a => a.RegisteredDomain)
    .Where(a => a.IsActive)
    .ToListAsync();
```

3. **Use projection for large datasets**
```csharp
var contacts = await context.DomainContacts
    .Where(dc => dc.IsCurrentVersion)
    .Select(dc => new { dc.Id, dc.Email, dc.LastSyncedAt })
    .ToListAsync();
```

4. **Batch updates**
```csharp
var contacts = await context.DomainContacts
    .Where(dc => dc.SourceContactPersonId == contactPersonId)
    .ToListAsync();

foreach (var contact in contacts)
{
    contact.NeedsSync = true;
}

await context.SaveChangesAsync(); // Single save
```
