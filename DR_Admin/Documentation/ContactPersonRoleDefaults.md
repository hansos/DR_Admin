# Contact Person Role Defaults Feature

## Overview
This feature adds the ability to mark contact persons as default/preferred for specific domain contact roles, and provides an intelligent endpoint that returns contact persons sorted by preference and usage history.

## Database Changes

### ContactPerson Entity
Added four boolean flags to the `ContactPerson` entity:
- `IsDefaultOwner` - Marks contact as default for Registrant role
- `IsDefaultBilling` - Marks contact as default for Billing role
- `IsDefaultTech` - Marks contact as default for Technical role
- `IsDefaultAdministrator` - Marks contact as default for Administrative role

## API Endpoint

### GET `/api/v1/ContactPersons/customer/{customerId}/for-role/{roleType}`

Returns contact persons for a customer, categorized and sorted by preference and usage for a specific role.

#### Parameters
- `customerId` (int): The customer ID
- `roleType` (ContactRoleType enum): The role to filter by
  - `1` = Registrant (Owner)
  - `2` = Administrative
  - `3` = Technical
  - `4` = Billing

#### Response Structure
```json
{
  "contactPersons": [
    {
      "id": 1,
      "firstName": "John",
      "lastName": "Doe",
      "fullName": "John Doe",
      "email": "john@example.com",
      "phone": "+1234567890",
      "position": "IT Manager",
      "department": "Technology",
      "isPrimary": false,
      "isActive": true,
      "category": 1,
      "usageCount": 0,
      "customerId": 100
    }
  ]
}
```

#### Categories
Contact persons are automatically categorized into three tiers:

1. **Preferred (Category = 1)**
   - Contact persons marked with `IsDefault[Role]` flag for the requested role
   - Appears at the top of the list

2. **Frequently Used (Category = 2)**
   - Contact persons used 3+ times for the requested role
   - Based on `DomainContactAssignments` history

3. **Available (Category = 3)**
   - All other active contact persons for the customer

#### Sorting
Within each category, contact persons are sorted by:
1. Usage count (descending)
2. Last name (ascending)
3. First name (ascending)

## DTO Changes

### ContactPersonDto
Added default role flags:
- `IsDefaultOwner`
- `IsDefaultBilling`
- `IsDefaultTech`
- `IsDefaultAdministrator`

### CreateContactPersonDto & UpdateContactPersonDto
Both DTOs now include the default role flags, allowing clients to set/update these preferences.

### New DTOs
- `CategorizedContactPersonListResponse` - Wrapper for the categorized list
- `CategorizedContactPersonDto` - Contact person with category and usage count
- `ContactPersonCategory` enum - Defines the three categories

## Service Layer

### IContactPersonService
Added method:
```csharp
Task<CategorizedContactPersonListResponse> GetContactPersonsForRoleAsync(
    int customerId, 
    ContactRoleType roleType);
```

### ContactPersonService
Implementation includes:
- Fetches active contact persons for the customer
- Queries `DomainContactAssignments` to calculate usage counts per role
- Categorizes based on default flags and usage threshold (3+ uses)
- Sorts according to category, usage, and name

## Usage Examples

### Setting Default Contacts
When creating/updating a contact person, set the appropriate flags:
```json
{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane@example.com",
  "customerId": 100,
  "isDefaultBilling": true,
  "isDefaultTech": false
}
```

### Getting Contacts for Domain Registration
When registering a domain, call the endpoint for each role:
```
GET /api/v1/ContactPersons/customer/100/for-role/1  // Owner/Registrant
GET /api/v1/ContactPersons/customer/100/for-role/2  // Administrative
GET /api/v1/ContactPersons/customer/100/for-role/3  // Technical
GET /api/v1/ContactPersons/customer/100/for-role/4  // Billing
```

### UI Implementation Suggestions
1. **Pre-selection**: Auto-select the first contact from the Preferred category
2. **Visual Grouping**: Show category headers ("Preferred", "Frequently Used", "Available")
3. **Badges**: Add visual indicators for preferred contacts
4. **Filtering**: Allow users to filter by category
5. **Transparency**: Display usage count to show why someone is "frequently used"

## Database Migration
You'll need to add a migration to add the four new boolean columns to the `ContactPersons` table:
```bash
Add-Migration AddContactPersonRoleDefaults
Update-Database
```

## Authorization
The endpoint uses the existing `Customer.Read` policy, matching other contact person read operations.

## Performance Considerations
- The endpoint queries `DomainContactAssignments` to calculate usage counts
- Consider adding an index on `ContactPersonId` and `RoleType` if not already present
- Usage count calculation is done in-memory after fetching relevant assignments
