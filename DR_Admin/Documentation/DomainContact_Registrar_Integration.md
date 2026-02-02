# Domain Contact Integration Guide

## Overview
This document describes how to integrate domain contact information from registrars with the ISPAdmin domain contact storage system.

## Changes Made

### 1. New Model: DomainContactInfo
**File:** `DomainRegistrationLib/Models/DomainContactInfo.cs`

A new model class has been created to represent contact information returned from domain registrars. This model is designed to be compatible with the `ISPAdmin.DTOs.CreateDomainContactDto` format.

**Properties:**
- ContactType: The type of contact (Registrant, Admin, Technical, Billing)
- FirstName, LastName: Contact person's name
- Organization: Company/organization name (optional)
- Email: Email address
- Phone: Phone number
- Fax: Fax number (optional)
- Address1, Address2: Street address
- City, State, PostalCode: Location information
- CountryCode: ISO 3166-1 alpha-2 country code
- IsActive: Whether the contact is active (default: true)
- Notes: Additional notes (optional)

### 2. Updated Model: RegisteredDomainInfo
**File:** `DomainRegistrationLib/Models/RegisteredDomainsResult.cs`

The `RegisteredDomainInfo` class now includes:
```csharp
public List<DomainContactInfo> Contacts { get; set; } = [];
```

### 3. Registrar Implementations Updated

#### AWS Route 53 (Full Implementation)
**File:** `DomainRegistrationLib/Implementations/AwsRegistrar.cs`

AWS Route 53 now fetches and returns contact information for all registered domains:
- Registrant contact
- Admin contact
- Technical contact

A new helper method `MapFromAwsContact()` converts AWS contact details to `DomainContactInfo`.

#### Other Registrars (Placeholder)
The following registrars have been updated with empty contact lists and documentation comments:
- **GoDaddy**: Contact info requires individual API calls per domain
- **Cloudflare**: Contact info requires individual API calls per domain
- **DomainBox**: Contact info requires individual API calls per domain
- **Namecheap**: Contact info requires individual API calls per domain
- **OpenSRS**: Contact info requires individual API calls per domain

**Note:** Most registrar APIs don't include contact information in their domain list endpoints. Fetching contact details would require individual API calls for each domain, which could be:
- Time-consuming for large domain portfolios
- Subject to rate limiting
- Potentially expensive depending on API pricing

## Converting to CreateDomainContactDto

To convert `DomainContactInfo` to `ISPAdmin.DTOs.CreateDomainContactDto`, you can use the following mapping:

```csharp
public CreateDomainContactDto MapToCreateDto(DomainContactInfo contact, int domainId)
{
    return new CreateDomainContactDto
    {
        ContactType = contact.ContactType,
        FirstName = contact.FirstName,
        LastName = contact.LastName,
        Organization = contact.Organization,
        Email = contact.Email,
        Phone = contact.Phone,
        Fax = contact.Fax,
        Address1 = contact.Address1,
        Address2 = contact.Address2,
        City = contact.City,
        State = contact.State,
        PostalCode = contact.PostalCode,
        CountryCode = contact.CountryCode,
        IsActive = contact.IsActive,
        Notes = contact.Notes,
        DomainId = domainId
    };
}
```

## Usage Example

### Fetching and Storing Domain Contacts

```csharp
// 1. Get registered domains from a registrar
var registrar = await _registrarService.GetRegistrarInstanceAsync(registrarId);
var domainsResult = await registrar.GetRegisteredDomainsAsync();

if (domainsResult.Success)
{
    foreach (var domainInfo in domainsResult.Domains)
    {
        // Find or create domain in database
        var domain = await _context.Domains
            .FirstOrDefaultAsync(d => d.Name == domainInfo.DomainName);
        
        if (domain != null && domainInfo.Contacts.Any())
        {
            // Store contact information
            foreach (var contact in domainInfo.Contacts)
            {
                var createDto = new CreateDomainContactDto
                {
                    ContactType = contact.ContactType,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    Organization = contact.Organization,
                    Email = contact.Email,
                    Phone = contact.Phone,
                    Fax = contact.Fax,
                    Address1 = contact.Address1,
                    Address2 = contact.Address2,
                    City = contact.City,
                    State = contact.State,
                    PostalCode = contact.PostalCode,
                    CountryCode = contact.CountryCode,
                    IsActive = contact.IsActive,
                    Notes = contact.Notes,
                    DomainId = domain.Id
                };
                
                await _domainContactService.CreateDomainContactAsync(createDto);
            }
        }
    }
}
```

### Complete Service Implementation Example

```csharp
public class DomainSyncService
{
    private readonly IRegistrarService _registrarService;
    private readonly IDomainContactService _domainContactService;
    private readonly ApplicationDbContext _context;
    
    public async Task SyncDomainContactsAsync(int registrarId)
    {
        // Get registrar instance
        var registrar = await _registrarService.GetRegistrarInstanceAsync(registrarId);
        
        // Fetch domains with contacts
        var domainsResult = await registrar.GetRegisteredDomainsAsync();
        
        if (!domainsResult.Success)
        {
            throw new Exception($"Failed to fetch domains: {domainsResult.Message}");
        }
        
        foreach (var domainInfo in domainsResult.Domains)
        {
            // Find domain in database
            var domain = await _context.Domains
                .FirstOrDefaultAsync(d => d.Name == domainInfo.DomainName);
            
            if (domain == null)
                continue; // Skip domains not in our database
            
            // Clear existing contacts (optional - or implement update logic)
            var existingContacts = await _context.DomainContacts
                .Where(dc => dc.DomainId == domain.Id)
                .ToListAsync();
            
            _context.DomainContacts.RemoveRange(existingContacts);
            
            // Add new contacts
            foreach (var contact in domainInfo.Contacts)
            {
                var createDto = MapToCreateDto(contact, domain.Id);
                await _domainContactService.CreateDomainContactAsync(createDto);
            }
        }
        
        await _context.SaveChangesAsync();
    }
    
    private CreateDomainContactDto MapToCreateDto(DomainContactInfo contact, int domainId)
    {
        return new CreateDomainContactDto
        {
            ContactType = contact.ContactType,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Organization = contact.Organization,
            Email = contact.Email,
            Phone = contact.Phone,
            Fax = contact.Fax,
            Address1 = contact.Address1,
            Address2 = contact.Address2,
            City = contact.City,
            State = contact.State,
            PostalCode = contact.PostalCode,
            CountryCode = contact.CountryCode,
            IsActive = contact.IsActive,
            Notes = contact.Notes,
            DomainId = domainId
        };
    }
}
```

## Contact Type Mapping

The system supports the following standard contact types:

| Contact Type | Description |
|--------------|-------------|
| Registrant   | Domain owner/registrant |
| Admin        | Administrative contact |
| Technical    | Technical contact |
| Billing      | Billing contact |

## Field Validation Notes

When creating domain contacts, ensure:

1. **Required Fields:**
   - ContactType
   - FirstName
   - LastName
   - Email
   - Phone
   - Address1
   - City
   - PostalCode
   - CountryCode
   - DomainId

2. **Country Codes:**
   - Use ISO 3166-1 alpha-2 format (e.g., "US", "CA", "GB")
   - AWS automatically provides these in the correct format

3. **Phone Numbers:**
   - Different registrars may have different formats
   - Store as-is from the registrar
   - Consider validation/normalization if needed

## Registrar-Specific Notes

### AWS Route 53
- ? Fully implemented - returns all contact types
- Contact information is fetched during domain listing
- May have rate limits - includes 100ms delay between domain queries

### Other Registrars
- ?? Contact information not included in list endpoints
- Would require additional API call per domain
- Consider implementing on-demand fetching when needed
- Example implementation:

```csharp
// Future enhancement: Fetch contacts for specific domain
public async Task<List<DomainContactInfo>> GetDomainContactsAsync(string domainName)
{
    // Make individual API call to fetch contact details
    // This would be implemented per registrar
}
```

## Future Enhancements

1. **Batch Contact Fetching:**
   - Implement background jobs to fetch contacts for high-priority domains
   - Add queueing system to manage API rate limits

2. **Caching Strategy:**
   - Cache contact information with TTL
   - Refresh only when needed or on schedule

3. **Selective Sync:**
   - Allow users to choose which domains to sync contacts for
   - Priority-based syncing

4. **Contact Validation:**
   - Add email validation
   - Add phone number normalization
   - Validate country codes against ISO standards

## API Documentation

For detailed API documentation on the domain contact endpoints, see:
- `/api/v1/domaincontacts` - Full CRUD operations
- Swagger/OpenAPI documentation at `/swagger`

## Database Migration

After implementing this integration, remember to run:

```bash
dotnet ef migrations add AddDomainContactTable --project DR_Admin
dotnet ef database update --project DR_Admin
```

## Support and Questions

For questions or issues related to domain contact integration, please refer to:
- `DR_Admin/Documentation/DomainContact_Implementation.md`
- API Controller: `DR_Admin/Controllers/DomainContactsController.cs`
- Service: `DR_Admin/Services/DomainContactService.cs`
