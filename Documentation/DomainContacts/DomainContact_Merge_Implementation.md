# Domain Contact Merge Implementation - Summary

## Overview
Updated the `RegistrarService.MergeRegisteredDomainsToDatabase` method to automatically merge contact persons from registrars into the `DomainContacts` table.

## Changes Made

### 1. Enhanced `MergeRegisteredDomainsToDatabase` Method
**File:** `DR_Admin/Services/RegistrarService.cs`

#### What Changed:
- Added contact merging capability
- Tracks created and updated contacts
- Calls new helper method `MergeDomainContactsAsync` for each domain with contacts
- Enhanced logging to include contact statistics

#### New Counters:
```csharp
int contactsCreated = 0;
int contactsUpdated = 0;
```

#### Contact Processing:
```csharp
// Merge contact information if available
if (domainInfo.Contacts != null && domainInfo.Contacts.Any())
{
    var contactStats = await MergeDomainContactsAsync(domain.Id, domainInfo.Contacts);
    contactsCreated += contactStats.Created;
    contactsUpdated += contactStats.Updated;
}
```

### 2. New Helper Method: `MergeDomainContactsAsync`
**File:** `DR_Admin/Services/RegistrarService.cs`

#### Purpose:
Merges domain contact information from registrar into the DomainContacts table

#### Parameters:
- `domainId` - The domain ID to associate contacts with
- `contacts` - List of `DomainContactInfo` from the registrar

#### Returns:
- Tuple `(int Created, int Updated)` with statistics

#### Logic Flow:

1. **Validation**
   - Checks for required fields: Email, FirstName, LastName
   - Skips contacts missing required data with warning log

2. **Find or Create**
   - Searches for existing contact by: DomainId + ContactType + Email
   - Updates if found, creates new if not found

3. **Update Strategy**
   - Required fields always updated
   - Optional fields updated only if provided (null-coalescing)
   - Sets `UpdatedAt` timestamp

4. **Create Strategy**
   - Creates new `DomainContact` entity
   - Sets default empty strings for required fields if null
   - Sets timestamps (CreatedAt, UpdatedAt)

### 3. Updated `GetRegisteredDomainsAsync` Method
**File:** `DR_Admin/Services/RegistrarService.cs`

#### Changes:
- Restored `save` parameter (was missing, causing build error)
- Now conditionally calls `MergeRegisteredDomainsToDatabase` when `save=true`
- Enhanced logging to include save parameter

#### New Logic:
```csharp
// If save is true, merge to database
if (save && result.Success && result.Domains != null && result.Domains.Any())
{
    await MergeRegisteredDomainsToDatabase(registrarId, result.Domains);
}
```

### 4. Simplified `DownloadDomainsForRegistrarAsync` Method
**File:** `DR_Admin/Services/RegistrarService.cs`

#### Changes:
- Removed duplicate call to `MergeRegisteredDomainsToDatabase`
- Now relies on `GetRegisteredDomainsAsync` to handle saving
- Cleaner, DRY (Don't Repeat Yourself) implementation

## Data Flow

```
Registrar API
    ?
GetRegisteredDomainsAsync(registrarId, save=true)
    ?
RegisteredDomainsResult (with Contacts)
    ?
MergeRegisteredDomainsToDatabase()
    ??? Update Domain Info
    ??? Create/Update TLDs
    ??? Create/Update RegistrarTlds
    ??? MergeDomainContactsAsync()
        ??? Validate required fields
        ??? Find existing by (DomainId + ContactType + Email)
        ??? Update if found
        ??? Create if not found
    ?
DomainContacts Table
```

## Contact Merge Strategy

### Matching Logic
Contacts are matched by these criteria:
1. `DomainId` - Must match
2. `ContactType` - Must match (Registrant, Admin, Technical, Billing)
3. `Email` - Must match

If all three match ? **UPDATE**  
If no match found ? **CREATE**

### Field Update Rules

| Field | Create | Update | Notes |
|-------|--------|--------|-------|
| FirstName | Required | Always updated | From registrar |
| LastName | Required | Always updated | From registrar |
| Email | Required | Always updated | From registrar |
| Organization | Optional | Always updated | From registrar |
| Phone | Default "" | Update if provided | Null-coalescing |
| Fax | Optional | Always updated | From registrar |
| Address1 | Default "" | Update if provided | Null-coalescing |
| Address2 | Optional | Always updated | From registrar |
| City | Default "" | Update if provided | Null-coalescing |
| State | Optional | Always updated | From registrar |
| PostalCode | Default "" | Update if provided | Null-coalescing |
| CountryCode | Default "" | Update if provided | Null-coalescing |
| IsActive | From registrar | Always updated | From registrar |
| Notes | Optional | Always updated | From registrar |

## Validation

### Required Fields
The following fields are validated before creating/updating:
- **Email** - Must not be null or whitespace
- **FirstName** - Must not be null or whitespace
- **LastName** - Must not be null or whitespace

Contacts missing any required field are **skipped** with a warning log.

## Logging

### New Log Messages

**Debug Level:**
- "Merging {Count} contacts for domain ID {DomainId}"
- "Updating existing {ContactType} contact for domain {DomainId}"
- "Creating new {ContactType} contact for domain {DomainId}"

**Warning Level:**
- "Skipping contact for domain {DomainId} due to missing required fields (Email, FirstName, or LastName)"

**Information Level:**
- "Database merge completed: {DomainsUpdated} domains updated, {TldsCreated} TLDs created, {RegistrarTldsCreated} RegistrarTlds created, {ContactsCreated} contacts created, {ContactsUpdated} contacts updated"

## Usage Examples

### Example 1: Download and Save Domains with Contacts
```csharp
// Using the controller endpoint
POST /api/v1/registrars/1/domains/download?save=true

// Or using the service directly
var count = await _registrarService.DownloadDomainsForRegistrarAsync(registrarId: 1, save: true);
// This will:
// 1. Fetch domains from registrar
// 2. Update domain info
// 3. Create/update TLDs
// 4. Create/update contacts
```

### Example 2: Just Retrieve Domains (No Save)
```csharp
GET /api/v1/registrars/1/domains?save=false

// Or using the service
var result = await _registrarService.GetRegisteredDomainsAsync(registrarId: 1, save: false);
// This will only fetch from registrar, no database changes
```

### Example 3: Retrieve and Save
```csharp
GET /api/v1/registrars/1/domains?save=true

// Or using the service
var result = await _registrarService.GetRegisteredDomainsAsync(registrarId: 1, save: true);
// Returns full result AND saves to database
```

## Statistics Example

After a successful sync, you'll see logs like:

```
Database merge completed: 25 domains updated, 3 TLDs created, 5 RegistrarTlds created, 48 contacts created, 27 contacts updated
```

This indicates:
- 25 existing domains were updated
- 3 new TLDs were discovered and created
- 5 new TLD-Registrar relationships created
- 48 new contacts were created (e.g., 25 domains × ~2 contacts each)
- 27 existing contacts were updated

## Contact Types Handled

The system processes the following contact types from registrars:

| Type | Description | AWS Route 53 | Other Registrars |
|------|-------------|--------------|------------------|
| Registrant | Domain owner | ? Included | ?? Varies |
| Admin | Administrative contact | ? Included | ?? Varies |
| Technical | Technical contact | ? Included | ?? Varies |
| Billing | Billing contact | ? Not included | ?? Varies |

**Note:** AWS Route 53 currently returns Registrant, Admin, and Technical contacts. Other registrars may vary.

## Important Considerations

### 1. Domain Must Exist
- Contacts can only be created/updated for domains that already exist in the database
- Domains not in the database are skipped (with log message)
- This is because domains require `CustomerId` and `ServiceId` which registrars don't provide

### 2. Contact Uniqueness
- Each domain can have multiple contacts of different types
- Each domain can have only ONE contact per type with the same email
- Duplicate prevention: DomainId + ContactType + Email must be unique

### 3. Performance
- Contacts are processed per-domain, not in batch
- Each contact lookup is a separate database query
- For large domain portfolios, consider:
  - Running during off-peak hours
  - Implementing batch processing
  - Adding caching

### 4. Data Consistency
- Contact updates are immediate (no soft delete)
- Old contact data is overwritten
- Consider implementing audit trail if needed

## Build Status

? **Build Successful** - All changes compile without errors

## Testing Recommendations

### Unit Tests
```csharp
[Fact]
public async Task MergeDomainContactsAsync_CreateNew_WhenNotExists()
{
    // Test creating new contacts
}

[Fact]
public async Task MergeDomainContactsAsync_UpdateExisting_WhenExists()
{
    // Test updating existing contacts
}

[Fact]
public async Task MergeDomainContactsAsync_SkipInvalid_WhenMissingRequired()
{
    // Test validation and skipping
}
```

### Integration Tests
```csharp
[Fact]
public async Task DownloadDomainsForRegistrar_MergesContacts_Successfully()
{
    // Test end-to-end contact merging
}
```

## Related Documentation

- **DomainContact Entity**: `DR_Admin/Data/Entities/DomainContact.cs`
- **DomainContactInfo Model**: `DomainRegistrationLib/Models/DomainContactInfo.cs`
- **Main Implementation**: `DR_Admin/Documentation/DomainContact_Implementation.md`
- **Registrar Integration**: `DR_Admin/Documentation/DomainContact_Registrar_Integration.md`

## Next Steps

1. **Run Database Migration** (if not already done)
   ```bash
   dotnet ef migrations add AddDomainContactTable --project DR_Admin
   dotnet ef database update --project DR_Admin
   ```

2. **Test with AWS Route 53**
   ```csharp
   POST /api/v1/registrars/{awsRegistrarId}/domains/download?save=true
   ```

3. **Verify Contact Creation**
   ```csharp
   GET /api/v1/domaincontacts/domain/{domainId}
   ```

4. **Add Indexes** (for performance)
   - Index on `(DomainId, ContactType, Email)` for faster lookups
   - Index on `DomainId` for contact queries by domain

5. **Consider Enhancement: Contact Deactivation**
   Instead of just creating/updating, consider:
   - Marking old contacts as inactive when they're no longer returned by registrar
   - Implementing soft delete strategy
   - Maintaining contact history

## Conclusion

The `MergeRegisteredDomainsToDatabase` method now provides complete domain synchronization including contact persons. When domains are downloaded from registrars (especially AWS Route 53), all contact information is automatically saved to the `DomainContacts` table with proper validation, duplicate prevention, and comprehensive logging.
